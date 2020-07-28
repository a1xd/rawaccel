#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "vec2.h"
#include "x64-util.hpp"
#include "external/tagged-union-single.h"

namespace rawaccel {

    /// <summary> Enum to hold acceleration implementation types (i.e. types of curves.) </summary>
    enum class mode { noaccel, linear, classic, natural, logarithmic, sigmoid, power };

    /// <summary> Struct to hold vector rotation details. </summary>
    struct rotator {

        /// <summary> Rotational vector, which points in the direction of the post-rotation positive y axis. </summary>
        vec2d rot_vec = { 1, 0 };

        /// <summary>
        /// Rotates given input vector according to struct's rotational vector.
        /// </summary>
        /// <param name="input">Input vector to be rotated</param>
        /// <returns>2d vector of rotated input.</returns>
        inline vec2d operator()(const vec2d& input) const {
            return {
                input.x * rot_vec.x - input.y * rot_vec.y,
                input.x * rot_vec.y + input.y * rot_vec.x
            };
        }

        rotator(double degrees) {
            double rads = degrees * M_PI / 180;
            rot_vec = { cos(rads), sin(rads) };
        }

        rotator() = default;
    };

    /// <summary> Struct to hold clamp (min and max) details for acceleration application </summary>
    struct accel_scale_clamp {
        double lo = 0;
        double hi = 9;

        /// <summary>
        /// Clamps given input to min at lo, max at hi.
        /// </summary>
        /// <param name="scale">Double to be clamped</param>
        /// <returns>Clamped input as double</returns>
        inline double operator()(double scale) const {
            return clampsd(scale, lo, hi);
        }

        accel_scale_clamp(double cap) : accel_scale_clamp() {
            if (cap <= 0) {
                // use default, effectively uncapped accel
                return;
            }

            if (cap < 1) {
                // assume negative accel
                lo = cap;
                hi = 1;
            }
            else hi = cap;
        }

        accel_scale_clamp() = default;
    };

// Error throwing calls std libraries which are unavailable in kernel mode.
#ifdef _KERNEL_MODE
    void error(const char*) {}
#else
    void error(const char* s);
#endif

    using milliseconds = double;

    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct args_t {
        mode accel_mode = mode::noaccel;
        milliseconds time_min = 0.4;
        double offset = 0;
        double accel = 0;
        double lim_exp = 2;
        double midpoint = 0;
        vec2d weight = { 1, 1 };
        vec2d cap = { 0, 0 };
    };

    /// <summary>
    /// Struct to hold acceleration curve implementation details.
    /// </summary>
    /// <typeparam name="T">Type of acceleration.</typeparam>
    template <typename T>
    struct accel_implentation {
        
        /// <summary> The acceleration ramp rate.</summary>
        double b = 0;

        /// <summary> The limit or exponent for various modes. </summary>
        double k = 0;

        /// <summary> The midpoint in sigmoid mode. </summary>
        double m = 0;

        /// <summary> The offset past which acceleration is applied. Used in power mode. </summary>
        double offset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="accel_implementation{T}"/> struct.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        accel_implentation(args_t args)
        {
            b = args.accel;
            k = args.lim_exp - 1;
            m = args.midpoint;
            offset = args.offset;
        }

        /// <summary>
        /// Returns accelerated value of speed as a ratio of magnitude.
        /// </summary>
        /// <param name="speed">Mouse speed at which to calculate acceleration.</param>
        /// <returns>Ratio of accelerated movement magnitude to input movement magnitude.</returns>
        double accelerate(double speed) { return 0; }

        /// <summary>
        /// Verifies arguments as valid. Errors if not.
        /// </summary>
        /// <param name="args">Arguments to verified.</param>
        void verify(args_t args) {
            if (args.accel < 0) error("accel can not be negative, use a negative weight to compensate");
            if (args.time_min <= 0) error("min time must be positive");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        accel_implentation() = default;
    };

    /// <summary> Struct to hold linear acceleration implementation. </summary>
    struct accel_linear : accel_implentation<accel_linear> {

        accel_linear(args_t args)
            : accel_implentation(args) {}

        double accelerate(double speed) {
            //f(x) = mx
            return b * speed;
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 1) error("limit must be greater than 1");
        }
    };

    /// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
    struct accel_classic : accel_implentation<accel_classic> {
        accel_classic(args_t args)
            : accel_implentation(args) {}

        double accelerate(double speed) {
            //f(x) = (mx)^k
            return pow(b * speed, k);
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 1) error("exponent must be greater than 1");
        }
    };

    /// <summary> Struct to hold "natural" (vanishing difference) acceleration implementation. </summary>
    struct accel_natural : accel_implentation<accel_natural> {
        accel_natural(args_t args)
            : accel_implentation(args) 
        { b /= k; }

        double accelerate(double speed) {
            // f(x) = k(1-e^(-mx))
            return k - (k * exp(-b * speed));;
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 1) error("exponent must be greater than 1");
        }
    };

    /// <summary> Struct to hold logarithmic acceleration implementation. </summary>
    struct accel_logarithmic : accel_implentation<accel_logarithmic> {
        accel_logarithmic(args_t args)
            : accel_implentation(args) {}

        double accelerate(double speed) {
            return log(speed * b + 1);
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 1) error("exponent must be greater than 1");
        }
    };

    /// <summary> Struct to hold sigmoid (s-shaped) acceleration implementation. </summary>
    struct accel_sigmoid : accel_implentation<accel_sigmoid> {
        accel_sigmoid(args_t args)
            : accel_implentation(args) {}

        double accelerate(double speed) {
            //f(x) = k/(1+e^(-m(c-x)))
            return k / (exp(-b * (speed - m)) + 1);
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 1) error("exponent must be greater than 1");
        }
    };

    /// <summary> Struct to hold power (non-additive) acceleration implementation. </summary>
    struct accel_power : accel_implentation<accel_power> {
        accel_power(args_t args)
            : accel_implentation(args)
        { k++; }

        double accelerate(double speed) {
            // f(x) = (mx)^k - 1
            // The subtraction of 1 occurs with later addition of 1 in mind, 
            // so that the input vector is directly multiplied by (mx)^k (if unweighted)
            return (offset > 0 && speed < 1) ? 0 : pow(speed * b, k) - 1;
        }

        void verify(args_t args) {
            accel_implentation::verify(args);
            if (args.lim_exp <= 0) error("exponent must be greater than 0");
        }
    };

    /// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
    struct accel_noaccel : accel_implentation<accel_noaccel> {
        accel_noaccel(args_t args)
            : accel_implentation(args) {}

        double accelerate(double speed) { return 0; }

        void verify(args_t args) {}
    };
    
    /// <summary> Tagged union to hold all accel implementations and allow "polymorphism" via a visitor call. </summary>
    using accel_implementation_t = tagged_union<accel_linear, accel_classic, accel_natural, accel_logarithmic, accel_sigmoid, accel_power, accel_noaccel>;

    /// <summary> Struct for holding acceleration application details. </summary>
    struct accel_function {

        /*
        This value is ideally a few microseconds lower than
        the user's mouse polling interval, though it should
        not matter if the system is stable.
        */
        /// <summary> The minimum time period for one mouse movement. </summary>
        milliseconds time_min = 0.4;

        /// <summary> The offset past which acceleration is applied. </summary>
        double speed_offset = 0;

        /// <summary> The acceleration implementation (i.e. curve) </summary>
        accel_implementation_t accel;

        /// <summary> The weight of acceleration applied in {x, y} dimensions. </summary>
        vec2d weight = { 1, 1 };

        /// <summary> The object which sets a min and max for the acceleration scale. </summary>
        vec2<accel_scale_clamp> clamp;

        accel_function(args_t args) {
            switch (args.accel_mode)
            {
				case mode::linear: accel = accel_linear(args);
                    break;
				case mode::classic: accel = accel_classic(args);
                    break;
				case mode::natural: accel = accel_natural(args);
                    break;
				case mode::logarithmic: accel = accel_logarithmic(args);
                    break;
				case mode::sigmoid: accel = accel_sigmoid(args);
                    break;
				case mode::power: accel = accel_power(args);
            }

            // Verification is performed by the accel_implementation object
            // and therefore must occur after the object has been instantiated
            verify(args);

            time_min = args.time_min;
            speed_offset = args.offset;
            weight = args.weight;
            clamp.x = accel_scale_clamp(args.cap.x);
            clamp.y = accel_scale_clamp(args.cap.y);
        }

        /// <summary>
        /// Applies mouse acceleration to a given speed, via visitor function to accel_implementation_t
        /// </summary>
        /// <param name="speed">Speed from which to determine acceleration</param>
        /// <returns>Acceleration as a ratio magnitudes, as a double</returns>
        double apply(double speed) const {
            return accel.visit([=](auto accel_t) { return accel_t.accelerate(speed); });
        }

        /// <summary>
        /// Verifies acceleration arguments, via visitor function to accel_implementation_t
        /// </summary>
        /// <param name="args">Arguments to be verified</param>
        void verify(args_t args) const {
            return accel.visit([=](auto accel_t) { accel_t.verify(args); });
        }

        /// <summary>
        /// Applies weighted acceleration to given input for given time period.
        /// </summary>
        /// <param name="input">2d vector of {x, y} mouse movement to be accelerated</param>
        /// <param name="time">Time period over which input movement was accumulated</param>
        /// <returns></returns>
        inline vec2d operator()(const vec2d& input, milliseconds time) const {
            double mag = sqrtsd(input.x * input.x + input.y * input.y);
            double time_clamped = clampsd(time, time_min, 100);
            double speed = maxsd(mag / time_clamped - speed_offset, 0);

            double accel_val = apply(speed);

            double scale_x = weight.x * accel_val + 1;
            double scale_y = weight.y * accel_val + 1;

            return {
                input.x * clamp.x(scale_x),
                input.y * clamp.y(scale_y)
            };
        }

        accel_function() = default;
    };

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_accel = false;
        rotator rotate;
        accel_function accel_fn;
        vec2d sensitivity = { 1, 1 };

        mouse_modifier(double degrees, vec2d sens, args_t accel_args)
            : accel_fn(accel_args)
        {
            apply_rotate = degrees != 0;
            if (apply_rotate) rotate = rotator(degrees);
            else rotate = rotator();

            apply_accel = accel_args.accel_mode != mode::noaccel;

            if (sens.x == 0) sens.x = 1;
            if (sens.y == 0) sens.y = 1;
            sensitivity = sens;
        }

        /// <summary>
        /// Applies modification without acceleration. Rotation is the only
        /// modification currently implemented.
        /// </summary>
        /// <param name="input">Input to be modified.</param>
        /// <returns>2d vector of modified input.</returns>
        inline vec2d modify(vec2d input)
        {
            if (apply_rotate)
            {
                return rotate(input);
            }

            return input;
        }

        /// <summary>
        /// Applies modification, including acceleration.
        /// </summary>
        /// <param name="input">Input to be modified</param>
        /// <param name="time">Time period for determining acceleration.</param>
        /// <returns>2d vector with modified input.</returns>
        inline vec2d modify(vec2d input, milliseconds time)
        {
            return accel_fn(modify(input), time);
        }

        mouse_modifier() = default;
    };

} // rawaccel
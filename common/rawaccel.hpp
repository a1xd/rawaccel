#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "vec2.h"
#include "x64-util.hpp"
#include "external/tagged-union-single.h"

namespace rawaccel {

    enum class mode { noaccel, linear, classic, natural, logarithmic, sigmoid, power };

    struct rotator {
        vec2d rot_vec = { 1, 0 };

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

    struct accel_scale_clamp {
        double lo = 0;
        double hi = 9;

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

#ifdef _KERNEL_MODE
    void error(const char*) {}
#else
    void error(const char* s);
#endif

    using milliseconds = double;

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
    /// Struct to hold acceleration implementation details.
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
            //f(x) = (mx)^k - 1
            // The subtraction of 1 is with later addition of 1 in mind, 
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
    
    using accel_implementation_t = tagged_union<accel_linear, accel_classic, accel_natural, accel_logarithmic, accel_sigmoid, accel_power, accel_noaccel>;

    struct accel_function {

        /*
        This value is ideally a few microseconds lower than
        the user's mouse polling interval, though it should
        not matter if the system is stable.
        */
        milliseconds time_min = 0.4;

        /// <summary> The offset past which acceleration is applied. </summary>
        double speed_offset = 0;

        accel_implementation_t accel;

        vec2d weight = { 1, 1 };
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

            verify(args);

            time_min = args.time_min;
            speed_offset = args.offset;
            weight = args.weight;
            clamp.x = accel_scale_clamp(args.cap.x);
            clamp.y = accel_scale_clamp(args.cap.y);
        }

        double apply(double speed) const {
            return accel.visit([=](auto accel_t) { return accel_t.accelerate(speed); });
        }

        void verify(args_t args) const {
            return accel.visit([=](auto accel_t) { accel_t.verify(args); });
        }

        inline vec2d operator()(const vec2d& input, milliseconds time, mode accel_mode) const {
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

    struct variables {
        bool apply_rotate = false;
        bool apply_accel = false;
        mode accel_mode = mode::noaccel;
        rotator rotate;
        accel_function accel_fn;
        vec2d sensitivity = { 1, 1 };

        variables(double degrees, vec2d sens, args_t accel_args)
            : accel_fn(accel_args)
        {
            apply_rotate = degrees != 0;
            if (apply_rotate) rotate = rotator(degrees);
            else rotate = rotator();

            apply_accel = accel_args.accel_mode != mode::noaccel;
            accel_mode = accel_args.accel_mode;

            if (sens.x == 0) sens.x = 1;
            if (sens.y == 0) sens.y = 1;
            sensitivity = sens;
        }

        variables() = default;
    };

} // rawaccel
#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "vec2.h"
#include "x64-util.hpp"
#include "external/tagged-union-single.h"

#include "accel_linear.cpp"
#include "accel_classic.cpp"
#include "accel_natural.cpp"
#include "accel_logarithmic.cpp"
#include "accel_sigmoid.cpp"
#include "accel_power.cpp"
#include "accel_noaccel.cpp"

namespace rawaccel {

    /// <summary> Struct to hold vector rotation details. </summary>
    struct rotator {

        /// <summary> Rotational vector, which points in the direction of the post-rotation positive x axis. </summary>
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

    /// <summary> Tagged union to hold all accel implementations and allow "polymorphism" via a visitor call. </summary>
    using accel_implementation_t = tagged_union<accel_linear, accel_classic, accel_natural, accel_logarithmic, accel_sigmoid, accel_power, accel_noaccel>;

    struct accel_fn_args {
        accel_args acc_args = accel_args{};
        int accel_mode = 0;
        milliseconds time_min = 0.4;
        vec2d weight = { 1, 1 };
        vec2d cap = { 0, 0 };
    };

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

        accel_function(accel_fn_args args) {
            accel.tag = args.accel_mode;
            accel.visit([&](auto& a){ a = {args.acc_args}; });

            // Verification is performed by the accel_implementation object
            // and therefore must occur after the object has been instantiated
            verify(args.acc_args);

            time_min = args.time_min;
            speed_offset = args.acc_args.offset;
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
        void verify(accel_args args) const {
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

    struct modifier_args
    {
        double degrees = 0;
        vec2d sens = { 1, 1 };
        accel_fn_args acc_fn_args = accel_fn_args{};
    };

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_accel = false;
        rotator rotate;
        accel_function accel_fn;
        vec2d sensitivity = { 1, 1 };

        mouse_modifier(modifier_args args)
            : accel_fn(args.acc_fn_args)
        {
            apply_rotate = args.degrees != 0;
            if (apply_rotate) rotate = rotator(args.degrees);
            else rotate = rotator();

            apply_accel = (args.acc_fn_args.accel_mode != 0 &&
						   args.acc_fn_args.accel_mode != accel_implementation_t::id<accel_noaccel>);

            if (args.sens.x == 0) args.sens.x = 1;
            if (args.sens.y == 0) args.sens.y = 1;
            sensitivity = args.sens;
        }

        /// <summary>
        /// Applies modification without acceleration.
        /// </summary>
        /// <param name="input">Input to be modified.</param>
        /// <returns>2d vector of modified input.</returns>
        inline vec2d modify_without_accel(vec2d input)
        {
            if (apply_rotate)
            {
                input = rotate(input);
            }

            input.x *= sensitivity.x;
            input.y *= sensitivity.y;

            return input;
        }

        /// <summary>
        /// Applies modification, including acceleration.
        /// </summary>
        /// <param name="input">Input to be modified</param>
        /// <param name="time">Time period for determining acceleration.</param>
        /// <returns>2d vector with modified input.</returns>
        inline vec2d modify_with_accel(vec2d input, milliseconds time)
        {
            if (apply_rotate)
            {
                input = rotate(input);
            }

            if (apply_accel)
            {
				input = accel_fn(input, time);
            }

            input.x *= sensitivity.x;
            input.y *= sensitivity.y;

            return input;
        }

        mouse_modifier() = default;
    };

} // rawaccel
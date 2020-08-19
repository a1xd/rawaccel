#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "x64-util.hpp"
#include "tagged-union-single.h"

#include "accel-linear.hpp"
#include "accel-classic.hpp"
#include "accel-natural.hpp"
#include "accel-naturalgain.hpp"
#include "accel-logarithmic.hpp"
#include "accel-sigmoid.hpp"
#include "accel-power.hpp"
#include "accel-noaccel.hpp"

namespace rawaccel {

    using milliseconds = double;

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
    using accel_impl_t = tagged_union<accel_linear, accel_classic, accel_natural, accel_logarithmic, accel_sigmoid, accel_power, accel_naturalgain, accel_noaccel>;

    /// <summary> Struct to hold information about applying a gain cap. </summary>
    struct velocity_gain_cap {

        // <summary> The minimum speed past which gain cap is applied. </summary>
        double threshold = 0;

        // <summary> The gain at the point of cap </summary>
        double slope = 0;

        // <summary> The intercept for the line with above slope to give continuous velocity function </summary>
        double intercept = 0;

        // <summary> Whether or not velocity gain cap is enabled </summary>
        bool cap_gain_enabled = false;

        /// <summary>
        /// Initializes a velocity gain cap for a certain speed threshold
        /// by estimating the slope at the threshold and creating a line
        /// with that slope for output velocity calculations.
        /// </summary>
        /// <param name="speed"> The speed at which velocity gain cap will kick in </param>
        /// <param name="offset"> The offset applied in accel calculations </param>
        /// <param name="accel"> The accel implementation used in the containing accel_fn </param>
        velocity_gain_cap(double speed, double offset, accel_impl_t accel)
        {
            if (speed <= 0) return;

            // Estimate gain at cap point by taking line between two input vs output velocity points.
            // First input velocity point is at cap; for second pick a velocity a tiny bit larger.
            double speed_second = 1.001 * speed;
            double speed_diff = speed_second - speed;

            // Return if by glitch or strange values the difference in points is 0.
            if (speed_diff == 0) return;

            cap_gain_enabled = true;

            // Find the corresponding output velocities for the two points.
            // Subtract offset for acceleration, like in accel_fn()
			double out_first = accel.visit([=](auto&& impl) {
                double accel_val = impl.accelerate(speed-offset);
                return impl.scale(accel_val); 
            }).x * speed;
			double out_second = accel.visit([=](auto&& impl) {
                double accel_val = impl.accelerate(speed_second-offset);
                return impl.scale(accel_val); 
            }).x * speed_second;

            // Calculate slope and intercept from two points.
            slope = (out_second - out_first) / speed_diff;
            intercept = out_first - slope * speed;

            threshold = speed;
        }

        /// <summary>
        /// Applies velocity gain cap to speed.
        /// Returns scale value by which to multiply input to place on gain cap line.
        /// </summary>
        /// <param name="speed"> Speed to be capped </param>
        /// <returns> Scale multiplier for input </returns>
        inline double operator()(double speed) const {
			return  slope + intercept / speed;
        }

        /// <summary>
        /// Whether gain cap should be applied to given speed.
        /// </summary>
        /// <param name="speed"> Speed to check against threshold. </param>
        /// <returns> Whether gain cap should be applied. </returns>
        inline bool should_apply(double speed) const {
            return cap_gain_enabled && speed > threshold;
        }

        velocity_gain_cap() = default;
    };

    struct accel_fn_args {
        accel_args acc_args;
        int accel_mode = accel_impl_t::id<accel_noaccel>;
        milliseconds time_min = 0.4;
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
        accel_impl_t accel;

        /// <summary> The object which sets a min and max for the acceleration scale. </summary>
        vec2<accel_scale_clamp> clamp;

        velocity_gain_cap gain_cap = velocity_gain_cap();

        accel_function(const accel_fn_args& args) {
            if (args.time_min <= 0) bad_arg("min time must be positive");
            if (args.acc_args.offset < 0) bad_arg("offset must not be negative");

            accel.tag = args.accel_mode;
            accel.visit([&](auto& impl) { impl = { args.acc_args }; });

            time_min = args.time_min;
            speed_offset = args.acc_args.offset;
            clamp.x = accel_scale_clamp(args.cap.x);
            clamp.y = accel_scale_clamp(args.cap.y);
			gain_cap = velocity_gain_cap(args.acc_args.gain_cap, speed_offset, accel);
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
            double raw_speed = mag / time_clamped;
                
            vec2d scale;
            
            // gain_cap needs raw speed for velocity line calculation
            if (gain_cap.should_apply(raw_speed))
            {
                double gain_cap_scale = gain_cap(raw_speed);
                scale = { gain_cap_scale, gain_cap_scale };
            }
            else
            {
                scale = accel.visit([=](auto&& impl) {
                    double accel_val = impl.accelerate(maxsd(mag / time_clamped - speed_offset, 0));
                    return impl.scale(accel_val);
                    });
            }

            return {
                input.x * clamp.x(scale.x),
                input.y * clamp.y(scale.y)
            };
        }

        accel_function() = default;
    };

    struct modifier_args {
        double degrees = 0;
        vec2d sens = { 1, 1 };
        accel_fn_args acc_fn_args;
    };

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_accel = false;
        rotator rotate;
        accel_function accel_fn;
        vec2d sensitivity = { 1, 1 };

        mouse_modifier(const modifier_args& args)
            : accel_fn(args.acc_fn_args)
        {
            apply_rotate = args.degrees != 0;

            if (apply_rotate) rotate = rotator(args.degrees);
            else rotate = rotator();

            apply_accel = args.acc_fn_args.accel_mode != 0 &&
                args.acc_fn_args.accel_mode != accel_impl_t::id<accel_noaccel>;

            if (args.sens.x == 0) sensitivity.x = 1;
            else sensitivity.x = args.sens.x;

            if (args.sens.y == 0) sensitivity.y = 1;
            else sensitivity.y = args.sens.y;
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

			input = accel_fn(input, time);

            input.x *= sensitivity.x;
            input.y *= sensitivity.y;

            return input;
        }

        mouse_modifier() = default;
    };

} // rawaccel
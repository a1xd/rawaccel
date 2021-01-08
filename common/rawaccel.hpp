#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "rawaccel-settings.h"
#include "x64-util.hpp"

#include "accel-linear.hpp"
#include "accel-classic.hpp"
#include "accel-natural.hpp"
#include "accel-naturalgain.hpp"
#include "accel-power.hpp"
#include "accel-motivity.hpp"
#include "accel-noaccel.hpp"

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
        inline vec2d apply(const vec2d& input) const {
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
        double hi = 128;

        /// <summary>
        /// Clamps given input to min at lo, max at hi.
        /// </summary>
        /// <param name="scale">Double to be clamped</param>
        /// <returns>Clamped input as double</returns>
        inline double operator()(double scale) const {
            return clampsd(scale, lo, hi);
        }

        accel_scale_clamp(double cap) {
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
    
    template <typename Visitor, typename Variant>
    inline auto visit_accel(Visitor vis, Variant&& var) {
        switch (var.tag) {
        case accel_mode::linear:      return vis(var.u.linear);
        case accel_mode::classic:     return vis(var.u.classic);
        case accel_mode::natural:     return vis(var.u.natural);
        case accel_mode::naturalgain: return vis(var.u.naturalgain);
        case accel_mode::power:       return vis(var.u.power);
        case accel_mode::motivity:    return vis(var.u.motivity);
        default:                      return vis(var.u.noaccel);
        }
    }

    struct accel_variant {
        si_pair* lookup;

        accel_mode tag = accel_mode::noaccel;

        union union_t {
            accel_linear linear;
            accel_classic classic;
            accel_natural natural;
            accel_naturalgain naturalgain;
            accel_power power;
            accel_motivity motivity;
            accel_noaccel noaccel = {};
        } u = {};

        accel_variant(const accel_args& args, accel_mode mode, si_pair* lut = nullptr) :
            tag(mode), lookup(lut)
        {
            visit_accel([&](auto& impl) {
                impl = { args }; 
            }, *this);

            if (lookup && tag == accel_mode::motivity) {
                u.motivity.fn.fill(lookup);
            }

        }

        inline double apply(double speed) const {
            if (lookup && tag == accel_mode::motivity) {
                return u.motivity.fn.apply(lookup, speed);
            }

            return visit_accel([=](auto&& impl) {
                return impl(speed);
            }, *this);
        }

        accel_variant() = default;
    };

    /// <summary> Struct to hold information about applying a gain cap. </summary>
    struct velocity_gain_cap {

        // <summary> The minimum speed past which gain cap is applied. </summary>
        double threshold = 0;

        // <summary> The gain at the point of cap </summary>
        double slope = 0;

        // <summary> The intercept for the line with above slope to give continuous velocity function </summary>
        double intercept = 0;

        /// <summary>
        /// Initializes a velocity gain cap for a certain speed threshold
        /// by estimating the slope at the threshold and creating a line
        /// with that slope for output velocity calculations.
        /// </summary>
        /// <param name="speed"> The speed at which velocity gain cap will kick in </param>
        /// <param name="accel"> The accel implementation used in the containing accel_variant </param>
        velocity_gain_cap(double speed, const accel_variant& accel)
        {
            if (speed <= 0) return;

            // Estimate gain at cap point by taking line between two input vs output velocity points.
            // First input velocity point is at cap; for second pick a velocity a tiny bit larger.
            double speed_second = 1.001 * speed;
            double speed_diff = speed_second - speed;

            // Return if by glitch or strange values the difference in points is 0.
            if (speed_diff == 0) return;

            // Find the corresponding output velocities for the two points.
            double out_first = accel.apply(speed) * speed;
            double out_second = accel.apply(speed_second) * speed_second;

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
        inline double apply(double speed) const {
			return  slope + intercept / speed;
        }

        /// <summary>
        /// Whether gain cap should be applied to given speed.
        /// </summary>
        /// <param name="speed"> Speed to check against threshold. </param>
        /// <returns> Whether gain cap should be applied. </returns>
        inline bool should_apply(double speed) const {
            return threshold > 0 && speed > threshold;
        }

        velocity_gain_cap() = default;
    };

    struct accelerator {
        accel_variant accel;
        velocity_gain_cap gain_cap;
        accel_scale_clamp clamp;
        double output_speed_cap = 0;

        accelerator(const accel_args& args, accel_mode mode, si_pair* lut = nullptr) :
            accel(args, mode, lut), gain_cap(args.gain_cap, accel), clamp(args.scale_cap)
        {
            output_speed_cap = maxsd(args.speed_cap, 0);
        }

        inline double apply(double speed) const { 
            double scale;

            if (gain_cap.should_apply(speed)) {
                scale = gain_cap.apply(speed);
            }
            else {
                scale = accel.apply(speed);
            }

            scale = clamp(scale);

            if (output_speed_cap > 0 && (scale * speed) > output_speed_cap) {
                scale = output_speed_cap / speed;
            }

            return scale;
        }

        accelerator() = default;
    };

    struct stigma_args {
        vec2d sigmas = { 1, 1 };
        double lp_norm = 2;
    };

    struct stigma_distance {
        double p = 2.0;
        double p_inverse = 0.5;
        bool lp_norm_infinity = false;
        double sigma_x = 1.0;
        double sigma_y = 1.0;

        stigma_distance(stigma_args args)
        {
            sigma_x = args.sigmas.x;
            sigma_y = args.sigmas.y;
            if (args.lp_norm <= 0)
            {
                lp_norm_infinity = true;
                p = 0.0;
                p_inverse = 0.0;
            }
            else
            {
                lp_norm_infinity = false;
                p = args.lp_norm;
                p_inverse = 1 / args.lp_norm;
            }
        }

        double calculate(int x, int y)
        {
            double x_scaled = x * sigma_x;
            double y_scaled = y * sigma_y;
            return pow(pow(x_scaled, p) + pow(y_scaled, p), p_inverse);
        }

        stigma_distance() = default;
    };

    struct direction_weight {
        double diff = 0.0;
        double start = 1.0;
        bool should_apply = false;

        direction_weight(vec2d thetas)
        {
            diff = thetas.y - thetas.x;
            start = thetas.x;

            if (diff != 0)
            {
                should_apply = true;
            }
            else
            {
                should_apply = false;
            }
        }

        inline int atan_scale(double x, double y)
        {
            return M_2_PI * atan2(fabs(y), fabs(x));
        }

        double apply(double x, double y)
        {
            return atan_scale(x, y) * diff + start;
        }

        direction_weight() = default;
    };

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_accel = false;
        bool combine_magnitudes = true;
        rotator rotate;
        stigma_distance stigma;
        direction_weight directional;
        vec2<accelerator> accels;
        vec2d sensitivity = { 1, 1 };
        vec2d directional_multipliers = {};

        mouse_modifier(const settings& args, vec2<si_pair*> luts = {}) :
            combine_magnitudes(args.combine_mags)
        {
            if (args.degrees_rotation != 0) {
                rotate = rotator(args.degrees_rotation);
                apply_rotate = true;
            }
            
            if (args.sens.x != 0) sensitivity.x = args.sens.x;
            if (args.sens.y != 0) sensitivity.y = args.sens.y;

            directional_multipliers.x = fabs(args.dir_multipliers.x);
            directional_multipliers.y = fabs(args.dir_multipliers.y);

            if ((combine_magnitudes && args.modes.x == accel_mode::noaccel) ||
                (args.modes.x == accel_mode::noaccel &&
                    args.modes.y == accel_mode::noaccel)) {
                return;
            }

            accels.x = accelerator(args.argsv.x, args.modes.x, luts.x);
            accels.y = accelerator(args.argsv.y, args.modes.y, luts.y);

            stigma = stigma_distance(args.args_stigma);
            directional = direction_weight(args.directional_weights);

            apply_accel = true;
        }

        void modify(vec2d& movement, milliseconds time) {
            apply_rotation(movement);
            apply_acceleration(movement, [=] { return time; });
            apply_sensitivity(movement);
        }

        inline void apply_rotation(vec2d& movement) {
            if (apply_rotate) movement = rotate.apply(movement);
        }

        template <typename TimeSupplier>
        inline void apply_acceleration(vec2d& movement, TimeSupplier time_supp) {
            if (apply_accel) {
                milliseconds time = time_supp();

                if (combine_magnitudes) {
                    double mag = stigma.calculate(movement.x, movement.y);
                    double speed = mag / time;
                    double scale = accels.x.apply(speed);

                    if (directional.should_apply)
                    {
                        scale *= directional.apply(movement.x, movement.y);
                    }

                    movement.x *= scale;
                    movement.y *= scale;
                }
                else {
                    movement.x *= accels.x.apply(fabs(movement.x) / time);
                    movement.y *= accels.y.apply(fabs(movement.y) / time);
                }
            }
        }

        inline void apply_sensitivity(vec2d& movement) {   
            movement.x *= sensitivity.x;
            movement.y *= sensitivity.y;

            if (directional_multipliers.x > 0 && movement.x < 0) {
                movement.x *= directional_multipliers.x;
            }
            if (directional_multipliers.y > 0 && movement.y < 0) {
                movement.y *= directional_multipliers.y;
            }
        }

        mouse_modifier() = default;
    };

} // rawaccel

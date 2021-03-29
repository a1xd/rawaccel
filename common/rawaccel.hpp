#pragma once

#include "accel-union.hpp"
#include "utility.hpp"

#define _USE_MATH_DEFINES
#include <math.h>

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

    struct snapper {
        double threshold = 0;

        inline vec2d apply(const vec2d& input) const {
            if (input.x != 0 && input.y != 0) {
                double angle = fabs(atan(input.y / input.x));
                auto mag = [&] { return sqrtsd(input.x * input.x + input.y * input.y); };

                if (angle > M_PI_2 - threshold) return { 0, _copysign(mag(), input.y) };
                if (angle < threshold) return { _copysign(mag(), input.x), 0 };
            }

            return input;
        }

        snapper(double degrees) : threshold(minsd(fabs(degrees), 45) * M_PI / 180) {}

        snapper() = default;
    };

    inline void cap_speed(vec2d& v, double cap, double norm) {
        double speed = sqrtsd(v.x * v.x + v.y * v.y) * norm;
        double ratio = minsd(speed, cap) / speed;
        v.x *= ratio;
        v.y *= ratio;
    }
    
    struct weighted_distance {
        double p = 2.0;
        double p_inverse = 0.5;
        bool lp_norm_infinity = false;
        double sigma_x = 1.0;
        double sigma_y = 1.0;

        weighted_distance(const domain_args& args)
        {
            sigma_x = args.domain_weights.x;
            sigma_y = args.domain_weights.y;
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

        inline double calculate(double x, double y)
        {
			double abs_x = fabs(x);
			double abs_y = fabs(y);

            if (lp_norm_infinity) return maxsd(abs_x, abs_y);

            double x_scaled = abs_x * sigma_x;
            double y_scaled = abs_y * sigma_y;

            if (p == 2) return sqrtsd(x_scaled * x_scaled + y_scaled * y_scaled);
            else return pow(pow(x_scaled, p) + pow(y_scaled, p), p_inverse);
        }

        weighted_distance() = default;
    };

    struct direction_weight {
        double diff = 0.0;
        double start = 1.0;
        bool should_apply = false;

        direction_weight(const vec2d& thetas)
        {
            diff = thetas.y - thetas.x;
            start = thetas.x;

            should_apply = diff != 0;
        }

        inline double atan_scale(double x, double y)
        {
            return M_2_PI * atan2(fabs(y), fabs(x));
        }

        inline double apply(double x, double y)
        {
            return atan_scale(x, y) * diff + start;
        }

        direction_weight() = default;
    };

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_snap = false;
        bool apply_accel = false;
        bool by_component = false;
        rotator rotate;
        snapper snap;
        double dpi_factor = 1;
        double speed_cap = 0;
        weighted_distance distance;
        direction_weight directional;
        vec2<accel_variant> accels;
        vec2d sensitivity = { 1, 1 };
        vec2d directional_multipliers = {};

        mouse_modifier(const settings& args, vec2<si_pair*> luts = {}) :
            by_component(!args.combine_mags),
            dpi_factor(1000 / args.dpi),
            speed_cap(args.speed_cap)
        {
            if (args.degrees_rotation != 0) {
                rotate = rotator(args.degrees_rotation);
                apply_rotate = true;
            }
            
            if (args.degrees_snap != 0) {
                snap = snapper(args.degrees_snap);
                apply_snap = true;
            }

            if (args.sens.x != 0) sensitivity.x = args.sens.x;
            if (args.sens.y != 0) sensitivity.y = args.sens.y;

            directional_multipliers.x = fabs(args.dir_multipliers.x);
            directional_multipliers.y = fabs(args.dir_multipliers.y);

            if ((!by_component && args.argsv.x.mode == accel_mode::noaccel) ||
                (args.argsv.x.mode == accel_mode::noaccel &&
                    args.argsv.y.mode == accel_mode::noaccel)) {
                return;
            }

            accels.x = accel_variant(args.argsv.x, luts.x);
            accels.y = accel_variant(args.argsv.y, luts.y);

            distance = weighted_distance(args.dom_args);
            directional = direction_weight(args.range_weights);

            apply_accel = true;
        }

        void modify(vec2d& movement, milliseconds time) {
            apply_rotation(movement);
            apply_angle_snap(movement);
            apply_acceleration(movement, [=] { return time; });
            apply_sensitivity(movement);
        }

        inline void apply_rotation(vec2d& movement) {
            if (apply_rotate) movement = rotate.apply(movement);
        }

        inline void apply_angle_snap(vec2d& movement) {
            if (apply_snap) movement = snap.apply(movement);
        }

        template <typename TimeSupplier>
        inline void apply_acceleration(vec2d& movement, TimeSupplier time_supp) {
            if (apply_accel) {
                milliseconds time = time_supp();
                double norm = dpi_factor / time;

                if (speed_cap > 0) cap_speed(movement, speed_cap, norm);

                if (!by_component) {
                    double mag = distance.calculate(movement.x, movement.y);
                    double speed = mag * norm;
                    double scale = accels.x.apply(speed);

                    if (directional.should_apply)
                    {
                        scale = (scale - 1)*directional.apply(movement.x, movement.y) + 1;
                    }

                    movement.x *= scale;
                    movement.y *= scale;
                }
                else {
                    if (movement.x != 0) {
                        movement.x *= accels.x.apply(fabs(movement.x) * norm);
                    }
                    if (movement.y != 0) {
                        movement.y *= accels.y.apply(fabs(movement.y) * norm);
                    }
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

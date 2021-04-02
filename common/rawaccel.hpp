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

        snapper(double degrees) : threshold(minsd(fabs(degrees), 45)* M_PI / 180) {}

        snapper() = default;
    };

    inline void clamp_speed(vec2d& v, double min, double max, double norm) {
        double speed = sqrtsd(v.x * v.x + v.y * v.y) * norm;
        double ratio = clampsd(speed, min, max) / speed;
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
            double abs_scaled_x = fabs(x) * sigma_x;
            double abs_scaled_y = fabs(y) * sigma_y;

            if (lp_norm_infinity) {
                return maxsd(abs_scaled_x, abs_scaled_y);
            }

            if (p == 2) {
                return sqrtsd(abs_scaled_x * abs_scaled_x +
                    abs_scaled_y * abs_scaled_y);
            }

            double dist_p = pow(abs_scaled_x, p) + pow(abs_scaled_y, p);
            return pow(dist_p, p_inverse);
        }

        weighted_distance() = default;
    };

    inline double directional_weight(const vec2d& in, const vec2d& weights)
    {
        double atan_scale = M_2_PI * atan2(fabs(in.y), fabs(in.x));
        return atan_scale * (weights.y - weights.x) + weights.x;
    }

    /// <summary> Struct to hold variables and methods for modifying mouse input </summary>
    struct mouse_modifier {
        bool apply_rotate = false;
        bool apply_snap = false;
        bool apply_speed_clamp = false;
        bool by_component = false;
        bool apply_directional_weight = false;
        rotator rotate;
        snapper snap;
        double dpi_factor = 1;
        double speed_min = 0;
        double speed_max = 0;
        weighted_distance distance;
        vec2d range_weights = { 1, 1 };
        vec2<accel_variant> accels;
        vec2d sensitivity = { 1, 1 };
        vec2d directional_multipliers = {};

        mouse_modifier(const settings& args) :
            by_component(!args.combine_mags),
            dpi_factor(1000 / args.dpi),
            speed_min(args.speed_min),
            speed_max(args.speed_max),
            range_weights(args.range_weights)
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

            apply_speed_clamp = speed_max > 0;

            if ((!by_component && args.argsv.x.mode == accel_mode::noaccel) ||
                (args.argsv.x.mode == accel_mode::noaccel &&
                    args.argsv.y.mode == accel_mode::noaccel)) {
                return;
            }

            accels.x = accel_variant(args.argsv.x);
            accels.y = accel_variant(args.argsv.y);

            distance = weighted_distance(args.dom_args);

            apply_directional_weight = range_weights.x != range_weights.y;
        }

        void modify(vec2d& movement, milliseconds time) {
            apply_rotation(movement);
            apply_angle_snap(movement);
            apply_acceleration(movement, time);
            apply_sensitivity(movement);
        }

        inline void apply_rotation(vec2d& movement) {
            if (apply_rotate) movement = rotate.apply(movement);
        }

        inline void apply_angle_snap(vec2d& movement) {
            if (apply_snap) movement = snap.apply(movement);
        }

        inline void apply_acceleration(vec2d& movement, milliseconds time) {
            double norm = dpi_factor / time;

            if (apply_speed_clamp) {
                clamp_speed(movement, speed_min, speed_max, norm);
            }

            if (!by_component) {
                double mag = distance.calculate(movement.x, movement.y);
                double speed = mag * norm;

                double weight;

                if (apply_directional_weight) {
                    weight = directional_weight(movement, range_weights);
                }
                else {
                    weight = range_weights.x;
                }

                double scale = accels.x.apply(speed, weight);
                movement.x *= scale;
                movement.y *= scale;
            }
            else {
                if (movement.x != 0) {
                    double x = fabs(movement.x) * norm * distance.sigma_x;
                    movement.x *= accels.x.apply(x, range_weights.x);
                }
                if (movement.y != 0) {
                    double y = fabs(movement.y) * norm * distance.sigma_y;
                    movement.y *= accels.y.apply(y, range_weights.y);
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

    struct io_t {
        settings args;
        mouse_modifier mod;
    };

} // rawaccel

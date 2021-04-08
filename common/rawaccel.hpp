#pragma once

#include "accel-invoke.hpp"

namespace rawaccel {

    inline vec2d direction(double degrees)
    {
        double radians = degrees * PI / 180;
        return { cos(radians), sin(radians) };
    }

    constexpr vec2d rotate(const vec2d& v, const vec2d& direction)
    {
        return {
            v.x * direction.x - v.y * direction.y,
            v.x * direction.y + v.y * direction.x
        };
    }

    inline double magnitude(const vec2d& v)
    {
        return sqrt(v.x * v.x + v.y * v.y);
    }

    inline double lp_distance(const vec2d& v, double p)
    {
        return pow(pow(v.x, p) + pow(v.y, p), 1 / p);
    }

    class mouse_modifier {
    public:
        enum accel_distance_mode : unsigned char {
            separate, 
            max,
            Lp,
            euclidean,
        };

        bool apply_rotate = false;
        bool compute_ref_angle = false;
        bool apply_snap = false;
        bool cap_speed = false;
        accel_distance_mode dist_mode = euclidean;
        bool apply_directional_weight = false;
        bool apply_dir_mul_x = false;
        bool apply_dir_mul_y = false;

        vec2d rot_vec = { 1, 0 };
        double snap = 0;
        double dpi_norm_factor = 1;
        double speed_min = 0;
        double speed_max = 0;
        vec2d domain_weights = { 1, 1 };
        double p = 2;
        vec2d range_weights = { 1, 1 };
        vec2d directional_multipliers = { 1, 1 };
        vec2d sensitivity = { 1, 1 };
        vec2<accel_union> accel;

#ifdef _KERNEL_MODE
        __forceinline
#endif
        void modify(vec2d& in, const vec2<accel_invoker>& inv, milliseconds time = 1) const
        {
            double ips_factor = dpi_norm_factor / time;
            double reference_angle = 0;

            if (apply_rotate) in = rotate(in, rot_vec);

            if (compute_ref_angle && in.y != 0) {
                if (in.x == 0) {
                    reference_angle = PI / 2;
                }
                else {
                    reference_angle = atan(fabs(in.y / in.x));

                    if (apply_snap) {
                        if (reference_angle > PI / 2 - snap) {
                            reference_angle = PI / 2;
                            in = { 0, _copysign(magnitude(in), in.y) };
                        }
                        else if (reference_angle < snap) {
                            reference_angle = 0;
                            in = { _copysign(magnitude(in), in.x), 0 };
                        }
                    }
                }
            }

            if (cap_speed) {
                double speed = magnitude(in) * ips_factor;
                double ratio = clampsd(speed, speed_min, speed_max) / speed;
                in.x *= ratio;
                in.y *= ratio;
            }

            vec2d abs_weighted_vel = {
                fabs(in.x * ips_factor * domain_weights.x),
                fabs(in.y * ips_factor * domain_weights.y)
            };

            if (dist_mode == separate) {
                in.x *= inv.x.invoke(accel.x, abs_weighted_vel.x, range_weights.x);
                in.y *= inv.y.invoke(accel.y, abs_weighted_vel.y, range_weights.y);
            }
            else { 
                double speed;

                if (dist_mode == max) {
                    speed = maxsd(abs_weighted_vel.x, abs_weighted_vel.y);
                }
                else if (dist_mode == Lp) {
                    speed = lp_distance(abs_weighted_vel, p);
                }
                else {
                    speed = magnitude(abs_weighted_vel);
                }

                double weight = range_weights.x;

                if (apply_directional_weight) {
                    double diff = range_weights.y - range_weights.x;
                    weight += 2 / PI * reference_angle * diff;
                }

                double scale = inv.x.invoke(accel.x, speed, weight);
                in.x *= scale;
                in.y *= scale;
            }

            if (apply_dir_mul_x && in.x < 0) {
                in.x *= directional_multipliers.x;
            }

            if (apply_dir_mul_y && in.y < 0) {
                in.y *= directional_multipliers.y;
            }

            in.x *= sensitivity.x;
            in.y *= sensitivity.y;
        }

        mouse_modifier(const settings& args) :
            rot_vec(direction(args.degrees_rotation)),
            snap(args.degrees_snap * PI / 180),
            dpi_norm_factor(1000 / args.dpi),
            speed_min(args.speed_min),
            speed_max(args.speed_max),
            p(args.dom_args.lp_norm),
            domain_weights(args.dom_args.domain_weights),
            range_weights(args.range_weights),
            directional_multipliers(args.dir_multipliers),
            sensitivity(args.sens),
            accel({ { args.argsv.x }, { args.argsv.y } })
        {
            cap_speed = speed_max > 0 && speed_min <= speed_max;
            apply_rotate = rot_vec.x != 1;
            apply_snap = snap != 0;
            apply_directional_weight = range_weights.x != range_weights.y;
            compute_ref_angle = apply_snap || apply_directional_weight;
            apply_dir_mul_x = directional_multipliers.x != 1;
            apply_dir_mul_y = directional_multipliers.y != 1;

            if (!args.combine_mags) dist_mode = separate;
            else if (p >= MAX_NORM) dist_mode = max;
            else if (p > 2)         dist_mode = Lp;
            else                    dist_mode = euclidean;
        }

        mouse_modifier() = default;
    };

    struct io_t {
        settings args;
        mouse_modifier mod;
    };

} // rawaccel

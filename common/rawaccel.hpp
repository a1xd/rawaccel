#pragma once

#include "accel-union.hpp"

namespace rawaccel {

    struct time_clamp {
        milliseconds min = DEFAULT_TIME_MIN;
        milliseconds max = DEFAULT_TIME_MAX;
    };

    struct device_config {
        bool disable = false;
        bool set_extra_info = false;
        int dpi = 0;
        int polling_rate = 0;
        time_clamp clamp;
    };

    struct device_settings {
        wchar_t name[MAX_NAME_LEN] = {};
        wchar_t profile[MAX_NAME_LEN] = {};
        wchar_t id[MAX_DEV_ID_LEN] = {};
        device_config config;
    };

    enum class distance_mode : unsigned char {
        euclidean = 0,
        separate = 1,
        max = 2,
        Lp = 3,
    };

    struct modifier_flags {
        bool apply_rotate = 0;
        bool compute_ref_angle = 0;
        bool apply_snap = 0;
        bool clamp_speed = 0;
        bool apply_directional_weight = 0;
        bool apply_dir_mul_x = 0;
        bool apply_dir_mul_y = 0;

        modifier_flags(const profile& args) 
        {
            clamp_speed = args.speed_max > 0 && args.speed_min <= args.speed_max;
            apply_rotate = args.degrees_rotation != 0;
            apply_snap = args.degrees_snap != 0;
            apply_directional_weight = args.input_speed_args.whole && 
                args.range_weights.x != args.range_weights.y;
            compute_ref_angle = apply_snap || apply_directional_weight;
            apply_dir_mul_x = args.lr_sens_ratio != 1;
            apply_dir_mul_y = args.ud_sens_ratio != 1;
        }

        modifier_flags() = default;
    };

    struct input_speed_processor {

        input_speed_args speed_args = {};
        distance_mode dist_mode = {};

        const double trendHalflife = 1.25;

        double windowCoefficient = 0;
        double cutoffCoefficient = 0;
        double windowTrendCoefficient = 0;
        double cutoffTrendCoefficient = 0;

        void init(input_speed_args args)
        {
            speed_args = args;

            if (!args.whole) {
                dist_mode = distance_mode::separate;
            }
            else if (args.lp_norm >= MAX_NORM || args.lp_norm <= 0) {
                dist_mode = distance_mode::max;
            }
            else if (args.lp_norm != 2) {
                dist_mode = distance_mode::Lp;
            }
            else {
                dist_mode = distance_mode::euclidean;
            }

            windowCoefficient = args.smooth_halflife > 0 ? pow(0.5, 1 / args.smooth_halflife) : 0;
            windowTrendCoefficient = trendHalflife > 0 ? pow(0.5, 1 / trendHalflife) : 0;
            cutoffCoefficient = 1.0 - sqrt(1.0 - windowCoefficient);
            cutoffTrendCoefficient = 1.0 - sqrt(1.0 - windowTrendCoefficient);
        }

        double calc_speed(vec2d in, milliseconds time)
        {
			double speed;

			if (dist_mode == distance_mode::max) {
				speed = maxsd(in.x, in.y);
			}
			else if (dist_mode == distance_mode::Lp) {
				speed = lp_distance(in, speed_args.lp_norm);
			}
			else {
				speed = magnitude(in);
			}

            if (speed_args.should_smooth &&
                speed_args.smooth_halflife > 0)
            {
                if (speed_args.use_linear)
                {

					speed = smooth_speed_linear_ema(speed, time);
                }
                else
                {
					speed = smooth_speed_simple_ema(speed, time);
                }
            }

            return speed;
        }

        double windowTotal = 0;
        double cutoffTotal = 0;

        double smooth_speed_simple_ema(
            const double speed,
            const milliseconds time)
        {
            // compute coefficients
            double timeAdjustedWindowCoefficient = 1 - pow(windowCoefficient, time);
            double timeAdjustedCutoffCoefficient = 1 - pow(cutoffCoefficient, time);

            // adjust total based on coefficient and difference between new value and total
            windowTotal += timeAdjustedWindowCoefficient * (speed - windowTotal);
            cutoffTotal += timeAdjustedWindowCoefficient * (speed - cutoffTotal);

            return min(windowTotal, cutoffTotal);
        }

        double windowTrendTotal = 0;
        double cutoffTrendTotal = 0;

        const double trendDampening = 0.75;

        double smooth_speed_linear_ema(
            const double speed,
            const milliseconds time)
        {
            // compute coefficients
            double timeAdjustedWindowCoefficient = 1 - pow(windowCoefficient, time);
            double timeAdjustedCutoffCoefficient = 1 - pow(cutoffCoefficient, time);

            double timeAdjustedWindowTrendCoefficient = 1 - pow(windowTrendCoefficient, time);
            double timeAdjustedCutoffTrendCoefficient = 1 - pow(cutoffTrendCoefficient, time);

            // save old totals for trend adjustment
            double oldWindowTotal = windowTotal;
            double oldCutoffTotal = cutoffTotal;

            // dampen trends
            windowTrendTotal *= trendDampening;
            cutoffTrendTotal *= trendDampening;

            // add dampened trend
            windowTotal += windowTrendTotal * time;
            cutoffTotal += cutoffTrendTotal * time;

            // adjust total based on coefficient and difference between new value and total
            windowTotal += timeAdjustedWindowCoefficient * (speed - windowTotal);
            cutoffTotal += timeAdjustedCutoffCoefficient * (speed - cutoffTotal);

            // don't let trend carry us below 0
            windowTotal = max(windowTotal, 0.0);
            cutoffTotal = max(windowTotal, 0.0);

            // adjust trend based on coefficient and difference between new value and total
            double newWindowTrend = time > 0 ? (windowTotal - oldWindowTotal) / time : 0;
            double newCutoffTrend = time > 0 ? (cutoffTotal - oldCutoffTotal) / time : 0;
            windowTrendTotal += timeAdjustedWindowTrendCoefficient * (newWindowTrend - windowTrendTotal);
            cutoffTrendTotal += timeAdjustedCutoffTrendCoefficient * (newCutoffTrend - cutoffTrendTotal);

            return min(windowTotal, cutoffTotal);
        }
    };

    struct modifier_settings {
        profile prof;

        struct data_t {
            modifier_flags flags;
            vec2d rot_direction;
            accel_union accel_x;
            accel_union accel_y;
        } data = {};
    };

    inline void init_data(modifier_settings& settings)
    {
        auto set_accel = [](accel_union& u, const accel_args& args) {
            u.visit([&](auto& impl) {
                impl = { args };
            }, args);
        };

        set_accel(settings.data.accel_x, settings.prof.accel_x);
        set_accel(settings.data.accel_y, settings.prof.accel_y);

        settings.data.rot_direction = direction(settings.prof.degrees_rotation);

        settings.data.flags = modifier_flags(settings.prof);
    }

    struct io_base {
        device_config default_dev_cfg;
        unsigned modifier_data_size = 0;
        unsigned device_data_size = 0;
    };


    static_assert(alignof(io_base) == alignof(modifier_settings) && alignof(modifier_settings) == alignof(device_settings));

    class modifier {
    public:
#ifdef _KERNEL_MODE
        __forceinline
#endif
        void modify(vec2d& in, input_speed_processor& speed_processor, const modifier_settings& settings, double dpi_factor, milliseconds time) const
        {
            auto& args = settings.prof;
            auto& data = settings.data;
            auto& flags = settings.data.flags;

            double reference_angle = 0;
            double ips_factor = dpi_factor / time;

            if (flags.apply_rotate) in = rotate(in, data.rot_direction);

            if (flags.compute_ref_angle && in.y != 0) {
                if (in.x == 0) {
                    reference_angle = M_PI / 2;
                }
                else {
                    reference_angle = atan(fabs(in.y / in.x));

                    if (flags.apply_snap) {
                        double snap = args.degrees_snap * M_PI / 180;

                        if (reference_angle > M_PI / 2 - snap) {
                            reference_angle = M_PI / 2;
                            in = { 0, _copysign(magnitude(in), in.y) };
                        }
                        else if (reference_angle < snap) {
                            reference_angle = 0;
                            in = { _copysign(magnitude(in), in.x), 0 };
                        }
                    }
                }
            }

            if (flags.clamp_speed) {
                double speed = magnitude(in) * ips_factor;
                double ratio = clampsd(speed, args.speed_min, args.speed_max) / speed;
                in.x *= ratio;
                in.y *= ratio;
            }

            vec2d abs_weighted_vel = {
                fabs(in.x * ips_factor * args.domain_weights.x),
                fabs(in.y * ips_factor * args.domain_weights.y)
            };

            if (speed_processor.dist_mode == distance_mode::separate) {
                in.x *= (*cb_x)(data.accel_x, args.accel_x, abs_weighted_vel.x, args.range_weights.x);
                in.y *= (*cb_y)(data.accel_y, args.accel_y, abs_weighted_vel.y, args.range_weights.y);
            }
            else {
                double speed = speed_processor.calc_speed(abs_weighted_vel, time);

                double weight = args.range_weights.x;

                if (flags.apply_directional_weight) {
                    double diff = args.range_weights.y - args.range_weights.x;
                    weight += 2 / M_PI * reference_angle * diff;
                }

                double scale = (*cb_x)(data.accel_x, args.accel_x, speed, weight);
                in.x *= scale;
                in.y *= scale;
            }

            double dpi_adjusted_sens = args.sensitivity * dpi_factor;
            in.x *= dpi_adjusted_sens;
            in.y *= dpi_adjusted_sens * args.yx_sens_ratio;

            if (flags.apply_dir_mul_x && in.x < 0) {
                in.x *= args.lr_sens_ratio;
            }

            if (flags.apply_dir_mul_y && in.y < 0) {
                in.y *= args.ud_sens_ratio;
            }
        }

        modifier(modifier_settings& settings)
        {
            set_callback(cb_x, settings.data.accel_x, settings.prof.accel_x);
            set_callback(cb_y, settings.data.accel_y, settings.prof.accel_y);
        }

        modifier() = default;

    private:
        using callback_t = double (*)(const accel_union&, const accel_args&, double, double);

        void set_callback(callback_t& cb, accel_union& u, const accel_args& args)
        {
            u.visit([&](auto& impl) {
                cb = &callback_template<remove_ref_t<decltype(impl)>>;
            }, args);
        }

        template <typename AccelFunc>
        static double callback_template(const accel_union& u, 
                                        const accel_args& args, 
                                        double x, 
                                        double range_weight)
        {
            auto& accel_fn = reinterpret_cast<const AccelFunc&>(u);
            return 1 + (accel_fn(x, args) - 1) * range_weight;
        }

        callback_t cb_x = &callback_template<accel_noaccel>;
        callback_t cb_y = &callback_template<accel_noaccel>;
    };

} // rawaccel

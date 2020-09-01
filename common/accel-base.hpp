#pragma once

namespace rawaccel {
    
    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct accel_args {
        double offset = 0;
        double accel = 0;
        double limit = 2;
        double exponent = 2;
        double midpoint = 0;
        double power_scale = 1;
        double power_exp = 0.05;
        double weight = 1;
        double rate = 0;
        double scale_cap = 0;
        double gain_cap = 0;
    };

    template <typename Func>
    struct accel_val_base {
        double offset = 0;
        double weight = 1;
        Func fn;

        accel_val_base(const accel_args& args) : fn(args) {}

    };

    template <typename Func>
    struct additive_accel : accel_val_base<Func> {

        additive_accel(const accel_args& args) : accel_val_base(args) {
            offset = args.offset;
            weight = args.weight;
        }

        inline double operator()(double speed) const {
            return 1 + fn(maxsd(speed - offset, 0)) * weight;
        }

    };

    template <typename Func>
    struct nonadditive_accel : accel_val_base<Func> {

        nonadditive_accel(const accel_args& args) : accel_val_base(args) {
            if (args.weight != 0) weight = args.weight;
        }

        inline double operator()(double speed) const {
            return fn(speed) * weight;
        }

    };

}

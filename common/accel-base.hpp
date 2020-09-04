#pragma once

namespace rawaccel {
    
    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct accel_args {
        double offset = 0;
        double legacy_offset = 0;
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
        bool legacy_offset = false;
        double offset = 0;
        double weight = 1;
        Func fn;

        accel_val_base(const accel_args& args) : fn(args) {}

    };

    template <typename Func>
    struct additive_accel : accel_val_base<Func> {

        additive_accel(const accel_args& args) : accel_val_base(args) {
            legacy_offset = args.offset <= 0 && args.legacy_offset > 0;
            offset = legacy_offset ? args.legacy_offset : args.offset;
            weight = args.weight;
        }

        inline double operator()(double speed) const {
            double offset_speed = speed - offset;
            return offset_speed > 0 ? ( legacy_offset ? 1 + fn.legacy_offset(offset_speed) * weight : 1 + fn(offset_speed) ) : 1;
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

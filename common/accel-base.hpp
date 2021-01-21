#pragma once

namespace rawaccel {

    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct accel_args {
        double offset = 0;
        bool legacy_offset = false;
        double accel = 0;
        double scale = 1;
        double limit = 2;
        double exponent = 2;
        double midpoint = 10;
        double weight = 1;
        double scale_cap = 0;
        double gain_cap = 0;
        double speed_cap = 0;
    };

    struct domain_args {
        vec2d domain_weights = { 1, 1 };
        double lp_norm = 2;
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
            this->legacy_offset = args.legacy_offset;
            this->offset = args.offset;
            this->weight = args.weight;
        }

        inline double operator()(double speed) const {
            double offset_speed = speed - this->offset;
            if (offset_speed <= 0) return 1;
            if (this->legacy_offset) return 1 + this->fn.legacy_offset(offset_speed) * this->weight;
            return 1 + this->fn(offset_speed) * this->weight;
        }
    };

    template <typename Func>
    struct nonadditive_accel : accel_val_base<Func> {

        nonadditive_accel(const accel_args& args) : accel_val_base(args) {
            if (args.weight > 0) this->weight = args.weight;
        }

        inline double operator()(double speed) const {
            return this->fn(speed) * this->weight;
        }

    };

}

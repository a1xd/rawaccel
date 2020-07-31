#pragma once

#include "vec2.h"

namespace rawaccel {

    // Error throwing calls std libraries which are unavailable in kernel mode.
    void error(const char* s);

    using milliseconds = double;

    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct accel_args {
        double offset = 0;
        double accel = 0;
        double limit = 2;
        double exponent = 2;
        double midpoint = 0;
        double power_scale = 1;
        vec2d weight = { 1, 1 };
    };

    /// <summary>
    /// Struct to hold common acceleration curve implementation details.
    /// </summary>
    struct accel_base {

        /// <summary> Coefficients applied to acceleration per axis.</summary>
        vec2d weight = { 1, 1 };

        /// <summary> Generally, the acceleration ramp rate.
        double speed_coeff = 0;

        accel_base(const accel_args& args) {
            verify(args);

            speed_coeff = args.accel;
            weight = args.weight;
        }

        /// <summary> 
        /// Default transformation of speed -> acceleration.
        /// </summary>
        inline double accelerate(double speed) const { 
            return speed_coeff * speed; 
        }

        /// <summary> 
        /// Default transformation of acceleration -> mouse input multipliers.
        /// </summary>
        inline vec2d scale(double accel_val) const {
            return {
                weight.x * accel_val + 1,
                weight.y * accel_val + 1
            };
        }

        /// <summary>
        /// Verifies arguments as valid. Errors if not.
        /// </summary>
        /// <param name="args">Arguments to verified.</param>
        void verify(const accel_args& args) const {
            if (args.accel < 0) error("accel can not be negative, use a negative weight to compensate");
        }

        accel_base() = default;
    };

}

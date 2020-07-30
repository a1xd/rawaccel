#pragma once

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
    };

    /// <summary>
    /// Struct to hold common acceleration curve implementation details.
    /// </summary>
    struct accel_base {
        
        /// <summary> Generally, the acceleration ramp rate.</summary>
        double speed_coeff = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="accel_base"/> struct.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        accel_base(accel_args args) {
            verify(args);

            speed_coeff = args.accel;
        }

        /// <summary>
        /// Verifies arguments as valid. Errors if not.
        /// </summary>
        /// <param name="args">Arguments to verified.</param>
        void verify(accel_args args) const {
            if (args.accel < 0) error("accel can not be negative, use a negative weight to compensate");
        }

        accel_base() = default;
    };

}

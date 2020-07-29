#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

namespace rawaccel {

// Error throwing calls std libraries which are unavailable in kernel mode.
#ifdef _KERNEL_MODE
    inline void error(const char*) {}
#else
    void error(const char* s);
#endif

    using milliseconds = double;

    /// <summary> Struct to hold arguments for an acceleration function. </summary>
    struct accel_args {
        milliseconds time_min = 0.4;
        double offset = 0;
        double accel = 0;
        double lim_exp = 2;
        double midpoint = 0;
    };

    /// <summary>
    /// Struct to hold acceleration curve implementation details.
    /// </summary>
    /// <typeparam name="T">Type of acceleration.</typeparam>
    template <typename T>
    struct accel_implentation {
        
        /// <summary> First constant for use in acceleration curves. Generally, the acceleration ramp rate.</summary>
        double curve_constant_one = 0;

        /// <summary> Second constant for use in acceleration curves. Generally, the limit or exponent in the curve. </summary>
        double curve_constant_two = 0;

        /// <summary> Third constant for use in acceleration curves. The midpoint in sigmoid mode. </summary>
        double curve_constant_three = 0;

        /// <summary> The offset past which acceleration is applied. Used in power mode. </summary>
        double offset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="accel_implementation{T}"/> struct.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        accel_implentation(accel_args args)
        {
            curve_constant_one = args.accel;
            curve_constant_two = args.lim_exp - 1;
            curve_constant_three = args.midpoint;
            offset = args.offset;
        }

        /// <summary>
        /// Returns accelerated value of speed as a ratio of magnitude.
        /// </summary>
        /// <param name="speed">Mouse speed at which to calculate acceleration.</param>
        /// <returns>Ratio of accelerated movement magnitude to input movement magnitude.</returns>
        double accelerate(double speed) { return 0; }

        /// <summary>
        /// Verifies arguments as valid. Errors if not.
        /// </summary>
        /// <param name="args">Arguments to verified.</param>
        void verify(accel_args args) {
            if (args.accel < 0) error("accel can not be negative, use a negative weight to compensate");
            if (args.time_min <= 0) error("min time must be positive");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        accel_implentation() = default;
    };

    /// <summary> Struct to hold linear acceleration implementation. </summary>
    struct accel_linear : accel_implentation<accel_linear> {
        accel_linear(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
    struct accel_classic : accel_implentation<accel_classic> {
        accel_classic(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold "natural" (vanishing difference) acceleration implementation. </summary>
    struct accel_natural : accel_implentation<accel_natural> {
        accel_natural(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold logarithmic acceleration implementation. </summary>
    struct accel_logarithmic : accel_implentation<accel_logarithmic> {
        accel_logarithmic(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold sigmoid (s-shaped) acceleration implementation. </summary>
    struct accel_sigmoid : accel_implentation<accel_sigmoid> {
        accel_sigmoid(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold power (non-additive) acceleration implementation. </summary>
    struct accel_power : accel_implentation<accel_power> {
        accel_power(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

    /// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
    struct accel_noaccel : accel_implentation<accel_noaccel> {
        accel_noaccel(accel_args args);
        double accelerate(double speed);
        void verify(accel_args args);
    };

}

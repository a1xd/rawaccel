#pragma once

#include "rawaccel-base.hpp"
#include "utility.hpp"

#include <float.h>

namespace rawaccel {

    /// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
    struct classic_base {
        double base_fn(double x, double accel_raised, const accel_args& args) const
        {
            return accel_raised * pow(x - args.input_offset, args.exponent_classic) / x;
        }

        static double base_accel(double x, double y, const accel_args& args)
        {
            auto power = args.exponent_classic;
            return pow(x * y * pow(x - args.input_offset, -power), 1 / (power - 1));
        }
    };

    template <bool Gain> struct classic;

    template<>
    struct classic<LEGACY> : classic_base {
        double accel_raised;
        double cap = DBL_MAX;
        double sign = 1;

        classic(const accel_args& args)
        {
            switch (args.cap_mode) {
            case cap_mode::io:
                cap = args.cap.y - 1;

                if (cap < 0) {
                    cap = -cap;
                    sign = -sign;
                }

                {
                    double a = base_accel(args.cap.x, cap, args);
                    accel_raised = pow(a, args.exponent_classic - 1);
                }
                break;
            case cap_mode::in:
                accel_raised = pow(args.acceleration, args.exponent_classic - 1);
                if (args.cap.x > 0) {
                    cap = base_fn(args.cap.x, accel_raised, args);
                }
                break;
            case cap_mode::out:
            default:
                accel_raised = pow(args.acceleration, args.exponent_classic - 1);

                if (args.cap.y > 0) {
                    cap = args.cap.y - 1;

                    if (cap < 0) {
                        cap = -cap;
                        sign = -sign;
                    }
                }

                break;
            }
        }

        double operator()(double x, const accel_args& args) const
        {
            if (x <= args.input_offset) return 1;
            return sign * minsd(base_fn(x, accel_raised, args), cap) + 1;
        }

    };

    template<>
    struct classic<GAIN> : classic_base {
        double accel_raised;
        vec2d cap = { DBL_MAX, DBL_MAX };
        double constant = 0;
        double sign = 1;

        classic(const accel_args& args)
        {
            switch (args.cap_mode) {
            case cap_mode::io:
                cap.x = args.cap.x;
                cap.y = args.cap.y - 1;

                if (cap.y < 0) {
                    cap.y = -cap.y;
                    sign = -sign;
                }

                {
                    double a = gain_accel(cap.x, cap.y, args.exponent_classic, args.input_offset);
                    accel_raised = pow(a, args.exponent_classic - 1);
                }
                constant = (base_fn(cap.x, accel_raised, args) - cap.y) * cap.x;
                break;
            case cap_mode::in:
                accel_raised = pow(args.acceleration, args.exponent_classic - 1);
                if (args.cap.x > 0) {
                    cap.x = args.cap.x;
                    cap.y = gain(cap.x, args.acceleration, args.exponent_classic, args.input_offset);
                    constant = (base_fn(cap.x, accel_raised, args) - cap.y) * cap.x;
                }
                break;
            case cap_mode::out:
            default:
                accel_raised = pow(args.acceleration, args.exponent_classic - 1);

                if (args.cap.y > 0) {
                    cap.y = args.cap.y - 1;

                    if (cap.y == 0) {
                        cap.x = 0;
                    }
                    else {
                        if (cap.y < 0) {
                            cap.y = -cap.y;
                            sign = -sign;
                        }

                        cap.x = gain_inverse(cap.y, 
                                            args.acceleration, 
                                            args.exponent_classic, 
                                            args.input_offset);
                        constant = (base_fn(cap.x, accel_raised, args) - cap.y) * cap.x;
                    }
                }
                break;
            }
        }

        double operator()(double x, const accel_args& args) const
        {
            double output;

            if (x <= args.input_offset) return 1;

            if (x < cap.x) {
                output = base_fn(x, accel_raised, args);
            }
            else {
                output = constant / x + cap.y;
            }

            return sign * output + 1;
        }

        static double gain(double x, double accel, double power, double offset)
        {
            return power * pow(accel * (x - offset), power - 1);
        }

        static double gain_inverse(double y, double accel, double power, double offset)
        {
            return (accel * offset + pow(y / power, 1 / (power - 1))) / accel;
        }

        static double gain_accel(double x, double y, double power, double offset)
        {
            return -pow(y / power, 1 / (power - 1)) / (offset - x);
        }

    };

}

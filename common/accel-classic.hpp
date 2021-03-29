#pragma once

#include "rawaccel-base.hpp"
#include "utility.hpp"

#include <math.h>
#include <float.h>

namespace rawaccel {

    /// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
    struct classic_base {
        double offset;
        double power;
        double accel_raised;

        classic_base(const accel_args& args) :
            offset(args.offset),
            power(args.power),
            accel_raised(pow(args.accel_classic, power - 1)) {}

        double base_fn(double x) const
        {
            return accel_raised * pow(x - offset, power) / x;
        }
    };

    struct classic_legacy : classic_base {
        double sens_cap = DBL_MAX;
        double sign = 1;

        classic_legacy(const accel_args& args) :
            classic_base(args) 
        {
            if (args.cap > 0) {
                sens_cap = args.cap - 1;

                if (sens_cap < 0) {
                    sens_cap = -sens_cap;
                    sign = -sign;
                }
            }
        }

        double operator()(double x) const 
        {
            if (x <= offset) return 1;
            return sign * minsd(base_fn(x), sens_cap) + 1;
        }   
    };

    struct classic : classic_base {
        vec2d gain_cap = { DBL_MAX, DBL_MAX };
        double constant = 0;
        double sign = 1;

        classic(const accel_args& args) :
            classic_base(args) 
        {
            if (args.cap > 0) {
                gain_cap.y = args.cap - 1;

                if (gain_cap.y < 0) {
                    gain_cap.y = -gain_cap.y;
                    sign = -sign;
                }

                gain_cap.x = gain_inverse(gain_cap.y, args.accel_classic, power, offset);
                constant = (base_fn(gain_cap.x) - gain_cap.y) * gain_cap.x;
            }
        }

        double operator()(double x) const 
        {
            double output;

            if (x <= offset) return 1;

            if (x < gain_cap.x) {
                output = base_fn(x);
            }
            else {
                output = constant / x + gain_cap.y;
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

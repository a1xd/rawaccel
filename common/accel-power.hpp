#pragma once

#include "rawaccel-base.hpp"

#include <math.h>
#include <float.h>

namespace rawaccel {

	struct power_base {
		static double base_fn(double x, const accel_args& args)
		{
			// f(x) = w(mx)^k
			return args.weight * pow(args.scale * x, args.exponent_power);
		}
	};

	template <bool Gain> struct power;

	template <>
	struct power<LEGACY> : power_base {
		vec2d cap = { DBL_MAX, DBL_MAX };

		power(const accel_args& args)
		{
			if (args.cap.x > 0) {
				cap.x = args.cap.x;
				cap.y = base_fn(cap.x, args);
			}
		}

		double operator()(double speed, const accel_args& args) const
		{
			if (speed < cap.x) {
				return base_fn(speed, args);
			}
			return cap.y;
		}

	};

	template <>
	struct power<GAIN> : power_base {
		vec2d cap = { DBL_MAX, DBL_MAX };
		double constant = 0;

		power(const accel_args& args) 
		{
			if (args.cap.x > 0) {
				cap.x = args.cap.x;
				double output = base_fn(cap.x, args);
				cap.y = output * (args.exponent_power + 1);
				constant = -args.exponent_power * output * args.cap.x;
			}
		}

		double operator()(double speed, const accel_args& args) const
		{
			if (speed < cap.x) {
				return base_fn(speed, args);
			}
			else {
				return cap.y + constant / speed;
			}
		}

	};

}

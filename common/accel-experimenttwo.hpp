#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct experimenttwo_impl {
		double rate;
		double limit;
		double midpoint;
		double subtractive_constant;

		experimenttwo_impl(const accel_args& args) :
			rate(pow(10,args.rate)), limit(2*log10(args.limit)), midpoint(log10(args.midpoint))
		{
			subtractive_constant = limit / 2;
		}

		inline double operator()(double speed) const {
			double log_speed = log10(speed);
			return pow(10, limit / (exp(-rate * (log_speed - midpoint)) + 1) - subtractive_constant);

		}

		inline double legacy_offset(double speed) const { return operator()(speed); }

		inline double apply(double* lookup, double speed)
		{
			int index = map(speed);
			double slope = lookup[index];
			double intercept = lookup[index + 1];
			return slope + intercept / speed;
		}

		inline int map(double speed)
		{
			return speed > 0 ? (int)floor(200*log10(speed)+402) : 0;
		}

		inline double fill(double* lookup)
		{
			double lookup_speed = 0;
			double gain_integral_speed = 0;
			double gain = 0;
			double intercept = 0;
			double output = 0;
			int index = 0;

			lookup[index] = gain;
			lookup[index + 1] = intercept;

			for (double x = -2.0; x <= 4.0; x += 0.01)
			{
				index+=2;
				lookup_speed = pow(10,x);

				while (gain_integral_speed < lookup_speed)
				{
					gain_integral_speed += 0.001;
					gain = operator()(gain_integral_speed);
					output += gain*0.001;
				}

				intercept = gain * lookup_speed - output;
				lookup[index] = gain;
				lookup[index + 1] = intercept;
			}

		}
	};

	using accel_experimentone = nonadditive_accel<experimenttwo_impl>;

}

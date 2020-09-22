#pragma once

#include <math.h>

#include "accel-base.hpp"

#define RA_LOOKUP

namespace rawaccel {

	constexpr size_t LUT_SIZE = 601;

	struct si_pair { 
		double slope = 0;
		double intercept = 0; 
	};

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

		inline double apply(si_pair* lookup, double speed) const
		{
			si_pair pair = lookup[map(speed)];
			return pair.slope + pair.intercept / speed;
		}

		inline int map(double speed) const
		{
			int index = speed > 0 ? (int)(100 * log10(speed) + 201) : 0;

			if (index < 0) return 0;
			if (index >= LUT_SIZE) return LUT_SIZE - 1;

			return index;
		}

		inline void fill(si_pair* lookup) const
		{
			double lookup_speed = 0;
			double gain_integral_speed = 0;
			double gain = 0;
			double intercept = 0;
			double output = 0;
			double x = -2;

			lookup[0] = {};

			for (size_t i = 1; i < LUT_SIZE; i++)
			{
				x += 0.01;

				lookup_speed = pow(10, x);

				while (gain_integral_speed < lookup_speed)
				{
					gain_integral_speed += 0.001;
					gain = operator()(gain_integral_speed);
					output += gain * 0.001;
				}

				intercept = output - gain * lookup_speed;

				lookup[i] = { gain, intercept };
			}

		}
	};

	using accel_experimentone = nonadditive_accel<experimenttwo_impl>;

}

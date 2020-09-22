#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct experimentone_impl {
		double rate;
		double limit;
		double midpoint;
		double subtractive_constant;

		experimentone_impl(const accel_args& args) :
			rate(pow(10,args.rate)), limit(2*log10(args.limit)), midpoint(log10(args.midpoint))
		{
			subtractive_constant = limit / 2;
		}

		inline double operator()(double speed) const {
			double log_speed = log10(speed);
			return pow(10, limit / (exp(-rate * (log_speed - midpoint)) + 1) - subtractive_constant);

		}

		inline double legacy_offset(double speed) const { return operator()(speed); }
	};

	using accel_experimentone = nonadditive_accel<experimentone_impl>;

}

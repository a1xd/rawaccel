#pragma once

#include "rawaccel-base.hpp"

#include <math.h>

namespace rawaccel {

	/// <summary> Struct to hold power (non-additive) acceleration implementation. </summary>
	struct power {
		double pre_scale;
		double exponent;
		double post_scale;

		power(const accel_args& args) :
			pre_scale(args.scale), 
			exponent(args.exponent),
			post_scale(args.weight) {}

		double operator()(double speed) const 
		{
			// f(x) = (mx)^k
			return post_scale * pow(speed * pre_scale, exponent);
		}
	};

}

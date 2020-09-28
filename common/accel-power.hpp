#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold power (non-additive) acceleration implementation. </summary>
	struct power_impl {
		double scale;
		double exponent;

		power_impl(const accel_args& args) :
			scale(args.scale), exponent(args.exponent)
		{}

		inline double operator()(double speed) const {
			// f(x) = (mx)^k
			return pow(speed * scale, exponent);
		}
	};

	using accel_power = nonadditive_accel<power_impl>;

}

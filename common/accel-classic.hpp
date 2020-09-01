#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
	struct classic_impl {
		double accel;
		double power;

		classic_impl(const accel_args& args) :
			accel(args.accel), power(args.exponent - 1)
		{}

		inline double operator()(double speed) const {
			//f(x) = (mx)^(k-1)
			return pow(accel * speed, power);
		}
	};

	using accel_classic = additive_accel<classic_impl>;
	
}

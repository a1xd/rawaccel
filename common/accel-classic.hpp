#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
	struct classic_impl {
		double accel;
		double power;
		double power_inc;
		double offset;
		double multiplicative_const;

		classic_impl(const accel_args& args) :
			accel(args.accel), power(args.exponent - 1), offset(args.offset) {
			multiplicative_const = pow(accel, power);
			power_inc = power + 1;
		}

		inline double operator()(double speed) const {
			//f(x) = (mx)^(k-1)
			double base_speed = speed + offset;
			return multiplicative_const * pow(speed, power_inc) / base_speed;
		}

		inline double legacy_offset(double speed) const {
			return pow(accel * speed, power);
		}
	};

	using accel_classic = additive_accel<classic_impl>;
	
}

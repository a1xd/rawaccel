#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	inline accel_classic::accel_classic(accel_args args)
		: accel_implentation(args) {}

	inline double accel_classic::accelerate(double speed) {
		//f(x) = (mx)^k
		return pow(curve_constant_one * speed, curve_constant_two);
	}

	inline void accel_classic::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
}

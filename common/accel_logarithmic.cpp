#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	inline accel_logarithmic::accel_logarithmic(accel_args args)
		: accel_implentation(args) {}

	inline double accel_logarithmic::accelerate(double speed) {
		//f(x) = log(m*x+1)
		return log(speed * curve_constant_one + 1);
	}

	inline void accel_logarithmic::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
}

#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	inline accel_linear::accel_linear(accel_args args)
		: accel_implentation(args) {}

	inline double accel_linear::accelerate(double speed) {
		//f(x) = mx
		return curve_constant_one * speed;
	}

	inline void accel_linear::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("limit must be greater than 1");
	}
}
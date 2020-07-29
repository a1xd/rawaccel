#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	inline accel_sigmoid::accel_sigmoid(accel_args args)
		: accel_implentation(args) {}

	inline double accel_sigmoid::accelerate(double speed) {
		//f(x) = k/(1+e^(-m(c-x)))
		return curve_constant_two / (exp(-curve_constant_one * (speed - curve_constant_three)) + 1);
	}

	inline void accel_sigmoid::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
}

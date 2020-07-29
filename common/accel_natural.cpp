
#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	inline accel_natural::accel_natural(accel_args args)
		: accel_implentation(args)
        { curve_constant_one /= curve_constant_two; }

	inline double accel_natural::accelerate(double speed) {
		// f(x) = k(1-e^(-mx))
		return curve_constant_two - (curve_constant_two * exp(-curve_constant_one * speed));;
	}

	inline void accel_natural::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
}

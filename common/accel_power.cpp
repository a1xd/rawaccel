#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	accel_power::accel_power(accel_args args)
		: accel_implentation(args)
        { curve_constant_two++; }

	double accel_power::accelerate(double speed) {
		// f(x) = (mx)^k - 1
		// The subtraction of 1 occurs with later addition of 1 in mind, 
		// so that the input vector is directly multiplied by (mx)^k (if unweighted)
		return (offset > 0 && speed < 1) ? 0 : pow(speed * curve_constant_one, curve_constant_two) - 1;
	}

	void accel_power::verify(accel_args args) {
		accel_implentation::verify(args);
		if (args.lim_exp <= 0) error("exponent must be greater than 0");
	}
}

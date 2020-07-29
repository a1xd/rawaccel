#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "accel_types.hpp"

namespace rawaccel {
	accel_noaccel::accel_noaccel(accel_args args)
		: accel_implentation(args) {}

	double accel_noaccel::accelerate(double speed) { return 0; }

	void accel_noaccel::verify(accel_args args) { }
}

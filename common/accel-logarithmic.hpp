#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold logarithmic acceleration implementation. </summary>
	struct accel_logarithmic : accel_base {

		accel_logarithmic(accel_args args) : accel_base(args) {}

		inline double accelerate(double speed) const {
			//f(x) = log(m*x+1)
			return log(speed_coeff * speed + 1);
		}
	};

}

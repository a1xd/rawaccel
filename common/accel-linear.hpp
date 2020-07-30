#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold linear acceleration implementation. </summary>
	struct accel_linear : accel_base {

		accel_linear(accel_args args) : accel_base(args) {}

		inline double accelerate(double speed) const {
			//f(x) = mx
			return speed_coeff * speed;
		}
	};

}

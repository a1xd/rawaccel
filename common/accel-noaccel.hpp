#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
	struct accel_noaccel : accel_base {

		accel_noaccel(accel_args args) : accel_base(args) {}

		inline double accelerate(double) const { return 0; }
	};

}

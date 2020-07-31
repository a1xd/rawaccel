#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
	struct accel_noaccel : accel_base {

		accel_noaccel(const accel_args&) : accel_base() {}

	};

}

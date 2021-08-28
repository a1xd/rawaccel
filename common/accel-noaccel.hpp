#pragma once

#include "rawaccel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
	struct accel_noaccel {

		accel_noaccel(const accel_args&) {}
		accel_noaccel() = default;

		double operator()(double, const accel_args&) const { return 1; }
	};

}

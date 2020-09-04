#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold acceleration implementation which applies no acceleration. </summary>
	struct accel_noaccel {

		accel_noaccel(const accel_args&) {}
		accel_noaccel() = default;

		inline double operator()(double) const { return 1; }
		
		inline double legacy_offset(double speed) const { return operator()(speed); }
	};

}

#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold linear acceleration implementation. </summary>
	struct linear_impl { 
		double accel;
		
		linear_impl(const accel_args& args) : accel(args.accel) {}

		inline double operator()(double speed) const {
			return accel * speed;
		}

	};

	using accel_linear = additive_accel<linear_impl>;

}

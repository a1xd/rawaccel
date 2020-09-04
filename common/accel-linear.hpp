#pragma once

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold linear acceleration implementation. </summary>
	struct linear_impl { 
		double accel;
		double offset;
		double subtractive_const;
		double divisive_const;
		
		linear_impl(const accel_args& args) : accel(args.accel), offset(args.offset) {
			subtractive_const = 2 * accel * offset;
			divisive_const = accel * offset * offset;
		}

		inline double operator()(double speed) const {
			double base_speed = speed + offset;
			return accel * base_speed - subtractive_const + divisive_const / base_speed;
		}

		inline double legacy_offset(double speed) const {
			return accel * speed;
		}
	};

	using accel_linear = additive_accel<linear_impl>;

}

#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold logarithmic acceleration implementation. </summary>
	struct logarithmic_impl {
		double accel;

		logarithmic_impl(const accel_args& args) : accel(args.accel) {}

		inline double operator()(double speed) const {
			//f(x) = log(m*x+1)
			return log(accel * speed + 1);
		}
	};

	using accel_logarithmic = additive_accel<logarithmic_impl>;
}

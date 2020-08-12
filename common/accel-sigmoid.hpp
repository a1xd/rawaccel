#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) acceleration implementation. </summary>
	struct accel_sigmoid : accel_base {
		double limit = 1;
		double midpoint = 0;

		accel_sigmoid(const accel_args& args) : accel_base(args) {
			verify(args);

			limit = args.limit - 1;
			midpoint = args.midpoint;
		}

		inline double accelerate(double speed) const {
			//f(x) = k/(1+e^(-m(c-x)))
			return limit / (exp(-speed_coeff * (speed - midpoint)) + 1);
		}

		void verify(const accel_args& args) const {
			if (args.limit <= 1) bad_arg("exponent must be greater than 1");
			if (args.midpoint < 0) bad_arg("midpoint must not be negative");
		}
	};

}

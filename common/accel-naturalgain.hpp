#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "natural" (vanishing difference) gain implementation. </summary>
	struct accel_naturalgain : accel_base {
		double limit = 1;
		double midpoint = 0;

		accel_naturalgain(const accel_args& args) : accel_base(args) {
			verify(args);

			limit = args.limit - 1;
			speed_coeff /= limit;
		}

		inline double accelerate(double speed) const {
			// f(x) = k((e^(-mx)-1)/mx + 1)
			double scaled_speed = speed_coeff * speed;
			return limit * (((exp(-scaled_speed) - 1) / scaled_speed) + 1);
		}

		void verify(const accel_args& args) const {
			if (args.limit <= 1) bad_arg("limit must be greater than 1");
		}
	};

}

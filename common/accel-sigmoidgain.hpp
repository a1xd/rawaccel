#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct accel_sigmoidgain : accel_base {
		double limit = 1;
		double midpoint = 0;
		double additive_constant = 0;
		double integration_constant = 0;

		accel_sigmoidgain(const accel_args& args) : accel_base(args) {
			verify(args);

			limit = args.limit - 1;
			midpoint = args.midpoint;
			additive_constant = exp(speed_coeff*midpoint);
			integration_constant = log(1 + additive_constant);
		}

		inline double accelerate(double speed) const {
			//f(x) = k/(1+e^(-m(c-x)))
			double scaled_speed = speed_coeff * speed;
			return limit * ((log(additive_constant+exp(scaled_speed)) - integration_constant)/scaled_speed);
		}

		void verify(const accel_args& args) const {
			if (args.limit <= 1) bad_arg("exponent must be greater than 1");
			if (args.midpoint < 0) bad_arg("midpoint must not be negative");
		}
	};

}

#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold power (non-additive) acceleration implementation. </summary>
	struct accel_power : accel_base {
		double exponent;
		double offset;

		accel_power(accel_args args) {
			verify(args);

			speed_coeff = args.power_scale;
			exponent = args.exponent;
			offset = args.offset;
		}

		inline double accelerate(double speed) const {
			// f(x) = (mx)^k
			return (offset > 0 && speed < 1) ? 1 : pow(speed * speed_coeff, exponent);
		}

		void verify(accel_args args) const {
			if (args.power_scale < 0) error("scale can not be negative");
			if (args.exponent <= 0) error("exponent must be greater than 0");
		}
	};

}

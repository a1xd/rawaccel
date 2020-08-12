#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "classic" (linear raised to power) acceleration implementation. </summary>
	struct accel_classic : accel_base {
		double exponent;

		accel_classic(const accel_args& args) : accel_base(args) {
			verify(args);

			exponent = args.exponent - 1;
		}

		inline double accelerate(double speed) const {
			//f(x) = (mx)^k
			return pow(speed_coeff * speed, exponent);
		}

		void verify(const accel_args& args) const {
			if (args.exponent <= 1) bad_arg("exponent must be greater than 1");
		}
	};
	
}

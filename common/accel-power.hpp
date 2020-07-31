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

			weight = args.weight;
			speed_coeff = args.power_scale;
			exponent = args.exponent;
			offset = args.offset;
		}

		inline double accelerate(double speed) const {
			// f(x) = (mx)^k
			return (offset > 0 && speed < 1) ? 1 : pow(speed * speed_coeff, exponent);
		}

		inline vec2d scale(double accel_val) const {
			return { 
				weight.x * accel_val,
				weight.y * accel_val
			};
		}

		void verify(accel_args args) const {
			if (args.power_scale <= 0) error("scale must be positive");
			if (args.exponent <= 0) error("exponent must be greater than 0");
		}
	};

}

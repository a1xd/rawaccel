#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct logarithm_impl {
		double rate;
		double offset;
		double additive_const;

		logarithm_impl (const accel_args& args) :
			rate(args.rate), offset (args.offset) {
			additive_const = offset * rate;
		}

		inline double operator()(double speed) const {
			double scaled_speed = rate * speed + 1;
			double base_speed = speed + offset;

			return (scaled_speed * log(scaled_speed) + additive_const ) / ( rate * base_speed) - 1;
		}

		inline double legacy_offset(double speed) const { return operator()(speed); }
	};

	using accel_logarithm = additive_accel<logarithm_impl>;

}

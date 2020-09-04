#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "natural" (vanishing difference) acceleration implementation. </summary>
	struct natural_impl {
		double rate;
		double limit;
		double offset;

		natural_impl(const accel_args& args) :
			rate(args.accel), limit(args.limit - 1), offset(args.offset)
		{
			rate /= limit;
		}

		inline double operator()(double speed) const {
			// f(x) = k(1-e^(-mx))
			double base_speed = speed + offset;
			return limit * (1 - ((exp(-rate * speed) * speed + offset) / base_speed));
		}

		inline double legacy_offset(double speed) const {
			return limit - (limit * exp(-rate * speed));
		}

	};

	using accel_natural = additive_accel<natural_impl>;

}

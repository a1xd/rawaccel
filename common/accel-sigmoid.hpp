#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) acceleration implementation. </summary>
	struct sigmoid_impl {
		double rate;
		double limit;
		double midpoint;

		sigmoid_impl(const accel_args& args) :
			rate(args.accel), limit(args.limit - 1), midpoint(args.midpoint)
		{}

		inline double operator()(double speed) const {
			//f(x) = k/(1+e^(-m(x-c)))
			return limit / (exp(-rate * (speed - midpoint)) + 1);
		}

	};

	using accel_sigmoid = additive_accel<sigmoid_impl>;

}

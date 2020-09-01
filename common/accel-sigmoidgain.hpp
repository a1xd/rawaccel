#pragma once

#include <math.h>

#include "accel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct sigmoidgain_impl {
		double rate;
		double limit;
		double additive_constant;
		double integration_constant;

		sigmoidgain_impl(const accel_args& args) :
			rate(args.rate), limit(args.limit - 1)
		{
			additive_constant = exp(rate * args.midpoint);
			integration_constant = log(1 + additive_constant);
		}

		inline double operator()(double speed) const {
			//f(x) = k/(1+e^(-m(c-x)))
			double scaled_speed = rate * speed;
			return limit * ((log(additive_constant+exp(scaled_speed)) - integration_constant)/scaled_speed);
		}

	};

	using accel_sigmoidgain = additive_accel<sigmoidgain_impl>;

}

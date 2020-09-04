#pragma once

#include <math.h>

#include "accel-natural.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "natural" (vanishing difference) gain implementation. </summary>
	struct naturalgain_impl : natural_impl {

		using natural_impl::natural_impl;

		inline double operator()(double speed) const {
			// f(x) = k((e^(-mx)-1)/mx + 1)
			double base_speed = speed + offset;
			double scaled_speed = rate * speed;
			return limit * (((exp(-scaled_speed) - 1) / (base_speed * rate) ) + 1 - offset / base_speed);
		}
		
		inline double legacy_offset(double speed) const {
			double scaled_speed = rate * speed;
			return limit * (((exp(-scaled_speed) - 1) / scaled_speed) + 1);
		}
	};

	using accel_naturalgain = additive_accel<naturalgain_impl>;
}

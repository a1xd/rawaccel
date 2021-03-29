#pragma once

#include "rawaccel-base.hpp"

#define _USE_MATH_DEFINES
#include <math.h>

namespace rawaccel {

	struct jump_base {
		vec2d step;
		double smooth_rate;

		jump_base(const accel_args& args) :
			step({ args.offset, args.cap - 1 })
		{
			if (args.smooth == 0 || args.offset == 0) {
				smooth_rate = 0;
			}
			else {
				smooth_rate = 2 * M_PI / (args.offset * args.smooth);
			}

		}

		bool is_smooth() const
		{
			return smooth_rate != 0;
		}

		double decay(double x) const
		{
			return exp(smooth_rate * (step.x - x));
		}

		double smooth(double x) const
		{
			return step.y * 1 / (1 + decay(x));
		}

		double smooth_antideriv(double x) const
		{
			return step.y * (x + log(1 + decay(x)) / smooth_rate);
		}
	};

	struct jump_legacy : jump_base {
		using jump_base::jump_base;

		double operator()(double x) const
		{
			if (is_smooth()) return smooth(x) + 1;
			else if (x < step.x) return 1;
			else return step.y;
		}
	};

	struct jump : jump_base {
		double C;

		jump(const accel_args& args) :
			jump_base(args),
			C(-smooth_antideriv(0)) {}

		double operator()(double x) const
		{
			if (is_smooth()) return 1 + (smooth_antideriv(x) + C) / x;
			else if (x < step.x) return 1;
			else return 1 + step.y * (x - step.x) / x;
		}
	};

}

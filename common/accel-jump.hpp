#pragma once

#include "rawaccel-base.hpp"

namespace rawaccel {

	struct jump_base {
		static constexpr double smooth_scale = 2 * M_PI;

		vec2d step;
		double smooth_rate;

		// requirements: args.smooth in range [0, 1]
		jump_base(const accel_args& args) :
			step({ args.cap.x, args.cap.y - 1 })
		{
			double rate_inverse = args.smooth * step.x;

			if (rate_inverse < 1) {
				smooth_rate = 0;
			}
			else {
				smooth_rate = smooth_scale / rate_inverse;
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
			return step.y / (1 + decay(x));
		}

		double smooth_antideriv(double x) const
		{
			return step.y * (x + log(1 + decay(x)) / smooth_rate);
		}

	};

	template <bool Gain> struct jump;

	template<>
	struct jump<LEGACY> : jump_base {
		using jump_base::jump_base;

		double operator()(double x, const accel_args&) const
		{
			if (is_smooth()) return smooth(x) + 1;
			else if (x < step.x) return 1;
			else return 1 + step.y;
		}
	};

	template<>
	struct jump<GAIN> : jump_base {
		double C;

		jump(const accel_args& args) :
			jump_base(args),
			C(-smooth_antideriv(0)) {}

		double operator()(double x, const accel_args&) const
		{
			if (x <= 0) return 1;

			if (is_smooth()) return 1 + (smooth_antideriv(x) + C) / x;

			if (x < step.x) return 1;
			else return 1 + step.y * (x - step.x) / x;
		}

	};

}

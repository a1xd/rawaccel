#pragma once

#include "rawaccel-base.hpp"

namespace rawaccel {

	/// <summary> Struct to hold "natural" (vanishing difference) acceleration implementation. </summary>
	struct natural_base {
		double offset;
		double accel;
		double limit;

		natural_base(const accel_args& args) :
			offset(args.input_offset),
			limit(args.limit - 1)
		{
			accel = args.decay_rate / fabs(limit);
		}

	};

	template<bool Gain> struct natural;

	template<>
	struct natural<LEGACY> : natural_base {

		double operator()(double x, const accel_args&) const
		{
			if (x <= offset) return 1;

			double offset_x = offset - x;
			double decay = exp(accel * offset_x);
			return limit * (1 - (offset - decay * offset_x) / x) + 1;
		}

		using natural_base::natural_base;
	};

	template<>
	struct natural<GAIN> : natural_base {
		double constant;

		double operator()(double x, const accel_args&) const
		{
			if (x <= offset) return 1;

			double offset_x = offset - x;
			double decay = exp(accel * offset_x);
			double output = limit * (decay / accel - offset_x) + constant;
			return output / x + 1;
		}

		natural(const accel_args& args) :
			natural_base(args),
			constant(-limit / accel) {}

	};
}

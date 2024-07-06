#pragma once

#include "rawaccel-base.hpp"

#include <float.h>

namespace rawaccel {

	struct power_base {
		vec2d offset;
		double scale;
		double constant;

		power_base(const accel_args& args)
		{
			auto n = args.exponent_power;

			if (args.cap_mode != cap_mode::io) {
				scale = args.scale;
			}
			else if (args.gain) {
				scale = scale_from_gain_point(args.cap.x, args.cap.y, n);
			}
			else {
				/* 
				* special case for legacy + io cap mode
				* 
				* offset is ignored because of the circular dependency:
				*     scale -> constant -> offset
				*/
				offset = {};
				constant = 0;
				scale = scale_from_output_point(args.cap.x, args.cap.y, n, constant);
				return;
			}

			offset.x = gain_inverse(args.output_offset, n, scale);
			offset.y = args.output_offset;
			constant = offset.x * offset.y * n / (n + 1);
		}

		double base_fn(double x, const accel_args& args) const
		{
			if (x <= offset.x) {
				return offset.y;
			}
			else {     
				return pow(scale * x, args.exponent_power) + constant / x;
			}
		}

		static double gain(double input, double power, double scale)
		{
			return (power + 1) * pow(input * scale, power);
		}

		static double gain_inverse(double gain, double power, double scale)
		{
			return pow(gain / (power + 1), 1 / power) / scale;
		}

		static double scale_from_gain_point(double input, double gain, double power)
		{
			return pow(gain / (power + 1), 1 / power) / input;
		}

		static double scale_from_output_point(double input, double output, double power, double C)
		{
			return pow(output - C / input, 1 / power) / input;
		}
	};

	template <bool Gain> struct power;

	template <>
	struct power<LEGACY> : power_base {
		double cap = DBL_MAX;

		power(const accel_args& args) : 
			power_base(args)
		{

			switch (args.cap_mode){
			case cap_mode::io:
				cap = args.cap.y;
				break;
			case cap_mode::in:
				if (args.cap.x > 0) cap = base_fn(args.cap.x, args);
				break;
			case cap_mode::out:
			default:
				if (args.cap.y > 0) cap = args.cap.y;
				break;
			}
		}

		double operator()(double speed, const accel_args& args) const
		{
			return minsd(base_fn(speed, args), cap);
		}

	};

	template <>
	struct power<GAIN> : power_base {
		vec2d cap = { DBL_MAX, DBL_MAX };
		double constant_b;

		power(const accel_args& args) :
			power_base(args)
		{
			switch (args.cap_mode) {
			case cap_mode::io:
				cap = args.cap;
				break;
			case cap_mode::in:
				if (args.cap.x > 0) {

					if (args.cap.x <= offset.x) {
						cap.x = 0;
						cap.y = offset.y;
						constant_b = 0;
						return;
					}

					cap.x = args.cap.x;
					cap.y = gain(
								args.cap.x,
								args.exponent_power,
								scale);
				}
				break;
			case cap_mode::out:
			default:
				if (args.cap.y > 0) {
					cap.x = gain_inverse(
								args.cap.y,
								args.exponent_power,
								scale);
					cap.y = args.cap.y;
				}
				break;
			}

			constant_b = integration_constant(cap.x, cap.y, base_fn(cap.x, args));
		}

		double operator()(double speed, const accel_args& args) const
		{
			if (speed < cap.x) {
				return base_fn(speed, args);
			}
			else {
				return cap.y + constant_b / speed;
			}
		}

		static double integration_constant(double input, double gain, double output)
		{
			return (output - gain) * input;
		}
	};

}

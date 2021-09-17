#pragma once

#include "rawaccel-base.hpp"

#include <float.h>

namespace rawaccel {

	struct power_base {
		static double base_fn(double x, double scale, const accel_args& args)
		{
			// f(x) = w(mx)^k
			return pow(scale * x, args.exponent_power);
		}
	};

	template <bool Gain> struct power;

	template <>
	struct power<LEGACY> : power_base {
		double cap = DBL_MAX;
		double scale = 0;

		power(const accel_args& args)
		{
			// Note that cap types may overwrite scale below. 
			scale = args.scale;

			switch (args.cap_mode){
			case classic_cap_mode::in:
				if (args.cap.x > 0)
				{
					cap = base_fn(
							args.cap.x,
							args.scale,
							args);
				}
				break;
			case classic_cap_mode::io:
				if (args.cap.x > 0 &&
					args.cap.y > 1)
				{
					cap = args.cap.y;
					scale = scale_from_sens_point(
								args.cap.y,
								args.cap.x,
								args.exponent_power);
				}
				break;
			case classic_cap_mode::out:
			default:
				if (args.cap.y > 1)
				{
					cap = args.cap.y;
				}
				break;
			}
			/*
			if (args.cap.x > 0) {
				cap.x = args.cap.x;
				cap.y = base_fn(cap.x, args);
			}
			*/
		}

		double operator()(double speed, const accel_args& args) const
		{
			if (args.powerStartFromOne) {
				return minsd(maxsd(base_fn(speed, scale, args), 1), cap);
			}
			else {
				return minsd(base_fn(speed, scale, args), cap);
			}
		}

		double static scale_from_sens_point(double sens, double input, double power)
		{
			return pow(sens, 1 / power) / input;
		}
	};

	template <>
	struct power<GAIN> : power_base {
		vec2d cap = { DBL_MAX, DBL_MAX };
		double constant = 0;
		double scale = 0;
		vec2d startFromOne{ 0, 0 };

		power(const accel_args& args) 
		{
			/*
			if (args.cap.x > 0) {
				cap.x = args.cap.x;
				double output = base_fn(cap.x, args);
				cap.y = output * (args.exponent_power + 1);
				constant = -args.exponent_power * output * args.cap.x;
			}
			*/

			// Note that cap types may overwrite this below. 
			scale = args.scale;

			switch (args.cap_mode) {
			case classic_cap_mode::in:
				if (args.cap.x > 0) {
					cap.x = args.cap.x;
					cap.y = gain(
								args.cap.x,
								args.exponent_power,
								scale);
				}
				break;
			case classic_cap_mode::io:
				if (args.cap.x > 0 &&
					args.cap.y > 1) {
					cap.x = args.cap.x;
					cap.y = args.cap.y;
					scale = scale_from_gain_point(
						args.cap.x,
						args.cap.y,
						args.exponent_power);
				}
				break;
			case classic_cap_mode::out:
			default:
				if (args.cap.y > 1) {
					cap.y = args.cap.y;
					cap.x = gain_inverse(
								args.cap.y,
								args.exponent_power,
								scale);
				}
				break;
			}

			if (args.powerStartFromOne)
			{
				startFromOne.x = gain_inverse(
									1,
									args.exponent_power,
									scale);
				startFromOne.y = -1 * integration_constant(startFromOne.x,
										1,
										base_fn(startFromOne.x, scale, args));
			}

			if (cap.x < DBL_MAX && cap.y < DBL_MAX)
			{
				if (args.powerStartFromOne) {
					constant = integration_constant(
								cap.x,
								cap.y,
								startFromOneOutput(
									startFromOne,
									cap.x,
									scale,
									args));
				}
				else {
					constant = integration_constant(
								cap.x,
								cap.y,
								base_fn(cap.x, scale, args));
				}
			}

		}

		double operator()(double speed, const accel_args& args) const
		{
			if (speed < cap.x) {
				if (args.powerStartFromOne) {
					return startFromOneOutput(
							startFromOne,
							speed,
							scale,
							args);
				}
				else {
					return base_fn(speed, scale, args);
				}
			}
			else {
				return cap.y + constant / speed;
			}
		}

		double static startFromOneOutput(
						const vec2d& startFromOne,
						double speed,
						double scale,
						const accel_args& args)
		{
			if (speed > startFromOne.x) {
				return base_fn(speed, scale, args) + startFromOne.y / speed;
			}
			else
			{
				return 1;
			}
		}

		double static gain_inverse(double gain, double power, double scale)
		{
			return pow(gain / (power + 1), 1 / power) / scale;
		}
		double static gain(double input, double power, double scale)
		{
			return (power + 1) * pow(input * scale, power);
		}

		double static scale_from_gain_point(double input, double gain, double power)
		{
			return pow(gain / (power + 1), 1 / power) / input;
		}

		double static integration_constant(double input, double gain, double output)
		{
			return (output - gain) * input;
		}
	};

}

#pragma once

#include "wrapper.hpp"
using namespace rawaccel;
using namespace System;

Tuple<double, double>^ ManagedAccel::Accelerate(int x, int y, double time)
{
	vec2d input_vec2d = {x, y};
	vec2d output = (*modifier_instance).modify_with_accel(input_vec2d, (milliseconds)time);

	return gcnew Tuple<double, double>(output.x, output.y);
}

void ManagedAccel::UpdateAccel(int mode, double offset, double accel, double lim_exp, double midpoint)
{
	delete modifier_instance;

	modifier_args args{};
	args.acc_fn_args.acc_args.accel = accel;
	args.acc_fn_args.acc_args.lim_exp = lim_exp;
	args.acc_fn_args.acc_args.midpoint = midpoint;
	args.acc_fn_args.accel_mode = mode;
	args.acc_fn_args.acc_args.offset = offset;

	modifier_instance = new mouse_modifier(args);
}

void ManagedAccel::WriteToDriver()
{
	driverWriter->writeToDriver(modifier_instance);
}

#pragma once

#include "wrapper.hpp"

using namespace rawaccel;
using namespace System;

Tuple<double, double>^ ManagedAccel::Accelerate(int x, int y, double time)
{
	vec2d input_vec2d = {
		(double)x, 
		(double)y
	};
	vec2d output = (*modifier_instance).modify_with_accel(input_vec2d, (milliseconds)time);

	return gcnew Tuple<double, double>(output.x, output.y);
}

void ManagedAccel::UpdateAccel(
	int mode,
	double rotation,
	double sensitivityX,
	double sensitivityY,
	double weightX,
	double weightY,
	double capX,
	double capY,
	double offset,
	double accel,
	double lim_exp,
	double midpoint,
	double gain_cap)
{
	modifier_args args{};
	args.acc_fn_args.accel_mode = mode;
	args.degrees = rotation;
	args.sens.x = sensitivityX;
	args.sens.y = sensitivityY;
	args.acc_fn_args.acc_args.weight.x = weightX;
	args.acc_fn_args.acc_args.weight.y = weightY;
	args.acc_fn_args.cap.x = capX;
	args.acc_fn_args.cap.y = capY;
	args.acc_fn_args.acc_args.offset = offset;
	args.acc_fn_args.acc_args.accel = accel;
	args.acc_fn_args.acc_args.limit = lim_exp;
	args.acc_fn_args.acc_args.exponent = lim_exp;
	args.acc_fn_args.acc_args.midpoint = midpoint;
	args.acc_fn_args.gain_cap = gain_cap;
	
	mouse_modifier* temp_modifier = new mouse_modifier(args);
	driverWriter->writeToDriver(temp_modifier);
	delete temp_modifier;

	ReadFromDriver();
}

void ManagedAccel::WriteToDriver()
{
	driverWriter->writeToDriver(modifier_instance);
}

void ManagedAccel::ReadFromDriver()
{
	delete modifier_instance;
	modifier_instance = driverWriter->readFromDriver();
}

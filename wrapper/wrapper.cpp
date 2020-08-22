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
	modifier_args args = modifier_args{};
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
	args.acc_fn_args.acc_args.gain_cap = gain_cap;
	
	mouse_modifier* temp_modifier = new mouse_modifier(args);
	driverWriter->writeToDriver(temp_modifier);
	delete temp_modifier;

	ReadFromDriver();

}

double ManagedAccel::SensitivityX::get() { return modifier_instance->sensitivity.x; }
double ManagedAccel::SensitivityY::get() { return modifier_instance->sensitivity.y; }
double ManagedAccel::Rotation::get() { return atan(modifier_instance->rotate.rot_vec.y / modifier_instance->rotate.rot_vec.x) * 180 / M_PI; }
int ManagedAccel::Type::get() { return modifier_instance->accel_fn.accel.tag; }
double ManagedAccel::Acceleration::get() { return modifier_instance->accel_fn.impl_args.accel; }
double ManagedAccel::CapX::get() { return modifier_instance->accel_fn.clamp.x.hi; }
double ManagedAccel::CapY::get() { return modifier_instance->accel_fn.clamp.y.hi; }
double ManagedAccel::GainCap::get() { return modifier_instance->accel_fn.gain_cap.threshold; }
bool ManagedAccel::GainCapEnabled::get() { return modifier_instance->accel_fn.gain_cap.cap_gain_enabled; }
double ManagedAccel::WeightX::get() { return modifier_instance->accel_fn.impl_args.weight.x; }
double ManagedAccel::WeightY::get() { return modifier_instance->accel_fn.impl_args.weight.y; }
double ManagedAccel::Offset::get() { return modifier_instance->accel_fn.speed_offset; }
double ManagedAccel::LimitExp::get() { return modifier_instance->accel_fn.impl_args.limit; }
double ManagedAccel::Midpoint::get() { return modifier_instance->accel_fn.impl_args.midpoint; }
double ManagedAccel::MinimumTime::get() { return modifier_instance->accel_fn.time_min; }
double ManagedAccel::PowerScale::get() { return modifier_instance->accel_fn.impl_args.power_scale; }

void ManagedAccel::WriteToDriver()
{
	driverWriter->writeToDriver(modifier_instance);
}

void ManagedAccel::ReadFromDriver()
{
	delete modifier_instance;
	modifier_instance = driverWriter->readFromDriver();
}

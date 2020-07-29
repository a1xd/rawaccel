#pragma once

#include "..\common\rawaccel.hpp";
#include "wrapper.hpp";
using namespace rawaccel;
using namespace System;

Tuple<double, double>^ ManagedAccel::Accelerate(int x, int y, double time)
{
	vec2d input_vec2d = {x, y};
	vec2d output = (*modifier_instance).modify_with_accel(input_vec2d, (milliseconds)time);

	return gcnew Tuple<double, double>(output.x, output.y);
}
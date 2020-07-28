#pragma once

#include "..\common\rawaccel.hpp";
#include "wrapper.hpp";
using namespace rawaccel;
using namespace System;

Tuple<double, double>^ ManagedAccel::Accelerate(int x, int y, double time, double mode)
{
	vec2d input_vec2d = {x, y};
	vec2d output = (*accel_instance)(input_vec2d, (milliseconds)time);

	return gcnew Tuple<double, double>(output.x, output.y);
}
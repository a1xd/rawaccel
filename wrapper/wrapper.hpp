#pragma once

#include "..\common\rawaccel.hpp";
#include "..\common\error.hpp";
#include <iostream>
using namespace rawaccel;
using namespace System;

public ref class ManagedAccel
{
protected:
	accel_function* accel_instance;
public:
	ManagedAccel(accel_function* accel)
		: accel_instance(accel)
	{
	}

    ManagedAccel(double mode, double offset, double accel, double lim_exp, double midpoint)
    {
        accel_args args{};
        args.accel = accel;
        args.lim_exp = lim_exp;
        args.midpoint = midpoint;
        args.accel_mode = (rawaccel::mode)mode;
        args.offset = offset;

		accel_instance = new accel_function(args);
	}

    virtual ~ManagedAccel()
    {
        if (accel_instance != nullptr)
        {
            delete accel_instance;
        }
    }
    !ManagedAccel()
    {
        if (accel_instance != nullptr)
        {
            delete accel_instance;
        }
    }

    accel_function* GetInstance()
    {
        return accel_instance;
    }

    Tuple<double, double>^ Accelerate(int x, int y, double time);
};
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

    ManagedAccel(int mode, double offset, double accel, double lim_exp, double midpoint)
    {
        accel_fn_args args{};
        args.acc_args.accel = accel;
        args.acc_args.lim_exp = lim_exp;
        args.acc_args.midpoint = midpoint;
        args.accel_mode = mode;
        args.acc_args.offset = offset;

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
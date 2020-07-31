#pragma once

#include "wrapper_writer.hpp"
#include "..\common\rawaccel.hpp";
#include "..\common\accel-error.hpp";
#include <iostream>
using namespace rawaccel;
using namespace System;


public value struct ArgsWrapper {
    int a;
};

public ref class ManagedAccel
{
protected:
	mouse_modifier* modifier_instance;
    writer* driverWriter;
public:
	ManagedAccel(mouse_modifier* accel)
		: modifier_instance(accel)
	{
        driverWriter = new writer();
	}

    ManagedAccel(System::IntPtr args)
    {
        modifier_instance = new mouse_modifier(*reinterpret_cast<modifier_args*>(args.ToPointer()));
        driverWriter = new writer();
	}

    virtual ~ManagedAccel()
    {
        if (modifier_instance!= nullptr)
        {
            delete modifier_instance;
        }
    }
    !ManagedAccel()
    {
        if (modifier_instance!= nullptr)
        {
            delete modifier_instance;
        }
    }

    mouse_modifier* GetInstance()
    {
        return modifier_instance;
    }

    Tuple<double, double>^ Accelerate(int x, int y, double time);

    void UpdateAccel(
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
        double midpoint);


    void WriteToDriver();
};
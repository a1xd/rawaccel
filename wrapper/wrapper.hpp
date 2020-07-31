#pragma once

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
public:
	ManagedAccel(mouse_modifier* accel)
		: modifier_instance(accel)
	{
	}

    ManagedAccel(System::IntPtr args)
    {
        modifier_instance = new mouse_modifier(*reinterpret_cast<modifier_args*>(args.ToPointer()));
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
};
#pragma once

#include <iostream>

#include "wrapper_io.hpp"

using namespace rawaccel;
using namespace System;

public ref class ManagedAccel
{
protected:
	mouse_modifier* modifier_instance;
    wrapper_io* driverWriter;
public:
	ManagedAccel(mouse_modifier* accel)
		: modifier_instance(accel)
	{
        driverWriter = new wrapper_io();
	}

    ManagedAccel(System::IntPtr args)
    {
        modifier_instance = new mouse_modifier(*reinterpret_cast<modifier_args*>(args.ToPointer()));
        driverWriter = new wrapper_io();
	}

    // Empty constructor needed for serialization
    ManagedAccel() {}

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

    // Duplicate all relevant rawaccel struct members here for access and display in GUI
    property double SensitivityX { double get(); }
    property double SensitivityY { double get(); }
    property double Rotation { double get(); }
    property int Type { int get(); }
    property double Acceleration { double get(); }
    property bool GainCapEnabled { bool get(); }
    property double CapX { double get(); }
    property double CapY { double get(); }
    property double GainCap { double get(); }
    property double WeightX { double get(); }
    property double WeightY { double get(); }
    property double Offset { double get(); }
    property double LimitExp { double get(); }
    property double Midpoint { double get(); }
    property double MinimumTime { double get(); }
    property double PowerScale { double get(); }
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
        double midpoint,
        double gain_cap);


    void WriteToDriver();

    void ReadFromDriver();
};
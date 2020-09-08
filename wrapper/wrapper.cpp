#pragma once

#include <rawaccel.hpp>

#include "wrapper_io.hpp"

using namespace System;

public ref struct DriverInterop
{
    static void GetActiveSettings(IntPtr argsOut)
    {
        wrapper_io::readFromDriver(*reinterpret_cast<settings*>(argsOut.ToPointer()));
    }

    static void SetActiveSettings(IntPtr argsIn)
    {
        wrapper_io::writeToDriver(*reinterpret_cast<settings*>(argsIn.ToPointer()));
    }
};

public ref class ManagedAccel
{
    mouse_modifier* const modifier_instance = new mouse_modifier();

public:
    static initonly double WriteDelay = -10000000 / -10000.0;

    virtual ~ManagedAccel()
    {
        delete modifier_instance;
    }

    !ManagedAccel()
    {
        delete modifier_instance;
    }

    Tuple<double, double>^ Accelerate(int x, int y, double time)
    {
        vec2d in_out_vec = {
            (double)x,
            (double)y
        };
        modifier_instance->modify(in_out_vec, time);

        return gcnew Tuple<double, double>(in_out_vec.x, in_out_vec.y);
    }

    void UpdateFromSettings(IntPtr argsIn)
    {
        *modifier_instance = { *reinterpret_cast<settings*>(argsIn.ToPointer()) };
    }

    static ManagedAccel^ GetActiveAccel()
    {
        settings args;
        wrapper_io::readFromDriver(args);

        auto active = gcnew ManagedAccel();
        *active->modifier_instance = { args };
        return active;
    }
};

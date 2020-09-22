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
#ifdef RA_LOOKUP
    si_pair* const lut_x = new si_pair[LUT_SIZE];
    si_pair* const lut_y = new si_pair[LUT_SIZE];
#else
    si_pair* lut_x = nullptr;
    si_pair* lut_y = nullptr;
#endif

public:
    static initonly double WriteDelay = -10000000 / -10000.0;

    virtual ~ManagedAccel()
    {
        delete modifier_instance;
        delete[] lut_x;
        delete[] lut_y;
    }

    !ManagedAccel()
    {
        delete modifier_instance;
        delete[] lut_x;
        delete[] lut_y;
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
        *modifier_instance = { 
            *reinterpret_cast<settings*>(argsIn.ToPointer())
            , vec2<si_pair*>{ lut_x, lut_y }
        };
    }

    static ManagedAccel^ GetActiveAccel()
    {
        settings args;
        wrapper_io::readFromDriver(args);

        auto active = gcnew ManagedAccel();
        *active->modifier_instance = { 
            args
            , vec2<si_pair*> { active->lut_x, active->lut_y }
        };
        return active;
    }
};

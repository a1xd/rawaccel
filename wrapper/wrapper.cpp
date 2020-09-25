#pragma once

#include <type_traits>

#include <rawaccel.hpp>

#include "wrapper_io.hpp"

using namespace System;
using namespace System::Runtime::InteropServices;

public enum class AccelMode
{
    linear, classic, natural, naturalgain, power, logarithm, motivity, noaccel
};

[StructLayout(LayoutKind::Sequential)]
public value struct AccelArgs
{
    double offset;
    double legacy_offset;
    double accel;
    double limit;
    double exponent;
    double midpoint;
    double powerScale;
    double powerExponent;
    double weight;
    double rate;
    double scaleCap;
    double gainCap;
};

generic <typename T>
[StructLayout(LayoutKind::Sequential)]
public value struct Vec2
{
    T x;
    T y;
};

[Serializable]
[StructLayout(LayoutKind::Sequential)]
public ref struct DriverSettings
{
    double rotation;
    [MarshalAs(UnmanagedType::U1)]
    bool combineMagnitudes;
    Vec2<AccelMode> modes;
    Vec2<AccelArgs> args;
    Vec2<double> sensitivity;
    double minimumTime;
};


template <typename NativeSettingsFunc>
void as_native(DriverSettings^ managed_args, NativeSettingsFunc fn)
{
#ifndef NDEBUG
    if (Marshal::SizeOf(managed_args) != sizeof(settings))
        throw gcnew InvalidOperationException("setting sizes differ");
#endif
    IntPtr unmanagedHandle = Marshal::AllocHGlobal(sizeof(settings));
    Marshal::StructureToPtr(managed_args, unmanagedHandle, false);
    fn(*reinterpret_cast<settings*>(unmanagedHandle.ToPointer()));
    if constexpr (!std::is_invocable_v<NativeSettingsFunc, const settings&>) {
        Marshal::PtrToStructure(unmanagedHandle, managed_args);
    }
    Marshal::FreeHGlobal(unmanagedHandle);
}

DriverSettings^ get_default()
{
    DriverSettings^ managed = gcnew DriverSettings();
    as_native(managed, [](settings& args) {
        args = {};
    });
    return managed;
}

void set_active(DriverSettings^ managed)
{
    as_native(managed, [](const settings& args) {
        wrapper_io::writeToDriver(args);
    });
}

DriverSettings^ get_active()
{
    DriverSettings^ managed = gcnew DriverSettings();
    as_native(managed, [](settings& args) {
        wrapper_io::readFromDriver(args);
    });
    return managed;
}

void update_modifier(mouse_modifier& mod, DriverSettings^ managed, vec2<si_pair*> luts = {})
{
    as_native(managed, [&](const settings& args) {
        mod = { args, luts };
    });
}

public ref struct DriverInterop
{
    static DriverSettings^ GetActiveSettings()
    {
        return get_active();
    }

    static void SetActiveSettings(DriverSettings^ args)
    {
        set_active(args);
    }

    static DriverSettings^ GetDefaultSettings()
    {
        return get_default();
    }

    using error_list_t = Collections::Generic::List<String^>;

    static error_list_t^ GetErrors(AccelArgs^ args)
    {
        auto error_list = gcnew error_list_t();

        if (args->accel < 0 || args->rate < 0)
                error_list->Add("accel can not be negative, use a negative weight to compensate");
        if (args->rate > 1) error_list->Add("rate can not be greater than 1");
        if (args->exponent <= 1) error_list->Add("exponent must be greater than 1");
        if (args->limit <= 1) error_list->Add("limit must be greater than 1");
        if (args->powerScale <= 0) error_list->Add("scale must be positive");
        if (args->powerExponent <= 0) error_list->Add("exponent must be positive");
        if (args->midpoint < 0) error_list->Add("midpoint must not be negative");
        
        return error_list;
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
    static initonly double WriteDelay = WRITE_DELAY;

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

    void UpdateFromSettings(DriverSettings^ args)
    {
        update_modifier(
            *modifier_instance, 
            args, 
            vec2<si_pair*>{ lut_x, lut_y }
        );
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

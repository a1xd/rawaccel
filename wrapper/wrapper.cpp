#pragma once

#include <type_traits>
#include <msclr\marshal_cppstd.h>

#include <rawaccel.hpp>
#include <rawaccel-version.h>
#include <utility-rawinput.hpp>

#include "wrapper_io.hpp"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

using namespace Windows::Forms;

using namespace Newtonsoft::Json;

[JsonConverter(Converters::StringEnumConverter::typeid)]
public enum class AccelMode
{
    linear, classic, natural, naturalgain, power, motivity, noaccel
};

[JsonObject(ItemRequired = Required::Always)]
[StructLayout(LayoutKind::Sequential)]
public value struct AccelArgs
{
    double offset;
    [MarshalAs(UnmanagedType::U1)]
    bool legacyOffset;
    double acceleration;
    double scale;
    double limit;
    double exponent;
    double midpoint;
    double weight;
    [JsonProperty("legacyCap")]
    double scaleCap;
    double gainCap;
    [JsonProperty(Required = Required::Default)]
    double speedCap;
};

generic <typename T>
[JsonObject(ItemRequired = Required::Always)]
[StructLayout(LayoutKind::Sequential)]
public value struct Vec2
{
    T x;
    T y;
};

[JsonObject(ItemRequired = Required::Always)]
[StructLayout(LayoutKind::Sequential)]
public value struct DomainArgs
{
    Vec2<double> domainXY;
    double lpNorm;
};

[JsonObject(ItemRequired = Required::Always)]
[StructLayout(LayoutKind::Sequential, CharSet = CharSet::Unicode)]
public ref struct DriverSettings
{
    literal String^ Key = "Driver settings";

    [JsonProperty("Degrees of rotation")]
    double rotation;

    [JsonProperty("Degrees of angle snapping", Required = Required::Default)]
    double snap;

    [JsonProperty("Use x as whole/combined accel")]
    [MarshalAs(UnmanagedType::U1)]
    bool combineMagnitudes;

    [JsonProperty("Accel modes")]
    Vec2<AccelMode> modes;

    [JsonProperty("Accel parameters")]
    Vec2<AccelArgs> args;

    [JsonProperty("Sensitivity multipliers")]
    Vec2<double> sensitivity;

    [JsonProperty("Negative directional multipliers", Required = Required::Default)]
    Vec2<double> directionalMultipliers;

    [JsonProperty("Stretches domain for horizontal vs vertical inputs", Required = Required::Default)]
    DomainArgs domainArgs;

    [JsonProperty("Stretches accel range for horizontal vs vertical inputs", Required = Required::Default)]
    Vec2<double> rangeXY;

    [JsonProperty(Required = Required::Default)]
    double minimumTime;

    [JsonProperty("Device ID", Required = Required::Default)]
    [MarshalAs(UnmanagedType::ByValTStr, SizeConst = MAX_DEV_ID_LEN)]
    String^ deviceID = "";

    bool ShouldSerializeminimumTime() 
    { 
        return minimumTime > 0 && minimumTime != DEFAULT_TIME_MIN;
    }

    DriverSettings() 
    {
        domainArgs = { { 1, 1 }, 2 };
        rangeXY = { 1, 1 };
    }
};


template <typename NativeSettingsFunc>
void as_native(DriverSettings^ managed_args, NativeSettingsFunc fn)
{
#ifndef NDEBUG
    if (Marshal::SizeOf(managed_args) != sizeof(settings))
        throw gcnew InvalidOperationException("setting sizes differ");
#endif
    settings args;
    Marshal::StructureToPtr(managed_args, (IntPtr)&args, false);
    fn(args);
    if constexpr (!std::is_invocable_v<NativeSettingsFunc, const settings&>) {
        Marshal::PtrToStructure((IntPtr)&args, managed_args);
    }
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

using error_list_t = Collections::Generic::List<String^>;

error_list_t^ get_accel_errors(AccelMode mode, AccelArgs^ args)
{
    accel_mode mode_native = (accel_mode)mode;

    auto is_mode = [mode_native](auto... modes) {
        return ((mode_native == modes) || ...);
    };
    
    using am = accel_mode;

    auto error_list = gcnew error_list_t();
    
    if (args->acceleration > 10 && is_mode(am::natural, am::naturalgain))
        error_list->Add("acceleration can not be greater than 10");
    else if (args->acceleration == 0 && is_mode(am::naturalgain))
        error_list->Add("acceleration must be positive");
    else if (args->acceleration < 0) {
        bool additive = mode_native < am::power;
        if (additive) error_list->Add("acceleration can not be negative, use a negative weight to compensate");
        else error_list->Add("acceleration can not be negative");
    }
        
    if (args->scale <= 0)
        error_list->Add("scale must be positive");

    if (args->exponent <= 1 && is_mode(am::classic))
        error_list->Add("exponent must be greater than 1");
    else if (args->exponent <= 0)
        error_list->Add("exponent must be positive");

    if (args->limit <= 1)
        error_list->Add("limit must be greater than 1");

    if (args->midpoint <= 0)
        error_list->Add("midpoint must be positive");

    if (args->offset < 0)
        error_list->Add("offset can not be negative");

    return error_list;
}

error_list_t^ get_other_errors(DriverSettings^ settings)
{
    auto error_list = gcnew error_list_t();

    if (settings->rangeXY.x <= 0 || settings->rangeXY.y <= 0)
    {
        error_list->Add("range values must be positive");
    }

    if (settings->domainArgs.domainXY.x <= 0 || settings->domainArgs.domainXY.y <= 0)
    {
        error_list->Add("domain values must be positive");
    }

    if (settings->domainArgs.lpNorm <= 0)
    {
        error_list->Add("lp norm must be positive");
    }
    
    return error_list;
}

public ref class SettingsErrors
{
public:
    error_list_t^ x;
    error_list_t^ y;
    error_list_t^ other;

    bool Empty()
    {
        return (x == nullptr || x->Count == 0) && 
            (y == nullptr || y->Count == 0) &&
            (other == nullptr || other->Count == 0);
    }

    virtual String^ ToString() override 
    {
        if (x == nullptr) throw;

        Text::StringBuilder^ sb = gcnew Text::StringBuilder();

        if (y == nullptr) // assume combineMagnitudes
        {
            for each (String^ str in x)
            {
                sb->AppendLine(str);
            }
        }
        else
        {
            for each (String^ str in x)
            {
                sb->AppendFormat("x: {0}\n", str);
            }
            for each (String^ str in y)
            {
                sb->AppendFormat("y: {0}\n", str);
            }
        }

        for each (String ^ str in other)
        {
			sb->AppendLine(str);
        }
        
        return sb->ToString();
    }
};

public ref struct RawInputInterop
{
    static void AddHandlesFromID(String^ deviceID, List<IntPtr>^ rawInputHandles)
    {
        try
        {
            std::vector<HANDLE> nativeHandles = rawinput_handles_from_dev_id(
                msclr::interop::marshal_as<std::wstring>(deviceID));

            for (auto nh : nativeHandles) rawInputHandles->Add(IntPtr(nh));
        }
        catch (const std::exception& e)
        {
            throw gcnew System::Exception(gcnew String(e.what()));
        }
    }

    static List<String^>^ GetDeviceIDs()
    {
        try
        {
            auto managed = gcnew List<String^>();

            for (auto&& id : rawinput_dev_id_list())
            {
                managed->Add(msclr::interop::marshal_as<String^>(id));
            }

            return managed;
        }
        catch (const std::exception& e)
        {
            throw gcnew System::Exception(gcnew String(e.what()));
        }
    }

};

public ref struct DriverInterop
{
    literal double WriteDelayMs = WRITE_DELAY;
    static initonly DriverSettings^ DefaultSettings = get_default();

    static DriverSettings^ GetActiveSettings()
    {
        return get_active();
    }

    static void Write(DriverSettings^ args)
    {
        set_active(args);
    }

    static DriverSettings^ GetDefaultSettings()
    {
        return get_default();
    }

    static SettingsErrors^ GetSettingsErrors(DriverSettings^ args)
    {
        auto errors = gcnew SettingsErrors();

        errors->x = get_accel_errors(args->modes.x, args->args.x);

        if (!args->combineMagnitudes) {
            errors->y = get_accel_errors(args->modes.y, args->args.y);
        }

        errors->other = get_other_errors(args);

        return errors;
    }



    static error_list_t^ GetAccelErrors(AccelMode mode, AccelArgs^ args)
    {
        return get_accel_errors(mode, args);
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

public ref struct RawAccelVersion
{
    literal String^ value = RA_VER_STRING;
};

public ref struct VersionException : public Exception 
{
public:
    VersionException() {}
    VersionException(String^ what) : Exception(what) {}
};

Version^ convert(rawaccel::version_t v)
{
    return gcnew Version(v.major, v.minor, v.patch, 0);
}

public ref struct VersionHelper
{

    static Version^ ValidateAndGetDriverVersion(Version^ wrapperTarget)
    {
        Version^ wrapperActual = VersionHelper::typeid->Assembly->GetName()->Version;

        if (wrapperTarget != wrapperActual) {
            throw gcnew VersionException("version mismatch, expected wrapper.dll v" + wrapperActual);
        }

        version_t drv_ver;

        try {
            wrapper_io::getDriverVersion(drv_ver);
        }
        catch (DriverNotInstalledException^ ex) {
            throw gcnew VersionException(ex->Message);
        }
        catch (DriverIOException^) {
            // Assume version ioctl is unimplemented (driver version < v1.3.0)
            throw gcnew VersionException("driver version is out of date\n\nrun installer.exe to reinstall");
        }

        Version^ drv_ver_managed = convert(drv_ver);

        if (drv_ver_managed < convert(min_driver_version)) {
            throw gcnew VersionException(
                String::Format("driver version is out of date (v{0})\n\nrun installer.exe to reinstall", 
                    drv_ver_managed));
        }
        else if (drv_ver_managed > wrapperActual) {
            throw gcnew VersionException(
                String::Format("newer driver version is installed (v{0})",
                    drv_ver_managed));
        }
        else {
            return drv_ver_managed;
        }
    }

};

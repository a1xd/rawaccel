#pragma once

#include <rawaccel-io.hpp>
#include <rawaccel-validate.hpp>
#include <utility-rawinput.hpp>

#include <algorithm>
#include <type_traits>
#include <msclr\marshal_cppstd.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

using namespace Windows::Forms;

using namespace Newtonsoft::Json;

namespace ra = rawaccel;

ra::settings default_settings;

[JsonConverter(Converters::StringEnumConverter::typeid)]
public enum class AccelMode
{
    classic, jump, natural, power, motivity, lut, noaccel
};

public enum class TableMode
{
    off, binlog, linear
};

public enum class TableType
{
    spaced, arbitrary
};

[StructLayout(LayoutKind::Sequential)]
public value struct TableArgs
{
    [JsonIgnore]
    TableMode mode;

    [JsonIgnore]
    TableType type;

    [MarshalAs(UnmanagedType::U1)]
    bool transfer;

    [MarshalAs(UnmanagedType::U1)]
    unsigned char partitions;

    short num;
    double start;
    double stop;
};


generic <typename T>
[StructLayout(LayoutKind::Sequential)]
public value struct Vec2
{
    T x;
    T y;
};

public ref struct SpacedTable
{
    [JsonProperty("Arguments for spacing in table")]
    TableArgs args;

    [JsonProperty("Series of points for use in curve")]
    List<double>^ points;
};

public ref struct ArbitraryTable
{
    [JsonProperty("Whether points affect velocity (true) or sensitivity (false)")]
    bool transfer;

    [JsonProperty("Series of points for use in curve")]
    List<Vec2<double>>^ points;
};

[StructLayout(LayoutKind::Sequential)]
public value struct AccelArgs
{
    AccelMode mode;

    [MarshalAs(UnmanagedType::U1)]
    bool legacy;

    [JsonProperty(Required = Required::Default)]
    TableArgs lutArgs;

    double offset;
    double cap;
    double accelClassic;
    double accelNatural;
    double accelMotivity;
    double motivity;
    double power;
    double scale;
    double weight;
    double exponent;
    double limit;
    double midpoint;
    double smooth;
};

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
    literal double WriteDelayMs = ra::WRITE_DELAY;
    literal String^ Key = "Driver settings";

    [JsonProperty("Degrees of rotation")]
    double rotation;

    [JsonProperty("Degrees of angle snapping")]
    double snap;

    [JsonProperty("Use x as whole/combined accel")]
    [MarshalAs(UnmanagedType::U1)]
    bool combineMagnitudes;

    double dpi;

    [JsonIgnore]
    double minimumSpeed;
    [JsonProperty("Input Speed Cap")]
    double maximumSpeed;

    [JsonProperty("Accel parameters")]
    Vec2<AccelArgs> args;

    [JsonProperty("Sensitivity multipliers")]
    Vec2<double> sensitivity;

    [JsonProperty("Negative directional multipliers")]
    Vec2<double> directionalMultipliers;

    [JsonProperty("Stretches domain for horizontal vs vertical inputs")]
    DomainArgs domainArgs;

    [JsonProperty("Stretches accel range for horizontal vs vertical inputs")]
    Vec2<double> rangeXY;

    [JsonProperty(Required = Required::Default)]
    double minimumTime;

    [JsonProperty(Required = Required::Default)]
    double maximumTime;

    [JsonProperty("Ignore devices with matching ID")]
    [MarshalAs(UnmanagedType::U1)]
    bool ignore;

    [JsonProperty("Device ID")]
    [MarshalAs(UnmanagedType::ByValTStr, SizeConst = ra::MAX_DEV_ID_LEN)]
    String^ deviceID = "";

    [JsonIgnore]
    SpacedTable^ SpacedTable;

    [JsonIgnore]
    ArbitraryTable^ ArbitraryTable;

    bool ShouldSerializeminimumTime() 
    { 
        return minimumTime != ra::DEFAULT_TIME_MIN;
    }

    bool ShouldSerializemaximumTime()
    {
        return maximumTime != ra::DEFAULT_TIME_MAX;
    }

    DriverSettings() 
    {
        Marshal::PtrToStructure(IntPtr(&default_settings), this);
    }
    
    void ToFile(String^ path)
    {
        using namespace Newtonsoft::Json::Linq;

        JObject^ thisJO = JObject::FromObject(this);
        String^ modes = String::Join(" | ", Enum::GetNames(AccelMode::typeid));
        thisJO->AddFirst(gcnew JProperty("### Accel Modes ###", modes));
        File::WriteAllText(path, thisJO->ToString(Formatting::Indented));
    }

    static DriverSettings^ FromFile(String^ path)
    {
        if (!File::Exists(path))
        {
            throw gcnew FileNotFoundException(
                String::Format("Settings file not found at {0}", path));
        }

        auto settings = JsonConvert::DeserializeObject<DriverSettings^>(
            File::ReadAllText(path));

        if (settings == nullptr) {
            throw gcnew JsonException(String::Format("{0} contains invalid JSON", path));
        }

        return settings;
    }
};

public ref struct InteropException : public Exception {
public:
    InteropException(String^ what) :
        Exception(what) {}
    InteropException(const char* what) :
        Exception(gcnew String(what)) {}
    InteropException(const std::exception& e) :
        InteropException(e.what()) {}
};

public ref class SettingsErrors
{
public:
    List<String^>^ list;
    int countX;
    int countY;

    delegate void MsgHandler(const char*);

    void Add(const char* msg)
    {
        list->Add(msclr::interop::marshal_as<String^>(msg));
    }

    SettingsErrors(DriverSettings^ settings)
    {
        MsgHandler^ del = gcnew MsgHandler(this, &SettingsErrors::Add);
        GCHandle gch = GCHandle::Alloc(del);
        auto fp = static_cast<void (*)(const char*)>(
            Marshal::GetFunctionPointerForDelegate(del).ToPointer());

        ra::settings args;
        Marshal::StructureToPtr(settings, (IntPtr)&args, false);

        list = gcnew List<String^>();
        auto [cx, cy, _] = ra::valid(args, fp);
        countX = cx;
        countY = cy;

        gch.Free();
    }

    bool Empty()
    {
        return list->Count == 0;
    }

    virtual String^ ToString() override
    {
        Text::StringBuilder^ sb = gcnew Text::StringBuilder();

        for each (auto s in list->GetRange(0, countX))
        {
            sb->AppendFormat("x: {0}\n", s);
        }
        for each (auto s in list->GetRange(countX, countY))
        {
            sb->AppendFormat("y: {0}\n", s);
        }
        for each (auto s in list->GetRange(countY, list->Count))
        {
            sb->AppendLine(s);
        }

        return sb->ToString();
    }
};

struct device_info {
    std::wstring name;
    std::wstring id;
};

std::vector<device_info> get_unique_device_info() {
    std::vector<device_info> info;

    rawinput_foreach_with_interface([&](const auto& dev, const WCHAR* name) {
        info.push_back({
            L"", // get_property_wstr(name, &DEVPKEY_Device_FriendlyName), /* doesn't work */
            dev_id_from_interface(name)
        });
    });

    std::sort(info.begin(), info.end(),
        [](auto&& l, auto&& r) { return l.id < r.id; });
    auto last = std::unique(info.begin(), info.end(),
        [](auto&& l, auto&& r) { return l.id == r.id; });
    info.erase(last, info.end());

    return info;
}

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
            throw gcnew InteropException(e);
        }
    }

    static List<ValueTuple<String^, String^>>^ GetDeviceIDs()
    {
        try
        {
            auto managed = gcnew List<ValueTuple<String^, String^>>();

            for (auto&& [name, id] : get_unique_device_info())
            {
                managed->Add(
                    ValueTuple<String^, String^>(
                        msclr::interop::marshal_as<String^>(name),
                        msclr::interop::marshal_as<String^>(id)));
            }

            return managed;
        }
        catch (const std::exception& e)
        {
            throw gcnew InteropException(e);
        }
    }

};

struct instance_t {
    ra::io_t data;
    vec2<ra::accel_invoker> inv;
};

public ref class ManagedAccel
{
    instance_t* const instance = new instance_t();

public:
    ManagedAccel() {};

    ManagedAccel(DriverSettings^ settings)
    {
        Settings = settings;
    }

    virtual ~ManagedAccel()
    {
        delete instance;
    }

    !ManagedAccel()
    {
        delete instance;
    }

    Tuple<double, double>^ Accelerate(int x, int y, double time)
    {
        vec2d in_out_vec = {
            (double)x,
            (double)y
        };

        instance->data.mod.modify(in_out_vec, instance->inv, time);

        return gcnew Tuple<double, double>(in_out_vec.x, in_out_vec.y);
    }

    void Activate()
    {
        try {
            ra::write(instance->data);
        }
        catch (const ra::error& e) {
            throw gcnew InteropException(e);
        }
    }

    property DriverSettings^ Settings
    {
        DriverSettings^ get()
        {
            DriverSettings^ settings = gcnew DriverSettings();
            Marshal::PtrToStructure(IntPtr(&instance->data.args), settings);
            return settings;
        }

        void set(DriverSettings^ val)
        {
            Marshal::StructureToPtr(val, IntPtr(&instance->data.args), false);
            instance->data.mod = { instance->data.args };
            instance->inv = ra::invokers(instance->data.args);
        }

    }

    static ManagedAccel^ GetActive()
    {
        try {
            auto active = gcnew ManagedAccel();
            ra::read(active->instance->data);
            active->instance->inv = ra::invokers(active->instance->data.args);
            return active;
        }
        catch (const ra::error& e) {
            throw gcnew InteropException(e);
        }
    }
};

public ref struct VersionHelper
{
    literal String^ VersionString = RA_VER_STRING;

    static Version^ ValidOrThrow()
    {
        try {
            ra::version_t v = ra::valid_version_or_throw();
            return gcnew Version(v.major, v.minor, v.patch, 0);
        }
        catch (const ra::error& e) {
            throw gcnew InteropException(e);
        }
    }

};

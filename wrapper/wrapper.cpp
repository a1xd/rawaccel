#pragma once

#include "interop-exception.h"

#include <rawaccel-io.hpp>
#include <rawaccel-validate.hpp>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

using namespace Newtonsoft::Json;
using namespace Newtonsoft::Json::Linq;

namespace ra = rawaccel;

ra::settings default_settings;

[JsonConverter(Converters::StringEnumConverter::typeid)]
public enum class AccelMode
{
    classic, jump, natural, power, motivity, noaccel
};

[JsonConverter(Converters::StringEnumConverter::typeid)]
public enum class TableMode
{
    off, binlog, linear, arbitrary
};

[StructLayout(LayoutKind::Sequential)]
public value struct TableArgs
{
    [JsonIgnore]
    TableMode mode;

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
    double growthRate;
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
};

[JsonObject(ItemRequired = Required::Always)]
public ref struct LutBase abstract
{
    [JsonConverter(Converters::StringEnumConverter::typeid)]
    enum class Mode
    {
        logarithmic, linear, arbitrary
    } mode;

    virtual void SetArgs(TableArgs%) abstract;
    virtual void SetData(ra::accel_union&) abstract;
};

[JsonObject(ItemRequired = Required::Always)]
public ref struct ArbitraryLut sealed : public LutBase
{
    [JsonProperty("Whether points affect velocity (true) or sensitivity (false)")]
    bool transfer;

    array<float, 2>^ data;

    virtual void SetArgs(TableArgs% args) override
    {
        args.mode = TableMode::arbitrary;
        args.transfer = transfer;
    }

    virtual void SetData(ra::accel_union& accel) override
    {
        throw gcnew NotImplementedException();
    }

};

[JsonObject(ItemRequired = Required::Always)]
public ref struct SpacedLut abstract : public LutBase
{
    [JsonProperty("Whether points affect velocity (true) or sensitivity (false)")]
    bool transfer;

    double start;
    double stop;
    array<float>^ data;

    void SetArgsBase(TableArgs% args)
    {
        args.transfer = transfer;
        args.start = start;
        args.stop = stop;
    }

    void SetDataBase(ra::accel_union& accel)
    {
        if (size_t(data->LongLength) > ra::LUT_CAPACITY) {
            throw gcnew InteropException("data is too large");
        }
    }
};

[JsonObject(ItemRequired = Required::Always)]
public ref struct LinearLut sealed : public SpacedLut
{
    LinearLut(const ra::linear_lut& table)
    {
        mode = Mode::linear;
        transfer = table.transfer;
        start = table.range.start;
        stop = table.range.stop;
        data = gcnew array<float>(table.range.num);

        pin_ptr<float> pdata = &data[0];
        std::memcpy(pdata, &table.data, sizeof(float) * data->Length);
    }

    virtual void SetArgs(TableArgs% args) override
    {
        SetArgsBase(args);
        args.num = data->Length;
        args.mode = TableMode::linear;
    }

    virtual void SetData(ra::accel_union& accel) override
    {
        SetDataBase(accel);
        pin_ptr<float> pdata = &data[0];
        std::memcpy(&accel.lin_lut.data, pdata, sizeof(float) * data->Length);
    }
};

[JsonObject(ItemRequired = Required::Always)]
public ref struct BinLogLut sealed : public SpacedLut
{
    short num;

    BinLogLut(const ra::binlog_lut& table)
    {
        mode = Mode::logarithmic;
        transfer = table.transfer;
        start = table.range.start;
        stop = table.range.stop;
        num = table.range.num;
        data = gcnew array<float>(1 + num * (int(stop) - int(start)));

        pin_ptr<float> pdata = &data[0];
        std::memcpy(pdata, &table.data, sizeof(float) * data->Length);
    }

    virtual void SetArgs(TableArgs% args) override
    {
        SetArgsBase(args);
        args.num = num;
        args.mode = TableMode::binlog;
    }

    virtual void SetData(ra::accel_union& accel) override
    {
        SetDataBase(accel);

        if (data->Length != 1 + num * (int(stop) - int(start))) {
            throw gcnew InteropException("size of data does not match args");
        }

        pin_ptr<float> pdata = &data[0];
        std::memcpy(&accel.log_lut.data, pdata, sizeof(float) * data->Length);
    }
};

public ref struct RaConvert {

    static DriverSettings^ Settings(String^ json)
    {
        return NonNullable<DriverSettings^>(json);
    }

    static String^ Settings(DriverSettings^ settings)
    {
        JObject^ jObject = JObject::FromObject(settings);
        String^ modes = String::Join(" | ", Enum::GetNames(AccelMode::typeid));
        jObject->AddFirst(gcnew JProperty("### Accel Modes ###", modes));
        return jObject->ToString(Formatting::Indented);
    }

    static LutBase^ Table(String^ json)
    {
        JObject^ jObject = JObject::Parse(json);

        if ((Object^)jObject == nullptr) {
            throw gcnew JsonException("bad json");
        }

        LutBase^ base = NonNullable<LutBase^>(jObject);

        switch (base->mode) {
        case LutBase::Mode::logarithmic:
            return NonNullable<BinLogLut^>(jObject);
        case LutBase::Mode::linear:
            return NonNullable<LinearLut^>(jObject);
        case LutBase::Mode::arbitrary:
            return NonNullable<ArbitraryLut^>(jObject);
        default:
            throw gcnew NotImplementedException();
        }
    }

    static String^ Table(LutBase^ lut)
    {
        auto serializerSettings = gcnew JsonSerializerSettings();
        return JsonConvert::SerializeObject(
            lut, lut->GetType(), Formatting::Indented, serializerSettings);
    };

    generic <typename T>
    static T NonNullable(String^ json)
    {
        T obj = JsonConvert::DeserializeObject<T>(json);
        if (obj == nullptr) throw gcnew JsonException("invalid JSON");
        return obj;
    }

    generic <typename T>
    static T NonNullable(JObject^ jObject)
    {
        T obj = jObject->ToObject<T>();
        if (obj == nullptr) throw gcnew JsonException("invalid JSON");
        return obj;
    }
};

public ref struct ExtendedSettings {
    DriverSettings^ baseSettings;
    Vec2<LutBase^> tables;
    
    using JSON = String^;

    ExtendedSettings(DriverSettings^ driverSettings) :
        baseSettings(driverSettings) {}

    ExtendedSettings() :
        ExtendedSettings(gcnew DriverSettings()) {}

    ExtendedSettings(JSON settingsJson) :
        ExtendedSettings(settingsJson, nullptr, nullptr, false) {}

    ExtendedSettings(JSON settingsJson, JSON tableJson) :
        ExtendedSettings(settingsJson, tableJson, nullptr, false) {}

    ExtendedSettings(JSON settingsJson, JSON xTableJson, JSON yTableJson) :
        ExtendedSettings(settingsJson, xTableJson, yTableJson, true) {}

private:
    ExtendedSettings(JSON settingsJson, JSON xTableJson, JSON yTableJson, bool byComponent)
    {
        if (settingsJson) {
            baseSettings = RaConvert::Settings(settingsJson);
        }
        else {
            baseSettings = gcnew DriverSettings();
        }

        if (xTableJson || yTableJson) {
            baseSettings->combineMagnitudes = !byComponent;
        }

        if (xTableJson) {
            tables.x = RaConvert::Table(xTableJson);
            tables.x->SetArgs(baseSettings->args.x.lutArgs);
        }

        if (yTableJson) {
            if (Object::ReferenceEquals(yTableJson, xTableJson)) {
                tables.y = tables.x;
            }
            else {
                tables.y = RaConvert::Table(yTableJson);
            }

            tables.y->SetArgs(baseSettings->args.y.lutArgs);
        }
    }

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
        list->Add(gcnew String(msg));
    }

    SettingsErrors(ExtendedSettings^ settings) :
        SettingsErrors(settings->baseSettings) {}

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

struct instance_t {
    ra::io_t data;
    vec2<ra::accel_invoker> inv;
};

public ref class ManagedAccel
{
    instance_t* const instance = new instance_t();

public:
    ManagedAccel() {}

    ManagedAccel(ExtendedSettings^ settings)
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

    property ExtendedSettings^ Settings
    {
        ExtendedSettings^ get()
        {
            auto settings = gcnew ExtendedSettings();
            Marshal::PtrToStructure(IntPtr(&instance->data.args), settings->baseSettings);
            settings->tables.x = extract(instance->data.args.argsv.x.lut_args.mode, 
                instance->data.mod.accels.x);
            settings->tables.y = extract(instance->data.args.argsv.y.lut_args.mode,
                instance->data.mod.accels.y);
            return settings;
        }

        void set(ExtendedSettings^ val)
        {
            Marshal::StructureToPtr(val->baseSettings, IntPtr(&instance->data.args), false);
            instance->data.mod = { instance->data.args };
            instance->inv = ra::invokers(instance->data.args);

            if (val->tables.x) {
                val->tables.x->SetData(instance->data.mod.accels.x);
            }

            if (val->tables.y) {
                val->tables.y->SetData(instance->data.mod.accels.y);
            }   
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

private:
    LutBase^ extract(ra::table_mode mode, ra::accel_union& au)
    {
        switch (mode) {
        case ra::table_mode::off: return nullptr;
        case ra::table_mode::linear: return gcnew LinearLut(au.lin_lut);
        case ra::table_mode::binlog: return gcnew BinLogLut(au.log_lut);
        case ra::table_mode::arbitrary:
        default: throw gcnew NotImplementedException();
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

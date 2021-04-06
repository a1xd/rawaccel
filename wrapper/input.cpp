#include "interop-exception.h"

#include <utility-rawinput.hpp>
#include <algorithm>
#include <msclr\marshal_cppstd.h>

using namespace System;
using namespace System::Collections::Generic;

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
            throw gcnew RawInputInteropException(e);
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
            throw gcnew RawInputInteropException(e);
        }
    }

};


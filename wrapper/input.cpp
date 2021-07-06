#include "input.h"
#include "interop-exception.h"

#include <msclr\marshal_cppstd.h>
#include <algorithm>

using namespace System;
using namespace System::Collections::Generic;

std::vector<HANDLE> rawinput_handles_from_id(const std::wstring& device_id)
{
    std::vector<HANDLE> handles;

    rawinput_foreach([&](const auto& dev) {
        if (dev.id == device_id) handles.push_back(dev.handle);
    });

    return handles;
}

std::vector<std::wstring> rawinput_id_list()
{
    std::vector<std::wstring> ids;

    rawinput_foreach([&](const auto& dev) {
        ids.push_back(dev.id);
    });

    std::sort(ids.begin(), ids.end());
    ids.erase(std::unique(ids.begin(), ids.end()), ids.end());
    return ids;
}

public ref struct RawInputInteropException : InteropException {
    RawInputInteropException(System::String^ what) :
        InteropException(what) {}
    RawInputInteropException(const char* what) :
        InteropException(what) {}
    RawInputInteropException(const std::exception& e) :
        InteropException(e) {}
};

public ref struct RawInputInterop
{
    static void AddHandlesFromID(String^ deviceID, List<IntPtr>^ rawInputHandles)
    {
        try
        {
            std::vector<HANDLE> nativeHandles = rawinput_handles_from_id(
                msclr::interop::marshal_as<std::wstring>(deviceID));

            for (auto nh : nativeHandles) rawInputHandles->Add(IntPtr(nh));
        }
        catch (const std::exception& e)
        {
            throw gcnew RawInputInteropException(e);
        }
    }

    static List<String^>^ GetDeviceIDs()
    {
        try
        {
            auto ids = gcnew List<String^>();

            for (auto&& name : rawinput_id_list())
            {
                ids->Add(msclr::interop::marshal_as<String^>(name));
            }

            return ids;
        }
        catch (const std::exception& e)
        {
            throw gcnew RawInputInteropException(e);
        }
    }

};

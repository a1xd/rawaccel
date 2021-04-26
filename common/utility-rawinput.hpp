#pragma once

#pragma comment(lib, "cfgmgr32.lib")

#include <string>
#include <system_error>
#include <vector>

#include <Windows.h>
#include <cfgmgr32.h>
#include <initguid.h> // needed for devpkey.h to parse properly
#include <devpkey.h>

// Returns an empty string on failure
// 
// interface names from GetRawInputDeviceInfo are not guaranteed to be valid;
// CM_Get_Device_Interface_PropertyW can return CR_NO_SUCH_DEVICE_INTERFACE
std::wstring dev_id_from_interface(const WCHAR* interface_name) {
    ULONG size = 0;
    DEVPROPTYPE type;
    CONFIGRET cm_res;

    cm_res = CM_Get_Device_Interface_PropertyW(interface_name, &DEVPKEY_Device_InstanceId,
        &type, NULL, &size, 0);

    if (cm_res != CR_BUFFER_SMALL && cm_res != CR_SUCCESS) return {};

    std::wstring id((size + 1) / 2, L'\0');

    cm_res = CM_Get_Device_Interface_PropertyW(interface_name, &DEVPKEY_Device_InstanceId,
        &type, reinterpret_cast<PBYTE>(&id[0]), &size, 0);

    if (cm_res != CR_SUCCESS) return {};

    auto instance_delim_pos = id.find_last_of('\\');

    if (instance_delim_pos != std::string::npos) {
        id.resize(instance_delim_pos);
    }

    return id;
}

template <typename Func>
void rawinput_foreach_with_interface(Func fn, DWORD input_type = RIM_TYPEMOUSE) {
    const UINT RI_ERROR = -1;

    UINT num_devs = 0;

    if (GetRawInputDeviceList(NULL, &num_devs, sizeof(RAWINPUTDEVICELIST)) == RI_ERROR) {
        throw std::system_error(GetLastError(), std::system_category(), "GetRawInputDeviceList failed");
    }

    auto devs = std::vector<RAWINPUTDEVICELIST>(num_devs);

    if (GetRawInputDeviceList(&devs[0], &num_devs, sizeof(RAWINPUTDEVICELIST)) == RI_ERROR) {
        throw std::system_error(GetLastError(), std::system_category(), "GetRawInputDeviceList failed");
    }

    for (auto&& dev : devs) {
        if (dev.dwType != input_type) continue;

        WCHAR name[256] = {};
        UINT name_size = sizeof(name);

        if (GetRawInputDeviceInfoW(dev.hDevice, RIDI_DEVICENAME, name, &name_size) == RI_ERROR) {
            throw std::system_error(GetLastError(), std::system_category(), "GetRawInputDeviceInfoW failed");
        }

        fn(dev, name);
    }
}

// returns device handles corresponding to a "device id"
// https://docs.microsoft.com/en-us/windows-hardware/drivers/install/device-ids
std::vector<HANDLE> rawinput_handles_from_dev_id(const std::wstring& device_id, DWORD input_type = RIM_TYPEMOUSE) {
    std::vector<HANDLE> handles;

    rawinput_foreach_with_interface([&](const auto& dev, const WCHAR* name) {
        auto id = dev_id_from_interface(name);
        if (!id.empty() && id == device_id) {
            handles.push_back(dev.hDevice);
        } 
    }, input_type);

    return handles;
}

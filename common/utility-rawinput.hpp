#pragma once

#pragma comment(lib, "cfgmgr32.lib")

#include <string>
#include <system_error>
#include <vector>

#include <Windows.h>
#include <cfgmgr32.h>
#include <initguid.h> // needed for devpkey.h to parse properly
#include <devpkey.h>

std::wstring dev_prop_wstr_from_interface(const WCHAR* interface_name, const DEVPROPKEY* key) {
    ULONG size = 0;
    DEVPROPTYPE type;
    CONFIGRET cm_res;

    cm_res = CM_Get_Device_Interface_PropertyW(interface_name, key,
        &type, NULL, &size, 0);

    if (cm_res != CR_BUFFER_SMALL && cm_res != CR_SUCCESS) {
        throw std::runtime_error("CM_Get_Device_Interface_PropertyW failed (" +
            std::to_string(cm_res) + ')');
    }

    std::wstring prop((size + 1) / 2, L'\0');

    cm_res = CM_Get_Device_Interface_PropertyW(interface_name, key,
        &type, reinterpret_cast<PBYTE>(&prop[0]), &size, 0);

    if (cm_res != CR_SUCCESS) {
        throw std::runtime_error("CM_Get_Device_Interface_PropertyW failed (" +
            std::to_string(cm_res) + ')');
    }

    return prop;
}

std::wstring dev_id_from_interface(const WCHAR* interface_name) {
    auto id = dev_prop_wstr_from_interface(interface_name, &DEVPKEY_Device_InstanceId);
    id.resize(id.find_last_of('\\'));
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
        if (device_id == dev_id_from_interface(name)) {
            handles.push_back(dev.hDevice);
        } 
    }, input_type);

    return handles;
}

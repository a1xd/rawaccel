#pragma once

#pragma comment(lib, "cfgmgr32.lib")

#include <string>
#include <system_error>
#include <vector>

#include <Windows.h>
#include <cfgmgr32.h>
#include <initguid.h> // needed for devpkey.h to parse properly
#include <devpkey.h>

struct rawinput_device {
    HANDLE handle;
    std::wstring id;
};

template <typename Func>
void rawinput_foreach(Func fn, DWORD device_type = RIM_TYPEMOUSE)
{
    const UINT RI_ERROR = -1;

    // get number of devices
    UINT num_devs = 0;
    if (GetRawInputDeviceList(NULL, &num_devs, sizeof(RAWINPUTDEVICELIST)) != 0) {
        throw std::system_error(GetLastError(), std::system_category(), "GetRawInputDeviceList failed");
    }

    auto dev_list = std::vector<RAWINPUTDEVICELIST>(num_devs);

    if (GetRawInputDeviceList(&dev_list[0], &num_devs, sizeof(RAWINPUTDEVICELIST)) == RI_ERROR) {
        return;
    }

    std::wstring name;
    rawinput_device dev;
    DEVPROPTYPE prop_type;
    CONFIGRET cm_res;

    for (auto [handle, dev_type] : dev_list) {
        if (dev_type != device_type) continue;

        // get interface name length
        UINT name_len = 0;
        if (GetRawInputDeviceInfoW(handle, RIDI_DEVICENAME, NULL, &name_len) == RI_ERROR) {
            continue;
        }

        name.resize(name_len);

        if (GetRawInputDeviceInfoW(handle, RIDI_DEVICENAME, &name[0], &name_len) == RI_ERROR) {
            continue;
        }

        // get sizeof device instance id
        ULONG id_size = 0;
        cm_res = CM_Get_Device_Interface_PropertyW(&name[0], &DEVPKEY_Device_InstanceId,
            &prop_type, NULL, &id_size, 0);

        if (cm_res != CR_BUFFER_SMALL && cm_res != CR_SUCCESS) continue;

        dev.id.resize((id_size + 1) / 2);

        cm_res = CM_Get_Device_Interface_PropertyW(&name[0], &DEVPKEY_Device_InstanceId,
            &prop_type, reinterpret_cast<PBYTE>(&dev.id[0]), &id_size, 0);

        if (cm_res != CR_SUCCESS) continue;

        // pop instance id
        auto instance_delim_pos = dev.id.find_last_of('\\');
        if (instance_delim_pos != std::string::npos) dev.id.resize(instance_delim_pos);

        dev.handle = handle;

        fn(dev);
    }
}

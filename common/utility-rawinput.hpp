#pragma once

#pragma comment(lib, "cfgmgr32.lib")

#include <algorithm>
#include <string>
#include <system_error>
#include <vector>

#include <Windows.h>
#include <cfgmgr32.h>
#include <initguid.h> // needed for devpkey.h to parse properly
#include <devpkey.h>

template <typename Func>
void rawinput_foreach_dev_with_id(Func fn, bool with_instance_id = false, 
                                           DWORD input_type = RIM_TYPEMOUSE) 
{
    const UINT RI_ERROR = -1;
    
    // get number of devices
    UINT num_devs = 0;
    if (GetRawInputDeviceList(NULL, &num_devs, sizeof(RAWINPUTDEVICELIST)) != 0) {
        throw std::system_error(GetLastError(), std::system_category(), "GetRawInputDeviceList failed");
    }

    auto devs = std::vector<RAWINPUTDEVICELIST>(num_devs);

    if (GetRawInputDeviceList(&devs[0], &num_devs, sizeof(RAWINPUTDEVICELIST)) == RI_ERROR) {
        return;
    }

    std::wstring name;
    std::wstring id;
    DEVPROPTYPE type;
    CONFIGRET cm_res;

    for (auto&& dev : devs) {
        if (dev.dwType != input_type) continue;

        // get interface name length
        UINT name_len = 0;
        if (GetRawInputDeviceInfoW(dev.hDevice, RIDI_DEVICENAME, NULL, &name_len) == RI_ERROR) {
            continue;
        }

        name.resize(name_len);

        if (GetRawInputDeviceInfoW(dev.hDevice, RIDI_DEVICENAME, &name[0], &name_len) == RI_ERROR) {
            continue;
        }

        // get sizeof dev instance id
        ULONG id_size = 0;
        cm_res = CM_Get_Device_Interface_PropertyW(&name[0], &DEVPKEY_Device_InstanceId,
            &type, NULL, &id_size, 0);

        if (cm_res != CR_BUFFER_SMALL && cm_res != CR_SUCCESS) continue;

        id.resize((id_size + 1) / 2);

        cm_res = CM_Get_Device_Interface_PropertyW(&name[0], &DEVPKEY_Device_InstanceId,
            &type, reinterpret_cast<PBYTE>(&id[0]), &id_size, 0);

        if (cm_res != CR_SUCCESS) continue;
    
        if (!with_instance_id) {
            auto instance_delim_pos = id.find_last_of('\\');
            if(instance_delim_pos != std::string::npos) id.resize(instance_delim_pos);
        }

        fn(dev, id);
    }
}

inline
std::vector<HANDLE> rawinput_handles_from_dev_id(const std::wstring& device_id, 
                                                 bool with_instance_id = false,
                                                 DWORD input_type = RIM_TYPEMOUSE) 
{
    std::vector<HANDLE> handles;

    rawinput_foreach_dev_with_id([&](const auto& dev, const std::wstring& id) {
        if (id == device_id) handles.push_back(dev.hDevice);
    }, with_instance_id, input_type);

    return handles;
}

inline
std::vector<std::wstring> rawinput_dev_id_list(bool with_instance_id = false, 
                                               DWORD input_type = RIM_TYPEMOUSE) 
{
    std::vector<std::wstring> ids;

    rawinput_foreach_dev_with_id([&](const auto& dev, const std::wstring& id) {
        ids.push_back(id);
    }, with_instance_id, input_type);

    std::sort(ids.begin(), ids.end());
    ids.erase(std::unique(ids.begin(), ids.end()), ids.end());
    return ids;
}

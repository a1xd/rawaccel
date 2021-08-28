#pragma once

#pragma comment(lib, "cfgmgr32.lib")
#pragma comment(lib, "hid.lib")
#pragma comment(lib, "User32.lib")

#include <string_view>
#include <vector>

#include <Windows.h>
#include <cfgmgr32.h>
#include <initguid.h> // needed for devpkey.h to parse properly
#include <devpkey.h>
#include <hidsdi.h>

inline constexpr size_t MAX_DEV_ID_LEN = 200;
inline constexpr size_t MAX_NAME_LEN = 256;
inline constexpr UINT RI_ERROR = -1;

struct rawinput_device {
    HANDLE handle = nullptr;        // rawinput handle
    WCHAR name[MAX_NAME_LEN] = {};  // manufacturer + product
    WCHAR id[MAX_DEV_ID_LEN] = {};  // hwid formatted device id
};

template <typename Func>
void rawinput_foreach(Func fn, DWORD device_type = RIM_TYPEMOUSE)
{
    const size_t HID_STR_MAX_LEN = 127;

    auto starts_with = [](auto&& a, auto&& b) {
        return b.size() <= a.size() && std::equal(b.begin(), b.end(), a.begin());
    };

    auto get_dev_list = []() -> std::vector<RAWINPUTDEVICELIST> {
        UINT elem_size = sizeof(RAWINPUTDEVICELIST);
        UINT num_devs = 0;
        
        if (GetRawInputDeviceList(NULL, &num_devs, elem_size) == 0) {
            auto dev_list = std::vector<RAWINPUTDEVICELIST>(num_devs);

            if (GetRawInputDeviceList(&dev_list[0], &num_devs, elem_size) != RI_ERROR) {
                return dev_list;
            }
        }

        return {};
    };

    std::wstring interface_name;
    rawinput_device dev;
    DEVPROPTYPE prop_type;
    CONFIGRET cm_res;
    WCHAR product_str_buf[HID_STR_MAX_LEN] = {};

    for (auto [handle, dev_type] : get_dev_list()) {
        if (dev_type != device_type) continue;

        dev.handle = handle;

        // get interface name
        UINT name_len = 0;
        if (GetRawInputDeviceInfoW(handle, RIDI_DEVICENAME, NULL, &name_len) == RI_ERROR) {
            continue;
        }

        interface_name.resize(name_len);

        if (GetRawInputDeviceInfoW(handle, RIDI_DEVICENAME, &interface_name[0], &name_len) == RI_ERROR) {
            continue;
        }

        // make name from vendor + product
        HANDLE hid_dev_object = CreateFileW(
            &interface_name[0], 0, FILE_SHARE_READ, 0, OPEN_EXISTING, 0, 0);

        if (hid_dev_object != INVALID_HANDLE_VALUE) {

            if (HidD_GetProductString(hid_dev_object, product_str_buf, HID_STR_MAX_LEN)) {
                auto product_sv = std::wstring_view(product_str_buf);

                if (HidD_GetManufacturerString(hid_dev_object, dev.name, HID_STR_MAX_LEN)) {
                    auto manufacturer_sv = std::wstring_view(dev.name);

                    if (starts_with(product_sv, manufacturer_sv)) {
                        wcsncpy_s(dev.name, product_str_buf, HID_STR_MAX_LEN);
                    }
                    else {
                        auto last = manufacturer_sv.size();
                        dev.name[last] = L' ';
                        wcsncpy_s(dev.name + last + 1, HID_STR_MAX_LEN, product_str_buf, HID_STR_MAX_LEN);
                    }
                }
                else {
                    wcsncpy_s(dev.name, product_str_buf, HID_STR_MAX_LEN);
                }
            }
            else {
                dev.name[0] = L'\0';
            }

            CloseHandle(hid_dev_object);
        }
        else {
            dev.name[0] = L'\0';
        }

        // get device instance id
        ULONG id_size = 0;
        cm_res = CM_Get_Device_Interface_PropertyW(&interface_name[0], &DEVPKEY_Device_InstanceId,
            &prop_type, NULL, &id_size, 0);

        if (cm_res != CR_BUFFER_SMALL && cm_res != CR_SUCCESS) continue;

        cm_res = CM_Get_Device_Interface_PropertyW(&interface_name[0], &DEVPKEY_Device_InstanceId,
            &prop_type, reinterpret_cast<PBYTE>(&dev.id[0]), &id_size, 0);

        if (cm_res != CR_SUCCESS) continue;

        // pop instance id
        auto instance_delim_pos = std::wstring_view(dev.id).find_last_of(L'\\');
        if (instance_delim_pos != std::string::npos) {
            dev.id[instance_delim_pos] = L'\0';
        }

        fn(dev);
    }
}

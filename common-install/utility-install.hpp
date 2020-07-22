#pragma once

#include <filesystem>
#include <Windows.h>

#include "external/WinReg.hpp"

namespace fs = std::filesystem;
namespace wr = winreg;

inline const std::wstring DRIVER_NAME = L"rawaccel";
inline const std::wstring DRIVER_FILE_NAME = DRIVER_NAME + L".sys";

fs::path get_sys_path() {
    std::wstring path;
    path.resize(MAX_PATH);

    UINT chars_copied = GetSystemDirectoryW(path.data(), MAX_PATH);
    if (chars_copied == 0) throw std::runtime_error("GetSystemDirectory failed");

    path.resize(chars_copied);
    return path;
}

fs::path get_target_path() {
    return get_sys_path() / L"drivers" / DRIVER_FILE_NAME;
}

fs::path make_temp_path(const fs::path& p) {
    auto tmp_path = p;
    tmp_path.concat(".tmp");
    return tmp_path;
}

template<typename Func>
void modify_upper_filters(Func fn) {
    const std::wstring FILTERS_NAME = L"UpperFilters";
    wr::RegKey key(
        HKEY_LOCAL_MACHINE,
        L"SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e96f-e325-11ce-bfc1-08002be10318}"
    );
 
    std::vector<std::wstring> filters = key.GetMultiStringValue(FILTERS_NAME);
    fn(filters);
    key.SetMultiStringValue(FILTERS_NAME, filters);
}

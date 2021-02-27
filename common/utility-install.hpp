#pragma once

#include <filesystem>
#include <Windows.h>

#include "external/WinReg.hpp"

namespace fs = std::filesystem;
namespace wr = winreg;

inline const std::wstring DRIVER_NAME = L"rawaccel";
inline const std::wstring DRIVER_FILE_NAME = DRIVER_NAME + L".sys";
inline const std::wstring DRIVER_ENV_PATH = L"%systemroot%\\system32\\drivers\\" + DRIVER_FILE_NAME;

inline const auto sys_error = [](auto what, DWORD code = GetLastError()) {
    return std::system_error(code, std::system_category(), what);
};

inline std::wstring expand(const std::wstring& env_path) {
    std::wstring path(MAX_PATH, L'\0');

    auto len = ExpandEnvironmentStringsW(env_path.c_str(), &path[0], MAX_PATH);
    if (len == 0) throw sys_error("ExpandEnvironmentStrings failed");
    path.resize(len - 1);
    return path;
}

inline fs::path make_temp_path(const fs::path& p) {
    auto tmp_path = p;
    tmp_path.concat(L".tmp");
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

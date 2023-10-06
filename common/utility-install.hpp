#pragma once

#pragma comment( lib, "setupapi" )

#include <filesystem>
#include <string_view>

#include <windows.h>
#include <initguid.h>
#include <setupapi.h>
#include <Devguid.h>
#include <tchar.h>

#define DRV_NAME L"rawaccel"
#define DRV_FILE_NAME DRV_NAME L".sys"
#define DRV_SRC_PATH L"driver\\" DRV_FILE_NAME
#define DRV_DST_PATH L"%systemroot%\\system32\\drivers\\" DRV_FILE_NAME

namespace fs = std::filesystem;

template <typename String>
std::system_error sys_error(String what, DWORD code = GetLastError())
{
    return std::system_error(code, std::system_category(), what);
}

inline std::wstring expand(const wchar_t* env_path)
{
    std::wstring path(MAX_PATH, L'\0');

    auto len = ExpandEnvironmentStringsW(env_path, &path[0], MAX_PATH);
    if (len == 0) throw sys_error("ExpandEnvironmentStrings failed");
    path.resize(len - 1);
    return path;
}

inline fs::path make_temp_path(const fs::path& p)
{
    auto tmp_path = p;
    tmp_path.concat(L".tmp");
    return tmp_path;
}

/*
 * returns the length (in characters) of the buffer required to hold this
 * MultiSz, INCLUDING the trailing null.
 *
 * example: MultiSzLength("foo\0bar\0") returns 9
 *
 * note: since MultiSz cannot be null, a number >= 1 will always be returned
 *
 * parameters:
 *   MultiSz - the MultiSz to get the length of
 */
inline size_t MultiSzLength(_In_ IN LPTSTR MultiSz)
{
    size_t len = 0;
    size_t totalLen = 0;

    // search for trailing null character
    while (*MultiSz != _T('\0'))
    {
        len = _tcslen(MultiSz) + 1;
        MultiSz += len;
        totalLen += len;
    }

    // add one for the trailing null character
    return (totalLen + 1);
}


/*
 * Deletes all instances of a string from within a multi-sz.
 *
 * parameters:
 *   FindThis        - the string to find and remove
 *   FindWithin      - the string having the instances removed
 *   NewStringLength - the new string length
 */
inline size_t MultiSzSearchAndDeleteCaseInsensitive(_In_ IN  LPCTSTR FindThis,
    _In_ IN  LPTSTR FindWithin,
    OUT size_t* NewLength)
{
    LPTSTR search;
    size_t currentOffset;
    DWORD  instancesDeleted;
    size_t searchLen;

    currentOffset = 0;
    instancesDeleted = 0;
    search = FindWithin;

    *NewLength = MultiSzLength(FindWithin);

    // loop while the multisz null terminator is not found
    while (*search != L'\0')
    {
        // length of string + null char; used in more than a couple places
        searchLen = _tcslen(search) + 1;

        // if this string matches the current one in the multisz...
        if (_tcsicmp(search, FindThis) == 0)
        {
            // they match, shift the contents of the multisz, to overwrite the
            // string (and terminating null), and update the length
            instancesDeleted++;
            *NewLength -= searchLen;
            memmove(search,
                search + searchLen,
                (*NewLength - currentOffset) * sizeof(TCHAR));
        }
        else
        {
            // they don't mactch, so move pointers, increment counters
            currentOffset += searchLen;
            search += searchLen;
        }
    }

    return (instancesDeleted);
}

template <bool Install = 1>
bool set_registry_filter()
{
    PBYTE buffer = NULL;
    DWORD size = 0;
    size_t len = 0;

    SetupDiGetClassRegistryPropertyW(
        &GUID_DEVCLASS_MOUSE,
        SPCRP_UPPERFILTERS,
        NULL,
        buffer,
        size,
        &size,
        NULL,
        NULL);

    buffer = new BYTE[size];

    if (!SetupDiGetClassRegistryPropertyW(
        &GUID_DEVCLASS_MOUSE,
        SPCRP_UPPERFILTERS,
        NULL,
        buffer,
        size,
        NULL,
        NULL,
        NULL)) {
        throw sys_error("SetupDiGetClassRegistryProperty failed");
    }

    auto found = MultiSzSearchAndDeleteCaseInsensitive(DRV_NAME, reinterpret_cast<LPWSTR>(buffer), &len);

    if constexpr (Install) {
        const auto filter_len = wcslen(DRV_NAME) + 1;
        LPWSTR tmp = new WCHAR[len + filter_len];
        memcpy(tmp, DRV_NAME, filter_len * sizeof(WCHAR));
        memcpy(tmp + filter_len, buffer, len * sizeof(WCHAR));
        delete[] buffer;
        buffer = reinterpret_cast<PBYTE>(tmp);
        len += filter_len;
    }

    if (!SetupDiSetClassRegistryPropertyW(
        &GUID_DEVCLASS_MOUSE,
        SPCRP_UPPERFILTERS,
        buffer,
        static_cast<DWORD>(len * sizeof(WCHAR)),
        NULL,
        NULL)) {
        throw sys_error("SetupDiSetClassRegistryProperty failed");
    }

    return found;
}

#pragma once

#include "rawaccel-io-def.h"
#include "rawaccel-version.h"
#include "rawaccel-error.hpp"
#include "rawaccel.hpp"

namespace rawaccel {

    inline void io_control(DWORD code, void* in, DWORD in_size, void* out, DWORD out_size) 
    {
        HANDLE ra_handle = INVALID_HANDLE_VALUE;

        ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

        if (ra_handle == INVALID_HANDLE_VALUE) {
            throw install_error();
        }

        DWORD dummy;

        BOOL success = DeviceIoControl(
            ra_handle,
            code,
            in,
            in_size,
            out,
            out_size,
            &dummy,  // bytes returned
            NULL     // overlapped structure
        );

        CloseHandle(ra_handle);

        if (!success) {
            throw sys_error("DeviceIoControl failed");
        }
    }

    inline void read(io_t& args)
    {
        io_control(READ, NULL, 0, &args, sizeof(io_t));
    }

    inline void write(const io_t& args) 
    {
        io_control(WRITE, const_cast<io_t*>(&args), sizeof(io_t), NULL, 0);
    }

    inline version_t get_version() 
    {
        version_t v;

        try {
            io_control(GET_VERSION, NULL, 0, &v, sizeof(version_t));
        }
        catch (const sys_error&) {
            // assume request is not implemented (< 1.3)
            v = { 0 }; 
        }

        return v;
    }

    inline version_t valid_version_or_throw()
    {
        auto v = get_version();

        if (v < min_driver_version) {
            throw error("reinstallation required");
        }

        if (version < v) {
            throw error("newer driver is installed");
        }

        return v;
    }

}

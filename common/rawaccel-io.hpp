#pragma once

#include <system_error>

#define NOMINMAX
#include <Windows.h>

#include "rawaccel-io-def.h"
#include "rawaccel-settings.h"
#include "rawaccel-version.h"
#include "rawaccel-error.hpp"

#pragma warning(push)
#pragma warning(disable:4245) // int -> DWORD conversion while passing CTL_CODE

namespace rawaccel {

	void io_control(DWORD code, void* in, DWORD in_size, void* out, DWORD out_size) {
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
			throw std::system_error(GetLastError(), std::system_category(), "DeviceIoControl failed");
		}
	}

	settings read() {
		settings args;
		io_control(RA_READ, NULL, 0, &args, sizeof(settings));
		return args;
	}


	void write(const settings& args) {
		auto in_ptr = const_cast<settings*>(&args);
		io_control(RA_WRITE, in_ptr, sizeof(settings), NULL, 0);
	}

	version_t get_version() {
		version_t ver;
		io_control(RA_GET_VERSION, NULL, 0, &ver, sizeof(version_t));
		return ver;
	}

}

#pragma warning(pop)

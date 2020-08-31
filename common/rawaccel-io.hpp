#pragma once

#include <system_error>

#define NOMINMAX
#include <Windows.h>

#include "rawaccel-settings.h"
#include "rawaccel-error.hpp"

#define RA_READ CTL_CODE(0x8888, 0x888, METHOD_OUT_DIRECT, FILE_ANY_ACCESS)
#define RA_WRITE CTL_CODE(0x8888, 0x889, METHOD_BUFFERED, FILE_ANY_ACCESS)

#pragma warning(push)
#pragma warning(disable:4245) // int -> DWORD conversion while passing CTL_CODE

namespace rawaccel {

	settings read() {
		HANDLE ra_handle = INVALID_HANDLE_VALUE;

		ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

		if (ra_handle == INVALID_HANDLE_VALUE) {
			throw install_error();
		}

		settings args;
		DWORD dummy;

		BOOL success = DeviceIoControl(
			ra_handle,
			RA_READ,
			NULL,					  // input buffer
			0,                        // input buffer size
			&args,                    // output buffer
			sizeof(settings),         // output buffer size
			&dummy,                   // bytes returned
			NULL                      // overlapped structure
		);

		CloseHandle(ra_handle);

		if (!success) {
			throw std::system_error(GetLastError(), std::system_category(), "DeviceIoControl failed");
		}

		return args;
	}


	void write(const settings& args) {
		HANDLE ra_handle = INVALID_HANDLE_VALUE;

		ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

		if (ra_handle == INVALID_HANDLE_VALUE) {
			throw install_error();
		}

		DWORD dummy;

		BOOL success = DeviceIoControl(
			ra_handle,
			RA_WRITE,
			const_cast<settings*>(&args),      // input buffer
			sizeof(settings),                  // input buffer size
			NULL,                              // output buffer
			0,                                 // output buffer size
			&dummy,                            // bytes returned
			NULL                               // overlapped structure
		);

		CloseHandle(ra_handle);

		if (!success) {
			if (auto err = GetLastError(); err != ERROR_BUSY) {
				throw std::system_error(err, std::system_category(), "DeviceIoControl failed");
			}
			throw cooldown_error();
		}
	}

}

#pragma warning(pop)

#pragma once

#include <system_error>

#define NOMINMAX
#include <Windows.h>

#include "rawaccel.hpp"

#define RA_READ CTL_CODE(0x8888, 0x888, METHOD_OUT_DIRECT, FILE_ANY_ACCESS)
#define RA_WRITE CTL_CODE(0x8888, 0x889, METHOD_BUFFERED, FILE_ANY_ACCESS)

#pragma warning(push)
#pragma warning(disable:4245) // int -> DWORD conversion while passing CTL_CODE

namespace rawaccel {

	mouse_modifier read() {
		HANDLE ra_handle = INVALID_HANDLE_VALUE;

		ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

		if (ra_handle == INVALID_HANDLE_VALUE) {
			throw std::system_error(GetLastError(), std::system_category(), "CreateFile failed");
		}

		mouse_modifier mod;
		DWORD dummy;

		BOOL success = DeviceIoControl(
			ra_handle,
			RA_READ,
			NULL,					  // input buffer
			0,                        // input buffer size
			&mod,                     // output buffer
			sizeof(mouse_modifier),   // output buffer size
			&dummy,                   // bytes returned
			NULL                      // overlapped structure
		);

		CloseHandle(ra_handle);

		if (!success) {
			throw std::system_error(GetLastError(), std::system_category(), "DeviceIoControl failed");
		}

		return mod;
	}


	void write(mouse_modifier mod) {
		HANDLE ra_handle = INVALID_HANDLE_VALUE;

		ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

		if (ra_handle == INVALID_HANDLE_VALUE) {
			throw std::system_error(GetLastError(), std::system_category(), "CreateFile failed");
		}

		DWORD dummy;

		BOOL success = DeviceIoControl(
			ra_handle,
			RA_WRITE,
			&mod,                     // input buffer
			sizeof(mouse_modifier),   // input buffer size
			NULL,                     // output buffer
			0,                        // output buffer size
			&dummy,                   // bytes returned
			NULL                      // overlapped structure
		);

		CloseHandle(ra_handle);

		if (!success) {
			throw std::system_error(GetLastError(), std::system_category(), "DeviceIoControl failed");
		}
	}

}

#pragma warning(pop)

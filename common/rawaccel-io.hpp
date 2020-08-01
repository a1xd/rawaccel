#pragma once

#include <system_error>

#define NOMINMAX
#include <Windows.h>

#include "rawaccel.hpp"

#define RA_IOCTL CTL_CODE(0x8888, 0x888, METHOD_BUFFERED, FILE_ANY_ACCESS)

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
			RA_IOCTL,
			NULL,					  // input buffer
			0,						  // input buffer size
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

}

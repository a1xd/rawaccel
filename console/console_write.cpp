#pragma once

#include "console_write.hpp"

void write(ra::mouse_modifier vars) {
	HANDLE ra_handle = INVALID_HANDLE_VALUE;

	ra_handle = CreateFileW(L"\\\\.\\rawaccel", 0, 0, 0, OPEN_EXISTING, 0, 0);

	if (ra_handle == INVALID_HANDLE_VALUE) {
		throw std::system_error(GetLastError(), std::system_category(), "CreateFile failed");
	}

	DWORD dummy;

	BOOL success = DeviceIoControl(
		ra_handle,               
		RA_WRITE,
		&vars,    
		sizeof(ra::mouse_modifier),      
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

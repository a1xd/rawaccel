#include <iostream>

#define NOMINMAX
#include <Windows.h>

#include <rawaccel-userspace.hpp>

#define RA_WRITE CTL_CODE(0x8888, 0x888, METHOD_BUFFERED, FILE_ANY_ACCESS)

namespace ra = rawaccel;

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

int main(int argc, char** argv) {
	try {
		write(ra::parse(argc, argv));
	}
	catch (std::domain_error e) {
		std::cerr << e.what() << '\n';
		return ra::INVALID_ARGUMENT;
	}
	catch (std::system_error e) {
		std::cerr << "Error: " << e.what() << " (" << e.code() << ")\n";
		return ra::SYSTEM_ERROR;
	}
}

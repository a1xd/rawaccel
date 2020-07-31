#include <iostream>

#define NOMINMAX
#include <Windows.h>

#include <rawaccel-userspace.hpp>
#include "console_write.hpp"

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

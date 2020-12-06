#pragma once

#include <stdexcept>

namespace rawaccel {

	class error : public std::runtime_error { 
		using std::runtime_error::runtime_error; 
	};

	class io_error : public error {
		using error::error;
	};

	class install_error : public io_error {
	public:
		install_error() : io_error("Raw Accel driver is not installed, run installer.exe") {}
	};

}

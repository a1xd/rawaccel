#pragma once

#include <stdexcept>

namespace rawaccel {

	class error : public std::runtime_error { 
		using std::runtime_error::runtime_error; 
	};

	class invalid_argument : public error {
		using error::error; 
	};

	class io_error : public error {
		using error::error;
	};

	class install_error : public io_error {
	public:
		install_error() : io_error("rawaccel is not installed") {}
	};

	class cooldown_error : public io_error {
	public:
		cooldown_error() : io_error("write is on cooldown") {}
	};

}

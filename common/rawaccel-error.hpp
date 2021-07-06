#pragma once

#include <system_error>
#include <string>

namespace rawaccel {

	class error : public std::runtime_error {
		using std::runtime_error::runtime_error;
	};

	class io_error : public error {
		using error::error;
	};

	class install_error : public io_error {
	public:
		install_error() :
			io_error("Raw Accel is not installed, run installer.exe") {}
	};

	class sys_error : public io_error {
	public:
		sys_error(const char* msg, DWORD code = GetLastError()) :
			io_error(build_msg(code, msg)) {}

		static std::string build_msg(DWORD code, const char* msg) 
		{
			std::string ret = 
				std::system_error(code, std::system_category(), msg).what();
			ret += " (";
			ret += std::to_string(code);
			ret += ")";
			return ret;
		}

	};

}

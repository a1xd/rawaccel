#pragma once

#include <rawaccel-error.hpp>
#include <rawaccel-settings.h>
#include <rawaccel-version.h>

using namespace rawaccel;
using namespace System;

struct wrapper_io {
	static void writeToDriver(const settings&);
	static void readFromDriver(settings&);
	static void getDriverVersion(version_t&);
};

public ref struct DriverIOException : public IO::IOException {
public:
	DriverIOException() {}
	DriverIOException(String^ what) : IO::IOException(what) {}
};

public ref struct DriverNotInstalledException : public DriverIOException {
	DriverNotInstalledException() : 
		DriverIOException(gcnew String(install_error().what())) {}
};

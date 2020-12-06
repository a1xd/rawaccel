#pragma once

#include <rawaccel-io.hpp>
#include "wrapper_io.hpp"

auto with_managed_ex = [](auto fn) {
	try
	{
		fn();
	}
	catch (const install_error&)
	{
		throw gcnew DriverNotInstalledException();
	}
	catch (const std::system_error& e)
	{
		throw gcnew DriverIOException(gcnew String(e.what()));
	}
};

void wrapper_io::writeToDriver(const settings& args)
{
	with_managed_ex([&] {
		write(args);
	});
}

void wrapper_io::readFromDriver(settings& args)
{
	with_managed_ex([&] {
		args = read();
	});
}

void wrapper_io::getDriverVersion(version_t& ver)
{
	with_managed_ex([&] {
		ver = get_version();
	});
}

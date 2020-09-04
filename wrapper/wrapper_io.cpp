#pragma once

#include <rawaccel-io.hpp>
#include "wrapper_io.hpp"

void wrapper_io::writeToDriver(const settings& args)
{
	try 
	{
		write(args);
	}
	catch (const install_error&)
	{
		throw gcnew DriverNotInstalledException();
	}
	catch (const std::system_error& e)
	{
		throw gcnew DriverIOException(gcnew String(e.what()));
	}
}

void wrapper_io::readFromDriver(settings& args)
{
	try
	{
		args = read();
	}
	catch (const install_error&)
	{
		throw gcnew DriverNotInstalledException();
	}
	catch (const std::system_error& e)
	{
		throw gcnew DriverIOException(gcnew String(e.what()));
	}
}

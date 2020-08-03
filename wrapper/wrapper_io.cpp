#pragma once

#include <rawaccel-io.hpp>
#include "wrapper_io.hpp"

void wrapper_io::writeToDriver(rawaccel::mouse_modifier* modifier)
{
	rawaccel::write(*modifier);
}

rawaccel::mouse_modifier* wrapper_io::readFromDriver()
{
	rawaccel::mouse_modifier modifier = rawaccel::read();
	return &(modifier);
}

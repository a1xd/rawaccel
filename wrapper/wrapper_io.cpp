#pragma once

#include <rawaccel-io.hpp>
#include "wrapper_io.hpp"

void wrapper_io::writeToDriver(const mouse_modifier& modifier)
{
	write(modifier);
}

void wrapper_io::readFromDriver(mouse_modifier& modifier)
{
	modifier = read();
}

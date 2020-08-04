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
	rawaccel::mouse_modifier* mod_pnt = (rawaccel::mouse_modifier*)malloc(sizeof(rawaccel::mouse_modifier));
	memcpy(mod_pnt, &modifier, sizeof(rawaccel::mouse_modifier));

	return mod_pnt;
}

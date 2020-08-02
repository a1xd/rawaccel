#pragma once

#include <rawaccel-io.hpp>
#include "wrapper_writer.hpp"

void writer::writeToDriver(rawaccel::mouse_modifier* modifier)
{
	rawaccel::write(*modifier);
}

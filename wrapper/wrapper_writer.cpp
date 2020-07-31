#pragma once

#include "..\console\console_write.cpp"
#include "wrapper_writer.hpp"

void writer::writeToDriver(rawaccel::mouse_modifier* modifier)
{
	write(*modifier);
}

#pragma once

#include <rawaccel.hpp>

struct wrapper_io {
	void writeToDriver(rawaccel::mouse_modifier* modifier);
	rawaccel::mouse_modifier* readFromDriver();
};
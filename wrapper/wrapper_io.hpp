#pragma once

#include <rawaccel.hpp>

using namespace rawaccel;

struct wrapper_io {
	static void writeToDriver(const mouse_modifier& modifier);
	static void readFromDriver(mouse_modifier& modifier);
};

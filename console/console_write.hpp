#pragma once

#include <iostream>

#define NOMINMAX
#include <Windows.h>

#include "..\common\rawaccel.hpp"

#define RA_WRITE CTL_CODE(0x8888, 0x888, METHOD_BUFFERED, FILE_ANY_ACCESS)

namespace ra = rawaccel;

void write(ra::mouse_modifier vars);
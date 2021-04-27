#pragma once

#define RA_VER_MAJOR 1
#define RA_VER_MINOR 4
#define RA_VER_PATCH 4

#define RA_OS "Win7+"

#define M_STR_HELPER(x) #x
#define M_STR(x) M_STR_HELPER(x)

#define RA_VER_STRING M_STR(RA_VER_MAJOR) "." M_STR(RA_VER_MINOR) "." M_STR(RA_VER_PATCH)

namespace rawaccel {

	struct version_t {
		int major;
		int minor;
		int patch;
	};

#ifndef _KERNEL_MODE
	inline constexpr version_t min_driver_version = { 1, 4, 0 };
#endif

}

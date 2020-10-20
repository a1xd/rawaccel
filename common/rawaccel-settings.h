#pragma once

#include "vec2.h"
#include "accel-base.hpp"

namespace rawaccel {
    using milliseconds = double;

    inline constexpr milliseconds WRITE_DELAY = 1000;
    inline constexpr milliseconds DEFAULT_TIME_MIN = 0.4;

    enum class accel_mode {
        linear, classic, natural, naturalgain, power, motivity, noaccel
    };

    struct settings {
        double degrees_rotation = 0;
        bool combine_mags = true;
        vec2<accel_mode> modes = { accel_mode::noaccel, accel_mode::noaccel };
        vec2<accel_args> argsv;
        double speed_cap = 0;
        vec2d sens = { 1, 1 };
        milliseconds time_min = DEFAULT_TIME_MIN;
    };

}

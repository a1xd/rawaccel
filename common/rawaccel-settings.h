#pragma once

#include "vec2.h"
#include "accel-base.hpp"

namespace rawaccel {
    using milliseconds = double;

    inline constexpr milliseconds WRITE_DELAY = 1000;

    enum class accel_mode {
        linear, classic, natural, naturalgain, power, logarithm, motivity, noaccel
    };

    struct settings {
        double degrees_rotation = 0;
        bool combine_mags = true;
        vec2<accel_mode> modes = { accel_mode::noaccel, accel_mode::noaccel };
        vec2<accel_args> argsv;
        vec2d sens = { 1, 1 };
        milliseconds time_min = 0.4;
    };

}

#pragma once

#include "vec2.h"
#include "accel-base.hpp"

namespace rawaccel {

    using milliseconds = double;

    inline constexpr int MAX_POLL_RATE_KHZ = 8;
    inline constexpr milliseconds DEFAULT_TIME_MIN = 1.0 / MAX_POLL_RATE_KHZ * 0.8;
    inline constexpr milliseconds WRITE_DELAY = 1000;

    enum class accel_mode {
        linear, classic, natural, naturalgain, power, motivity, noaccel
    };

    struct settings {
        double degrees_rotation = 0;
        bool combine_mags = true;
        vec2<accel_mode> modes = { accel_mode::noaccel, accel_mode::noaccel };
        vec2<accel_args> argsv;
        vec2d sens = { 1, 1 };
        vec2d dir_multipliers = {};
        vec2d directional_weights = { 1, 1 };
        stigma_args args_stigma;
        milliseconds time_min = DEFAULT_TIME_MIN;
    };

}

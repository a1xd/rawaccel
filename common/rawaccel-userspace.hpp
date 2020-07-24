#pragma once

#include <iostream>

#include "external/clipp.h"

#include "rawaccel.hpp"

namespace rawaccel {

inline constexpr int SYSTEM_ERROR = -1;
inline constexpr int PARSE_ERROR = 1;
inline constexpr int INVALID_ARGUMENT = 2;

void error(const char* s) { 
    throw std::domain_error(s); 
}

variables parse(int argc, char** argv) {
    double degrees = 0;
    vec2d sens = { 1, 1 };
    accel_function::args_t accel_args{};

    auto make_opt_vec = [](vec2d& v, auto first_flag, auto... rest) {
        return clipp::option(first_flag, rest...) & (
            clipp::number("xy", v.x, v.y) | (
                (clipp::required("x") & clipp::number("num", v.x)),
                (clipp::required("y") & clipp::number("num", v.y))
            )
        );
    };

    auto make_doc_fmt = [] {
        return clipp::doc_formatting{}
            .first_column(4)
            .doc_column(28)
            .last_column(80)
            // min value to not split optional vec2 alternatives
            .alternatives_min_split_size(5);
    };

    // default options
    auto opt_sens = "sensitivity (default = 1)" % make_opt_vec(sens, "sens");

    auto opt_rot = "counter-clockwise rotation (default = 0)" % (
        clipp::option("rotate") &
        clipp::number("degrees", degrees)
    );
    
    // mode-independent accel options
    auto opt_weight = "accel multiplier (default = 1)" % 
        make_opt_vec(accel_args.weight, "weight");

    auto opt_offset = "speed (dots/ms) where accel kicks in (default = 0)" % (
        clipp::option("offset") & clipp::number("speed", accel_args.offset)
    );

    auto opt_cap = "accel scale cap (default = 9)" % 
        make_opt_vec(accel_args.cap, "cap");

    auto opt_tmin = "minimum time between polls (default = 0.4)" % (
        clipp::option("tmin") &
        clipp::number("ms", accel_args.time_min)
    );

    auto accel_var = (clipp::required("accel") & clipp::number("num", accel_args.accel)) % "ramp rate";
    auto limit_var = (clipp::required("limit") & clipp::number("scale", accel_args.lim_exp)) % "limit";

    // modes
    auto noaccel_mode = "no-accel mode" % (
        clipp::command("off", "noaccel").set(accel_args.accel_mode, mode::noaccel)
    );
    auto lin_mode = "linear accel mode:" % (
        clipp::command("linear").set(accel_args.accel_mode, mode::linear),
        accel_var
    );
    auto classic_mode = "classic accel mode:" % (
        clipp::command("classic").set(accel_args.accel_mode, mode::classic),
        accel_var,
        (clipp::required("exponent") & clipp::number("num", accel_args.lim_exp)) % "exponent"
    );
    auto nat_mode = "natural accel mode:" % (
        clipp::command("natural").set(accel_args.accel_mode, mode::natural),
        accel_var,
        limit_var
    );
    auto log_mode = "logarithmic accel mode:" % (
        clipp::command("logarithmic").set(accel_args.accel_mode, mode::logarithmic),
        accel_var
    );
    auto sig_mode = "sigmoid accel mode:" % (
        clipp::command("sigmoid").set(accel_args.accel_mode, mode::sigmoid),
        accel_var,
        limit_var,
        (clipp::required("midpoint") & clipp::number("speed", accel_args.midpoint)) % "midpoint"
    );
    auto pow_mode = "power accel mode:" % (
        clipp::command("power").set(accel_args.accel_mode, mode::power) >> [&] { accel_args.accel = 1; },
        (clipp::required("exponent") & clipp::number("num", accel_args.lim_exp)) % "exponent",
        (clipp::option("scale") & clipp::number("num", accel_args.accel)) % "scale factor"
    );

    auto accel_mode_exclusive = (lin_mode | classic_mode | nat_mode | log_mode | sig_mode | pow_mode);
    auto accel_opts = "mode-independent accel options:" % (opt_cap, opt_weight, opt_offset, opt_tmin);

    bool help = false;

    auto cli = clipp::group(clipp::command("help").set(help)) | (
            noaccel_mode | (accel_mode_exclusive, accel_opts),
            opt_sens, 
            opt_rot
        );

    if (!clipp::parse(argc, argv, cli)) {
        std::cout << clipp::usage_lines(cli, "rawaccel", make_doc_fmt());
        std::exit(PARSE_ERROR);
    }

    if (help) {
        std::cout << clipp::make_man_page(cli, "rawaccel", make_doc_fmt());
        std::exit(0);
    }

    return variables(-degrees, sens, accel_args);
}

} // rawaccel

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

mouse_modifier parse(int argc, char** argv) {
    modifier_args modifier_args{};

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
    auto opt_sens = "sensitivity (default = 1)" % make_opt_vec(modifier_args.sens, "sens");

    auto opt_rot = "counter-clockwise rotation (default = 0)" % (
        clipp::option("rotate") &
        clipp::number("degrees", modifier_args.degrees)
    );
    
    // mode-independent accel options
    auto opt_weight = "accel multiplier (default = 1)" % 
        make_opt_vec(modifier_args.acc_fn_args.weight, "weight");

    auto opt_offset = "speed (dots/ms) where accel kicks in (default = 0)" % (
        clipp::option("offset") & clipp::number("speed", modifier_args.acc_fn_args.acc_args.offset)
    );

    auto opt_cap = "accel scale cap (default = 9)" % 
        make_opt_vec(modifier_args.acc_fn_args.cap, "cap");

    auto opt_tmin = "minimum time between polls (default = 0.4)" % (
        clipp::option("tmin") &
        clipp::number("ms", modifier_args.acc_fn_args.acc_args.time_min)
    );

    auto accel_var = (clipp::required("accel") & clipp::number("num", modifier_args.acc_fn_args.acc_args.accel)) % "ramp rate";
    auto limit_var = (clipp::required("limit") & clipp::number("scale", modifier_args.acc_fn_args.acc_args.lim_exp)) % "limit";

    // modes
    auto noaccel_mode = "no-accel mode" % (
        clipp::command("off", "noaccel").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_noaccel>)
    );
    auto lin_mode = "linear accel mode:" % (
        clipp::command("linear").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_linear>),
        accel_var
    );
    auto classic_mode = "classic accel mode:" % (
        clipp::command("classic").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_classic>),
        accel_var,
        (clipp::required("exponent") & clipp::number("num", modifier_args.acc_fn_args.acc_args.lim_exp)) % "exponent"
    );
    auto nat_mode = "natural accel mode:" % (
        clipp::command("natural").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_natural>),
        accel_var,
        limit_var
    );
    auto log_mode = "logarithmic accel mode:" % (
        clipp::command("logarithmic").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_logarithmic>),
        accel_var
    );
    auto sig_mode = "sigmoid accel mode:" % (
        clipp::command("sigmoid").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_sigmoid>),
        accel_var,
        limit_var,
        (clipp::required("midpoint") & clipp::number("speed", modifier_args.acc_fn_args.acc_args.midpoint)) % "midpoint"
    );
    auto pow_mode = "power accel mode:" % (
        clipp::command("power").set(modifier_args.acc_fn_args.accel_mode, accel_implementation_t::id<accel_power>) >> [&] { modifier_args.acc_fn_args.acc_args.accel = 1; },
        (clipp::required("exponent") & clipp::number("num", modifier_args.acc_fn_args.acc_args.lim_exp)) % "exponent",
        (clipp::option("scale") & clipp::number("num", modifier_args.acc_fn_args.acc_args.accel)) % "scale factor"
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

    return mouse_modifier(modifier_args);
}

} // rawaccel

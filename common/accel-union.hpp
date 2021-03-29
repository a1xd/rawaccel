#pragma once

#include "accel-classic.hpp"
#include "accel-natural.hpp"
#include "accel-power.hpp"
#include "accel-motivity.hpp"
#include "accel-noaccel.hpp"

namespace rawaccel {

    enum class internal_mode {
        classic_lgcy,
        classic_gain,
        natural_lgcy,
        natural_gain,
        power,
        motivity,
        noaccel
    };

    constexpr internal_mode make_mode(accel_mode m, bool legacy) 
    {
        switch (m) {
        case accel_mode::classic:
            return legacy ? internal_mode::classic_lgcy : internal_mode::classic_gain;
        case accel_mode::natural:
            return legacy ? internal_mode::natural_lgcy : internal_mode::natural_gain;
        case accel_mode::power:
            return internal_mode::power;
        case accel_mode::motivity:
            return internal_mode::motivity;
        default:
            return internal_mode::noaccel;
        }
    }

    constexpr internal_mode make_mode(const accel_args& args) 
    {
        return make_mode(args.mode, args.legacy);
    }

    template <typename Visitor, typename Variant>
    inline auto visit_accel(Visitor vis, Variant&& var) 
    {
        switch (var.tag) {
        case internal_mode::classic_lgcy: return vis(var.u.classic_l);
        case internal_mode::classic_gain: return vis(var.u.classic_g);
        case internal_mode::natural_lgcy: return vis(var.u.natural_l);
        case internal_mode::natural_gain: return vis(var.u.natural_g);
        case internal_mode::power:        return vis(var.u.power);
        case internal_mode::motivity:     return vis(var.u.motivity);
        default:                          return vis(var.u.noaccel);
        }
    }

    struct accel_variant {
        si_pair* lookup;

        internal_mode tag = internal_mode::noaccel;

        union union_t {
            classic classic_g;
            classic_legacy classic_l;
            natural natural_g;
            natural_legacy natural_l;
            power power;
            motivity motivity;
            accel_noaccel noaccel = {};
        } u = {};

        accel_variant(const accel_args& args, si_pair* lut = nullptr) :
            tag(make_mode(args)), lookup(lut)
        {
            visit_accel([&](auto& impl) {
                impl = { args };
            }, *this);

            if (lookup && tag == internal_mode::motivity) {
                u.motivity.fill(lookup);
            }

        }

        double apply(double speed) const 
        {
            if (lookup && tag == internal_mode::motivity) {
                return u.motivity.apply(lookup, speed);
            }

            return visit_accel([=](auto&& impl) {
                return impl(speed);
            }, *this);
        }

        accel_variant() = default;
    };

}

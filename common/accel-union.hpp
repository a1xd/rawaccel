#pragma once

#include "accel-classic.hpp"
#include "accel-jump.hpp"
#include "accel-natural.hpp"
#include "accel-power.hpp"
#include "accel-motivity.hpp"
#include "accel-noaccel.hpp"

namespace rawaccel {

    enum class internal_mode {
        classic_lgcy,
        classic_gain,
        jump_lgcy,
        jump_gain,
        natural_lgcy,
        natural_gain,
        power_lgcy,
        power_gain,
        motivity_lgcy,
        motivity_gain,
        lut_log,
        lut_lin,
        noaccel
    };

    constexpr internal_mode make_mode(accel_mode mode, table_mode lut_mode, bool legacy) 
    {
        if (lut_mode != table_mode::off) {
            switch (lut_mode) {
            case table_mode::binlog: return internal_mode::lut_log;
            case table_mode::linear: return internal_mode::lut_lin;
            default: return internal_mode::noaccel;
            }
        }
        else if (mode < accel_mode{} || mode >= accel_mode::noaccel) {
            return internal_mode::noaccel;
        }
        else {
            int im = static_cast<int>(mode) * 2 + (legacy ? 0 : 1);
            return static_cast<internal_mode>(im);
        }
    }

    constexpr internal_mode make_mode(const accel_args& args) 
    {
        return make_mode(args.mode, args.lut_args.mode, args.legacy);
    }

    template <typename Visitor, typename Variant>
    inline auto visit_accel(Visitor vis, Variant&& var) 
    {
        switch (var.tag) {
        case internal_mode::classic_lgcy:  return vis(var.u.classic_l);
        case internal_mode::classic_gain:  return vis(var.u.classic_g);
        case internal_mode::jump_lgcy:     return vis(var.u.jump_l);
        case internal_mode::jump_gain:     return vis(var.u.jump_g);
        case internal_mode::natural_lgcy:  return vis(var.u.natural_l);
        case internal_mode::natural_gain:  return vis(var.u.natural_g);
        case internal_mode::power_lgcy:    return vis(var.u.power_l);
        case internal_mode::power_gain:    return vis(var.u.power_g);
        case internal_mode::motivity_lgcy: return vis(var.u.motivity_l);
        case internal_mode::motivity_gain: return vis(var.u.motivity_g);
        case internal_mode::lut_log:       return vis(var.u.log_lut);
        case internal_mode::lut_lin:       return vis(var.u.lin_lut);
        default:                           return vis(var.u.noaccel);
        }
    }

    struct accel_variant {
        internal_mode tag = internal_mode::noaccel;

        union union_t {
            classic classic_g;
            classic_legacy classic_l;
            jump jump_g;
            jump_legacy jump_l;
            natural natural_g;
            natural_legacy natural_l;
            power power_g;
            power_legacy power_l;
            sigmoid motivity_l;
            motivity motivity_g;
            linear_lut lin_lut;
            binlog_lut log_lut;
            accel_noaccel noaccel = {};
        } u = {};

        accel_variant(const accel_args& args) :
            tag(make_mode(args))
        {
            visit_accel([&](auto& impl) {
                impl = { args };
            }, *this);
        }

        double apply(double speed) const 
        {
            return visit_accel([=](auto&& impl) {
                return impl(speed);
            }, *this);
        }

        accel_variant() = default;
    };

}

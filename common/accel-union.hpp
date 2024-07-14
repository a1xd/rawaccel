#pragma once

#include "accel-classic.hpp"
#include "accel-jump.hpp"
#include "accel-lookup.hpp"
#include "accel-synchronous.hpp"
#include "accel-natural.hpp"
#include "accel-noaccel.hpp"
#include "accel-power.hpp"

namespace rawaccel {

    union accel_union {
        accel_noaccel noaccel;
        lookup lut;
        classic<GAIN> classic_g;
        classic<LEGACY> classic_l;
        jump<GAIN> jump_g;
        jump<LEGACY> jump_l;
        natural<GAIN> natural_g;
        natural<LEGACY> natural_l;
        power<GAIN> power_g;
        power<LEGACY> power_l;
        activation_framework<GAIN> loglog_sigmoid_g;
        activation_framework<LEGACY> loglog_sigmoid_l;

        template <template <bool> class AccelTemplate, typename Visitor>
        auto visit_helper(Visitor vis, bool gain)
        {
            if (gain) return vis(reinterpret_cast<AccelTemplate<GAIN>&>(*this));
            else  return vis(reinterpret_cast<AccelTemplate<LEGACY>&>(*this));
        }

        template <typename Visitor>
        auto visit(Visitor vis, const accel_args& args)
        {
            switch (args.mode) {
            case accel_mode::classic:       return visit_helper<classic>(vis, args.gain);
            case accel_mode::jump:          return visit_helper<jump>(vis, args.gain);
            case accel_mode::natural:       return visit_helper<natural>(vis, args.gain);
            case accel_mode::synchronous:   return visit_helper<activation_framework>(vis, args.gain);
            case accel_mode::power:         return visit_helper<power>(vis, args.gain);
            case accel_mode::lookup:        return vis(lut);
            default:                        return vis(noaccel);
            }
        }

        void init(const accel_args& args)
        {
            visit([&](auto& impl) {
                impl = { args };
            }, args);
        }

    };

}

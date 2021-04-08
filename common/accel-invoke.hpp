#pragma once

#include "accel-union.hpp"

namespace rawaccel {

    class accel_invoker {
        using callback_t = double (*)(const accel_union&, double, double);

        callback_t cb = &invoke_impl<accel_noaccel>;

        template <typename T>
        static double invoke_impl(const accel_union& u, double x, double w)
        {
            return apply_weighted(reinterpret_cast<const T&>(u), x, w);
        }

    public:

        accel_invoker(const accel_args& args)
        {
            cb = visit_accel([](auto&& arg) { 
                using T = remove_ref_t<decltype(arg)>;

                if constexpr (is_same_v<T, motivity>) {
                    static_assert(sizeof motivity == sizeof binlog_lut);
                    return &invoke_impl<binlog_lut>;
                }
                else {
                    return &invoke_impl<T>;
                }

            }, make_mode(args), accel_union{});
        }

        accel_invoker() = default;

        double invoke(const accel_union& u, double x, double weight = 1) const
        {
            return (*cb)(u, x, weight);
        }
    };

    inline vec2<accel_invoker> invokers(const settings& args)
    {
        return {
            accel_invoker(args.argsv.x),
            accel_invoker(args.argsv.y)
        };
    }

}

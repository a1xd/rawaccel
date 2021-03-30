#pragma once

#ifdef _MANAGED
#include <math.h>
#else
#include <emmintrin.h>
#endif

namespace rawaccel {

#ifdef _MANAGED
    inline double sqrtsd(double val) { return sqrt(val); }
#else
    inline double sqrtsd(double val) {
        __m128d src = _mm_load_sd(&val);
        __m128d dst = _mm_sqrt_sd(src, src);
        _mm_store_sd(&val, dst);
        return val;
    }
#endif

    constexpr double minsd(double a, double b) 
    {
        return (a < b) ? a : b;
    }

    constexpr double maxsd(double a, double b) 
    {
        return (b < a) ? a : b;
    }

    constexpr double clampsd(double v, double lo, double hi) 
    {
        return minsd(maxsd(v, lo), hi);
    }

    template <typename T>
    constexpr const T& min(const T& a, const T& b)
    {
        return (b < a) ? b : a;
    }

    template <typename T>
    constexpr const T& max(const T& a, const T& b)
    {
        return (b < a) ? a : b;
    }

    template <typename T>
    constexpr const T& clamp(const T& v, const T& lo, const T& hi)
    {
        return (v < lo) ? lo : (hi < v) ? hi : v;
    }

    constexpr double lerp(double a, double b, double t)
    {
        double x = a + t * (b - a);
        if ((t > 1) == (a < b)) {
            return maxsd(x, b);
        }
        return minsd(x, b);
    }

    // returns the unbiased exponent of x if x is normal 
    inline int ilogb(double x)
    {
	    union { double f; unsigned long long i; } u = { x };
	    return static_cast<int>((u.i >> 52) & 0x7ff) - 0x3ff;
    }

    // returns x * 2^n if n is in [-1022, 1023] 
    inline double scalbn(double x, int n)
    {
        union { double f; unsigned long long i; } u;
        u.i = static_cast<unsigned long long>(0x3ff + n) << 52;
        return x * u.f;
    }

    inline bool infnan(double x)
    {
	    return ilogb(x) == 0x400;
    }

}

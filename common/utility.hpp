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

    // returns the unbiased exponent of x if x is normal 
    inline int ilogb(double x)
    {
	    union { double f; unsigned long long i; } u = { x };
	    return static_cast<int>((u.i >> 52) & 0x7ff) - 0x3ff;
    }

    inline bool infnan(double x)
    {
	    return ilogb(x) == 0x400;
    }

}

#pragma once

#ifdef _MANAGED

#include <math.h>
inline double sqrtsd(double val) { return sqrt(val); }

#else

#include <emmintrin.h>
inline double sqrtsd(double val) {
    __m128d src = _mm_load_sd(&val);
    __m128d dst = _mm_sqrt_sd(src, src);
    _mm_store_sd(&val, dst);
    return val;
}

#endif

inline constexpr double minsd(double a, double b) {
    return (a < b) ? a : b;
}

inline constexpr double maxsd(double a, double b) {
    return (b < a) ? a : b;
}

inline constexpr double clampsd(double v, double lo, double hi) {
    return minsd(maxsd(v, lo), hi);
}

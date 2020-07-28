#pragma once

#define _USE_MATH_DEFINES
#include <math.h>

#include "vec2.h"
#include "x64-util.hpp"
#include "external/tagged-union-single.h"

namespace rawaccel {   

enum class mode { noaccel, linear, classic, natural, logarithmic, sigmoid, power };

struct rotator {
    vec2d rot_vec = { 1, 0 };

    inline vec2d operator()(const vec2d& input) const {
        return { 
            input.x * rot_vec.x - input.y * rot_vec.y,
            input.x * rot_vec.y + input.y * rot_vec.x
        };
    }

    rotator(double degrees) {
        double rads = degrees * M_PI / 180;
        rot_vec = { cos(rads), sin(rads) };
    }

    rotator() = default;
};

struct accel_scale_clamp {
    double lo = 0;
    double hi = 9;

    inline double operator()(double scale) const {
        return clampsd(scale, lo, hi);
    }

    accel_scale_clamp(double cap) : accel_scale_clamp() {
        if (cap <= 0) {
            // use default, effectively uncapped accel
            return;
        }

        if (cap < 1) {
            // assume negative accel
            lo = cap;
            hi = 1;
        }
        else hi = cap;
    }

    accel_scale_clamp() = default;
};

void error(const char*);

using milliseconds = double;

struct args_t {
	mode accel_mode = mode::noaccel;
	milliseconds time_min = 0.4;
	double offset = 0;
	double accel = 0;
	double lim_exp = 2;
	double midpoint = 0;
	vec2d weight = { 1, 1 };
	vec2d cap = { 0, 0 };
};

struct accel_implentation {
	double b = 0;
	double k = 0;
	double m = 0;
	double offset = 0;

	accel_implentation(args_t args)
	{
		b = args.accel;
		k = args.lim_exp;
		m = args.midpoint;
		offset = args.offset;
	}

	double virtual accelerate(double speed) { return 0; }
	void virtual verify(args_t args) {
		if (args.accel < 0) error("accel can not be negative, use a negative weight to compensate");
		if (args.time_min <= 0) error("min time must be positive");
	}

	accel_implentation() = default;
};

struct accel_linear : accel_implentation {
    accel_linear(args_t args)
        : accel_implentation(args) {}

    double virtual accelerate(double speed) override {
		return b * speed;
	}

	void virtual verify(args_t args) override {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("limit must be greater than 1");
	}
};

struct accel_classic : accel_implentation {
    accel_classic(args_t args)
        : accel_implentation(args) {}

	double virtual accelerate(double speed) override {
		return pow(b * speed, k); 

	}

	void virtual verify(args_t args) override {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
};

struct accel_logarithmic : accel_implentation {
    accel_logarithmic(args_t args)
        : accel_implentation(args) {}

	double virtual accelerate(double speed) override {
		return log(speed * b + 1); 
	}

	void virtual verify(args_t args) override {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
};

struct accel_sigmoid : accel_implentation {
    accel_sigmoid(args_t args)
        : accel_implentation(args) {}

	double virtual accelerate(double speed) override {
		return k / (exp(-b * (speed - m)) + 1); 

	}

	void virtual verify(args_t args) override {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
};

struct accel_power : accel_implentation {
    accel_power(args_t args)
        : accel_implentation(args) {}

	double virtual accelerate(double speed) override {
		return (offset > 0 && speed < 1) ? 0 : pow(speed*b, k) - 1;


	}

	void virtual verify(args_t args) override {
		accel_implentation::verify(args);
		if (args.lim_exp <= 1) error("exponent must be greater than 1");
	}
};

using accel_implementation_t = tagged_union<accel_implentation, accel_linear, accel_classic, accel_logarithmic, accel_sigmoid, accel_power>;

struct accel_function {

    /* 
    This value is ideally a few microseconds lower than 
    the user's mouse polling interval, though it should
    not matter if the system is stable.
    */
    milliseconds time_min = 0.4;
    
    double speed_offset = 0;

    // speed midpoint in sigmoid mode
    double m = 0;

    // accel ramp rate
    double b = 0; 

    // the limit for natural and sigmoid modes
    // or the exponent for classic and power modes
    double k = 1; 
    
	accel_implementation_t accel = accel_implentation{};

    vec2d weight = { 1, 1 };
    vec2<accel_scale_clamp> clamp;

    accel_function(args_t args) {
        accel = accel_linear(args);

        time_min = args.time_min;
        m = args.midpoint;
        b = args.accel;
        k = args.lim_exp - 1;
        if (args.accel_mode == mode::natural) b /= k;
        if (args.accel_mode == mode::power) k++;
        
        speed_offset = args.offset;
        weight = args.weight;
        clamp.x = accel_scale_clamp(args.cap.x);
        clamp.y = accel_scale_clamp(args.cap.y);
    }

	double apply(double speed) const {
		return accel.visit([=](auto accel_t) { return accel_t.accelerate(speed); });
	}

    inline vec2d operator()(const vec2d& input, milliseconds time, mode accel_mode) const {
        double mag = sqrtsd(input.x * input.x + input.y * input.y);
        double time_clamped = clampsd(time, time_min, 100);
        double speed = maxsd(mag / time_clamped - speed_offset, 0);

        double accel_val = apply(speed);

        double scale_x = weight.x * accel_val + 1;
        double scale_y = weight.y * accel_val + 1;

        return { 
            input.x * clamp.x(scale_x), 
            input.y * clamp.y(scale_y) 
        };
    }

    accel_function() = default;
};

struct variables {
    bool apply_rotate = false;
    bool apply_accel = false;
    mode accel_mode = mode::noaccel;
    rotator rotate;
    accel_function accel_fn;
    vec2d sensitivity = { 1, 1 };

    variables(double degrees, vec2d sens, args_t accel_args)
        : accel_fn(accel_args)
    {
        apply_rotate = degrees != 0;
        if (apply_rotate) rotate = rotator(degrees);
        else rotate = rotator();

        apply_accel = accel_args.accel_mode != mode::noaccel;
        accel_mode = accel_args.accel_mode;

        if (sens.x == 0) sens.x = 1;
        if (sens.y == 0) sens.y = 1;
        sensitivity = sens;
    }

    variables() = default;
};

} // rawaccel

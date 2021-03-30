#pragma once

#include "accel-lookup.hpp"

#include <math.h>

namespace rawaccel {

	struct sigmoid {
		double accel;
		double motivity;
		double midpoint;
		double constant;

		sigmoid(const accel_args& args) :
			accel(exp(args.accel_motivity)),
			motivity(2 * log(args.motivity)),
			midpoint(log(args.midpoint)),
			constant(-motivity / 2) {}

		double operator()(double x) const
		{
			double denom = exp(accel * (midpoint - log(x))) + 1;
			return exp(motivity / denom + constant);
		}
	};

	/// <summary> Struct to hold sigmoid (s-shaped) gain implementation. </summary>
	struct motivity : binlog_lut {

		using binlog_lut::operator();

		motivity(const accel_args& args) :
			binlog_lut(args)
		{
			double sum = 0;
			double a = 0;
			auto sigmoid_sum = [&, sig = sigmoid(args)](double b) mutable {
				double interval = (b - a) / args.lut_args.partitions;
				for (int i = 1; i <= args.lut_args.partitions; i++) {
					sum += sig(a + i * interval) * interval;
				}
				a = b;
				return sum;
			};

			fill([&](double x) {
				double y = sigmoid_sum(x);
				if (!this->transfer) y /= x;
				return y;
			});
		}

	};

}

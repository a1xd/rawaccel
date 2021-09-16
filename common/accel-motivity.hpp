#pragma once

#include "accel-lookup.hpp"

namespace rawaccel {

	template <bool Gain> struct loglog_sigmoid;

	template <>
	struct loglog_sigmoid<LEGACY> {
		double accel;
		double motivity;
		double midpoint;
		double constant;

		loglog_sigmoid(const accel_args& args) :
			accel(exp(args.growth_rate)),
			motivity(2 * log(args.motivity)),
			midpoint(log(args.midpoint)),
			constant(-motivity / 2) {}

		double operator()(double x, const accel_args&) const
		{
			double denom = exp(accel * (midpoint - log(x))) + 1;
			return exp(motivity / denom + constant);
		}

	};

	template <>
	struct loglog_sigmoid<GAIN> {
		enum { capacity = LUT_RAW_DATA_CAPACITY };

		bool velocity;
		fp_rep_range range;
		double x_start;

		loglog_sigmoid(const accel_args& args) 
		{
			init({ -3, 9, 8 }, true);

			double sum = 0;
			double a = 0;
			auto sig = loglog_sigmoid<LEGACY>(args);
			auto sigmoid_sum = [&](double b) {
				int partitions = 2;

				double interval = (b - a) / partitions;
				for (int i = 1; i <= partitions; i++) {
					sum += sig(a + i * interval, args) * interval;
				}
				a = b;
				return sum;
			};

			fill([&](double x) {
				double y = sigmoid_sum(x);
				if (!velocity) y /= x;
				return y;
			}, args, range);
		}

		double operator()(double x, const accel_args& args) const
		{
			auto* data = args.data;

			int e = min(ilogb(x), range.stop - 1);

			if (e >= range.start) {
				int idx_int_log_part = e - range.start;
				double idx_frac_lin_part = scalbn(x, -e) - 1;
				double idx_f = range.num * (idx_int_log_part + idx_frac_lin_part);

				unsigned idx = min(static_cast<int>(idx_f), range.size() - 2);

				if (idx < capacity - 1) {
					double y = lerp(data[idx], data[idx + 1], idx_f - idx);
					if (velocity) y /= x;
					return y;
				}
			}

			double y = data[0];
			if (velocity) y /= x_start;
			return y;
		}

		void init(const fp_rep_range& r, bool vel) 
		{
			velocity = vel;
			range = r;
			x_start = scalbn(1, range.start);
		}

		template <typename Func>
		static void fill(Func fn, const accel_args& args, const fp_rep_range& range)
		{
			range.for_each([&, fn, i = 0](double x) mutable {
					args.data[i++] = static_cast<float>(fn(x));
				});
		}

	};

}

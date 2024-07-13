#pragma once

#include "accel-lookup.hpp"

namespace rawaccel {

	template <bool Gain> struct activation_framework;

	template <>
	struct activation_framework<LEGACY> {
		double log_motivity;
		double gamma_const;
		double log_syncspeed;
		double syncspeed;
		double sharpness;
		double sharpness_recip;
		bool use_linear_clamp;
		double minimum_sens;
		double maximum_sens;

		activation_framework(const accel_args& args) :
			log_motivity(log(args.motivity)),
			gamma_const(args.gamma / log_motivity),
			log_syncspeed(log(args.sync_speed)),
			syncspeed(args.sync_speed),
			sharpness(args.smooth == 0 ? 16 : 0.5 / args.smooth),
			sharpness_recip(1 / sharpness),
		    use_linear_clamp(sharpness >= 16),
		    minimum_sens(1 / args.motivity),
		    maximum_sens(args.motivity) {}

		double operator()(double x, const accel_args&) const
		{
			// if sharpness >= 16, use linear clamp for activation function.
			// linear clamp means: clamp(x, -1, 1).
			if (use_linear_clamp)
			{
				double log_space = gamma_const * (log(x) - log_syncspeed);

				if (log_space < -1)
				{
					return minimum_sens;
				}

				if (log_space > 1)
				{
					return maximum_sens;
				}

				return exp(log_space * log_motivity);
			}

			if (x == syncspeed) {
				return 1.0;
			}

			double log_x = log(x);
			double log_diff = log_x - log_syncspeed;

			if (log_diff > 0)
			{
				double log_space = gamma_const * log_diff;
				double exponent = pow(tanh(pow(log_space, sharpness)), sharpness_recip);
				return exp(exponent * log_motivity);
			}
			else
			{
				double log_space = -gamma_const * log_diff;
				double exponent = -pow(tanh(pow(log_space, sharpness)), sharpness_recip);
				return exp(exponent * log_motivity);
			}
		}
	};

	template <>
	struct activation_framework<GAIN> {
		enum { capacity = LUT_RAW_DATA_CAPACITY };

		bool velocity;
		fp_rep_range range;
		double x_start;

		activation_framework(const accel_args& args) 
		{
			init({ -3, 9, 8 }, true);

			double sum = 0;
			double a = 0;
			auto sig = activation_framework<LEGACY>(args);
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

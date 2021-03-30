#pragma once

#include "rawaccel-base.hpp"
#include "utility.hpp"

namespace rawaccel {

	struct linear_range {
		double start;
		double stop;
		int num;

		template <typename Func>
		void for_each(Func fn) const
		{
			double interval = (stop - start) / (num - 1);
			for (int i = 0; i < num; i++) {
				fn(i * interval + start);
			}
		}

		int size() const
		{
			return num;
		}
	};


	// represents the range [2^start, 2^stop], with num - 1
	// elements linearly spaced between each exponential step
	struct fp_rep_range {
		int start;
		int stop;
		int num;

		template <typename Func>
		void for_each(Func fn) const
		{
			for (int e = 0; e < stop - start; e++) {
				double exp_scale = scalbn(1, e + start) / num;

				for (int i = 0; i < num; i++) {
					fn((i + num) * exp_scale);
				}
			}

			fn(scalbn(1, stop));
		}

		int size() const
		{
			return (stop - start) * num + 1;
		}
	};

	template <typename Lookup>
	struct lut_base {
		enum { capacity = LUT_CAPACITY };
		using value_t = float;

		template <typename Func>
		void fill(Func fn)
		{
			auto* self = static_cast<Lookup*>(this);

			self->range.for_each([&, fn, i = 0](double x) mutable {
				self->data[i++] = static_cast<value_t>(fn(x));
			});
		}

	};

	struct linear_lut : lut_base<linear_lut> {
		linear_range range;
		bool transfer = false;
		value_t data[capacity] = {};

		double operator()(double x) const
		{
			if (x > range.start) {
				double range_dist = range.stop - range.start;
				double idx_f = (x - range.start) * (range.num - 1) / range_dist;

				unsigned idx = min(static_cast<int>(idx_f), range.size() - 2);

				if (idx < capacity - 1) {
					double y = lerp(data[idx], data[idx + 1], idx_f - idx);
					if (transfer) y /= x;
					return y;
				}
			}

			double y = data[0];
			if (transfer) y /= range.start;
			return y;
		}

		linear_lut(const table_args& args) :
			range({
				args.start,
				args.stop,
				args.num_elements
				}),
			transfer(args.transfer) {}

		linear_lut(const accel_args& args) :
			linear_lut(args.lut_args) {}
	};

	struct binlog_lut : lut_base<binlog_lut> {
		fp_rep_range range;
		double x_start;
		bool transfer = false;
		value_t data[capacity] = {};

		double operator()(double x) const
		{
			int e = min(ilogb(x), range.stop - 1);

			if (e >= range.start) {
				int idx_int_log_part = e - range.start;
				double idx_frac_lin_part = scalbn(x, -e) - 1;
				double idx_f = range.num * (idx_int_log_part + idx_frac_lin_part);

				unsigned idx = min(static_cast<int>(idx_f), range.size() - 2);

				if (idx < capacity - 1) {
					double y = lerp(data[idx], data[idx + 1], idx_f - idx);
					if (transfer) y /= x;
					return y;
				}
			}

			double y = data[0];
			if (transfer) y /= x_start;
			return y;
		}

		binlog_lut(const table_args& args) :
			range({
				static_cast<int>(args.start),
				static_cast<int>(args.stop),
				args.num_elements
				}),
			x_start(scalbn(1, range.start)),
			transfer(args.transfer) {}

		binlog_lut(const accel_args& args) :
			binlog_lut(args.lut_args) {}
	};

}

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

	struct si_pair { 
		double slope = 0;
		double intercept = 0; 
	};

	struct arbitrary_lut_point {
		double applicable_speed = 0;
		si_pair slope_intercept = {};
	};

	struct arbitrary_lut {
		fp_rep_range range;
		arbitrary_lut_point data[LUT_CAPACITY] = {};
		int log_lookup[LUT_CAPACITY] = {};
		double first_point_speed;
		double last_point_speed;
		int last_arbitrary_index;
		int last_log_lookup_index;

		double operator()(double speed) const
		{
			int index = 0;

			if (speed < first_point_speed)
			{
				// Apply from 0 index
			}
			else if (speed > last_point_speed)
			{
				index = last_arbitrary_index;
			}
			else if (speed > range.stop)
			{
				index = search_from(log_lookup[last_log_lookup_index], speed);
			}
			else if (speed < range.start)
			{
				index = search_from(0, speed);
			}
			else
			{
				int log_lookup = get_log_index(speed);
				index = search_from(log_lookup, speed);
			}

			return apply(index, speed);
		}

		int inline get_log_index(double speed) const
		{
			double speed_log = log(speed) - range.start;
			int index = (int)floor(speed_log * range.num);
			return index;
		}

		int inline search_from(int index, double speed) const
		{
			int prev_index;

			do
			{
				prev_index = index;
				index++;
			}
			while (index <= last_arbitrary_index && data[index].applicable_speed < speed);

			index--;

			return index;
		}

		double inline apply(int index, double speed) const
		{
			si_pair pair = data[index].slope_intercept;
			return pair.slope + pair.intercept / speed;
		}

		void fill(vec2d* points, int length)
		{
			vec2d current = {0, 0};
			vec2d next;
			int log_index = 0;
			double log_inner_iterator = range.start;
			double log_inner_slice = 1 / range.num;
			double log_value = pow(2, log_inner_iterator);

			for (int i = 0; i < length; i++)
			{
				next = points[i];
				double slope = (next.y - current.y) / (next.x - current.x);
				double intercept = next.y - slope * next.x;
				si_pair current_si = { slope, intercept };
				arbitrary_lut_point current_lut_point = { next.x, current_si };

				this->data[i] = current_lut_point;

				while (log_value < next.x)
				{
					this->log_lookup[log_index] = log_value;
					log_index++;
					log_inner_iterator += log_inner_slice;
					log_value = pow(2, log_inner_iterator);
				}
			}
		}

		arbitrary_lut(vec2d* points, int length)
		{
			first_point_speed = points[0].x;
			// -2 because the last index in the arbitrary array is used for slope-intercept only
			last_arbitrary_index = length - 2;
			last_point_speed = points[last_arbitrary_index].x;

			int start = static_cast<int>(log(first_point_speed));
			int end = static_cast<int>(log(last_point_speed));
			int num = static_cast<int>(LUT_CAPACITY / (end - start));
			range = fp_rep_range{ start, end, num };
			last_log_lookup_index = num * (end - start) - 1;

			fill(points, length);
		}
	};
}

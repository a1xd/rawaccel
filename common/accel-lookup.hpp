#pragma once

#include "rawaccel-base.hpp"
#include "utility.hpp"

#include <math.h>

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
		enum { capacity = SPACED_LUT_CAPACITY };
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

		linear_lut(const spaced_lut_args& args) :
			range({
				args.start,
				args.stop,
				args.num_elements
				}),
			transfer(args.transfer) {}

		linear_lut(const accel_args& args) :
			linear_lut(args.spaced_args) {}
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

		binlog_lut(const spaced_lut_args& args) :
			range({
				static_cast<int>(args.start),
				static_cast<int>(args.stop),
				args.num_elements
				}),
			x_start(scalbn(1, range.start)),
			transfer(args.transfer) {}

		binlog_lut(const accel_args& args) :
			binlog_lut(args.spaced_args) {}
	};

	struct si_pair { 
		float slope = 0;
		float intercept = 0;
	};

	struct arbitrary_lut_point {
		float applicable_speed = 0;
		si_pair slope_intercept = {};
	};

	struct arbitrary_lut {
		enum { capacity = ARB_LUT_CAPACITY };

		fp_rep_range range;
		arbitrary_lut_point data[capacity] = {};
		int log_lookup[capacity] = {};
		double first_point_speed;
		double last_point_speed;
		int last_arbitrary_index;
		int last_log_lookup_index;
		double last_log_lookup_speed;
		double first_log_lookup_speed;
		bool velocity_points;

		double operator()(double speed) const
		{
			int index = 0;
			int last_arb_index = last_arbitrary_index;
			int last_log_index = last_log_lookup_index;

			if (speed <= 0) return 1;

			if (unsigned(last_arb_index) < capacity &&
				unsigned(last_log_index) < capacity &&
				speed > first_point_speed)
			{
				if (speed > last_point_speed)
				{
					index = last_arb_index;
				}
				else if (speed > last_log_lookup_speed)
				{
					int last_log = log_lookup[last_log_index];
					if (unsigned(last_log) >= capacity) return 1;
					index = search_from(last_log, last_arb_index, speed);
				}
				else if (speed < first_log_lookup_speed)
				{
					index = search_from(0, last_arb_index, speed);
				}
				else
				{
					int log_index = get_log_index(speed);
					if (unsigned(log_index) >= capacity) return 1;
					int arbitrary_index = log_lookup[log_index];
					if (arbitrary_index < 0) return 1;
					index = search_from(arbitrary_index, last_arb_index, speed);
				}

			}

			return apply(index, speed);
		}

		int inline get_log_index(double speed) const
		{
			double speed_log = log(speed) - range.start;
			int index = (int)floor(speed_log * range.num);
			return index;
		}

		int inline search_from(int index, int last, double speed) const
		{
			do
			{
				index++;
			} 			
			while (index <= last && data[index].applicable_speed < speed);

			return index - 1;
		}

		double inline apply(int index, double speed) const
		{
			auto [slope, intercept] = data[index].slope_intercept;

			if (velocity_points)
			{
				return slope + intercept / speed;
			}
			else
			{
				return slope * speed + intercept;
			}
		}

		void fill(const vec2<float>* points, int length)
		{
			first_point_speed = points[0].x;
			last_arbitrary_index = length - 1;
			// -2 because the last index in the arbitrary array is used for slope-intercept only
			last_point_speed = points[length-2].x;

			int start = static_cast<int>(floor(log(first_point_speed)));
			first_log_lookup_speed = exp(start*1.0);
			int end = static_cast<int>(floor(log(last_point_speed)));
			last_log_lookup_speed = exp(end*1.0);
			int num = end > start ? static_cast<int>(capacity / (end - start)) : 1;
			range = fp_rep_range{ start, end, num };
			last_log_lookup_index = end > start ? num * (end - start) - 1 : 0;

			vec2<float> current = {0, velocity_points ? 0.0f : 1.0f };
			vec2<float> next;
			int log_index = 0;
			double log_inner_iterator = range.start;
			double log_inner_slice = 1.0 / (range.num * 1.0);
			double log_value = exp(log_inner_iterator);

			for (int i = 0; i < length; i++)
			{
				next = points[i];
				double slope = (next.y - current.y) / (next.x - current.x);
				double intercept = next.y - slope * next.x;
				si_pair current_si = { 
					static_cast<float>(slope), 
					static_cast<float>(intercept)
				};
				arbitrary_lut_point current_lut_point = { 
					static_cast<float>(current.x), 
					current_si 
				};

				this->data[i] = current_lut_point;

				while (log_value < next.x && log_inner_iterator < end)
				{
					this->log_lookup[log_index] = i;
					log_index++;
					log_inner_iterator += log_inner_slice;
					log_value = exp(log_inner_iterator);
				}

				current = next;
			}
		}

		arbitrary_lut(const accel_args& args)
		{
			velocity_points = args.arb_args.velocity;
			fill(args.arb_args.data, args.arb_args.length);
		}
	};
}

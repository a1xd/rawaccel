#pragma once

#include "rawaccel-base.hpp"
#include "utility.hpp"

namespace rawaccel {

	struct valid_ret_t {
		int count_x = 0;
		int count_y = 0;
		int count = 0;

		explicit operator bool() const
		{
			return count == 0;
		}
	};

	template <typename MsgHandler = noop>
	valid_ret_t valid(const settings& args, MsgHandler fn = {})
	{
		valid_ret_t ret;

		auto error = [&](auto msg) {
			++ret.count;
			fn(msg);
		};

		auto check_accel = [&error](const accel_args& args) {
			static_assert(LUT_CAPACITY == 1025, "update error msg");

			const auto& lut_args = args.lut_args;

			if (lut_args.partitions <= 0) {
				error("lut partitions"" must be positive");
			}

			if (lut_args.mode == table_mode::linear) {
				if (lut_args.start <= 0) {
					error("start"" must be positive");
				}

				if (lut_args.stop <= lut_args.start) {
					error("stop must be greater than start");
				}

				if (lut_args.num_elements < 2 ||
					lut_args.num_elements > 1025) {
					error("num must be between 2 and 1025");
				}
			}
			else if (lut_args.mode == table_mode::binlog) {
				int istart = static_cast<int>(lut_args.start);
				int istop = static_cast<int>(lut_args.stop);

				if (lut_args.start < -99) {
					error("start is too small");
				}
				else if (lut_args.stop > 99) {
					error("stop is too large");
				}
				else if (istart != lut_args.start || istop != lut_args.stop) {
					error("start and stop must be integers");
				}
				else if (istop <= istart) {
					error("stop must be greater than start");
				}
				else if (lut_args.num_elements <= 0) {
					error("num"" must be positive");
				}
				else if (((lut_args.stop - lut_args.start) * lut_args.num_elements) >= 1025) {
					error("binlog mode requires (num * (stop - start)) < 1025");
				}
			}


			if (args.offset < 0) {
				error("offset can not be negative");
			}

			if (args.cap <= 0) {
				error("cap"" must be positive");
			}

			if (args.growth_rate <= 0 ||
				args.decay_rate <= 0 ||
				args.accel_classic <= 0) {
				error("acceleration"" must be positive");
			}

			if (args.motivity <= 1) {
				error("motivity must be greater than 1");
			}

			if (args.power <= 1) {
				error("power must be greater than 1");
			}

			if (args.scale <= 0) {
				error("scale"" must be positive");
			}

			if (args.weight <= 0) {
				error("weight"" must be positive");
			}

			if (args.exponent <= 0) {
				error("exponent"" must be positive");
			}

			if (args.limit <= 0) {
				error("limit"" must be positive");
			}

			if (args.midpoint <= 0) {
				error("midpoint"" must be positive");
			}

			if (args.smooth < 0 || args.smooth > 1) {
				error("smooth must be between 0 and 1");
			}

		};

		check_accel(args.argsv.x);

		if (!args.combine_mags) {
			ret.count_x = ret.count;
			check_accel(args.argsv.y);
			ret.count_y = ret.count;
		}

		if (args.dpi <= 0) {
			error("dpi"" must be positive");
		}

		if (args.speed_max < 0) {
			error("speed cap is negative");
		}
		else if (args.speed_max < args.speed_min) {
			error("max speed is less than min speed");
		}

		if (args.degrees_snap < 0 || args.degrees_snap > 45) {
			error("snap angle must be between 0 and 45 degrees");
		}

		if (args.sens.x == 0 || args.sens.y == 0) {
			error("sens multiplier is 0");
		}

		if (args.dom_args.domain_weights.x <= 0 ||
			args.dom_args.domain_weights.y <= 0) {
			error("domain weights"" must be positive");
		}

		if (args.dir_multipliers.x <= 0 || args.dir_multipliers.y <= 0) {
			error("directional multipliers must be positive");
		}

		if (args.dom_args.lp_norm < 2) {
			error("Lp norm is less than 2 (default=2)");
		}

		if (args.range_weights.x <= 0 || args.range_weights.y <= 0) {
			error("range weights"" must be positive");
		}

		if (args.time_min <= 0) {
			error("minimum time"" must be positive");
		}

		if (args.time_max < args.time_min) {
			error("max time is less than min time");
		}

		return ret;
	}

}

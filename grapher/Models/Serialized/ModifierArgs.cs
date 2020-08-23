using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Serialized
{
    public enum accel_mode 
    {
        linear=1, classic, natural, logarithmic, sigmoid, power, naturalgain, sigmoidgain, noaccel
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Serializable]
    public struct vec2d 
    {
        public double x;
        public double y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Serializable]
    public struct accel_args
    {
        public double offset;
        public double accel;
        public double limit;
        public double exponent;
        public double midpoint;
        public double power_scale;
        public double gain_cap;
        public vec2d weight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Serializable]
    public struct accel_fn_args
    {
        public accel_args acc_args;
        public int accel_mode;
        public double time_min;
        public vec2d cap;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Serializable]
    public struct modifier_args
    {
        public double degrees;
        public vec2d sens;
        public accel_fn_args acc_fn_args;

        public modifier_args(ManagedAccel managedAccel)
        {
            degrees = managedAccel.Rotation;
            sens.x = managedAccel.SensitivityX;
            sens.y = managedAccel.SensitivityY;
            acc_fn_args.accel_mode = managedAccel.Type;
            acc_fn_args.time_min = managedAccel.MinimumTime;
            acc_fn_args.cap.x = managedAccel.CapX;
            acc_fn_args.cap.y = managedAccel.CapY;
            acc_fn_args.acc_args.accel = managedAccel.Acceleration;
            acc_fn_args.acc_args.exponent = managedAccel.LimitExp;
            acc_fn_args.acc_args.gain_cap = managedAccel.GainCap;
            acc_fn_args.acc_args.limit = managedAccel.LimitExp;
            acc_fn_args.acc_args.midpoint = managedAccel.Midpoint;
            acc_fn_args.acc_args.offset = managedAccel.Offset;
            acc_fn_args.acc_args.power_scale = managedAccel.PowerScale;
            acc_fn_args.acc_args.weight.x = managedAccel.WeightX;
            acc_fn_args.acc_args.weight.y = managedAccel.WeightY;
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace grapher.Models.Serialized
{
    public enum AccelMode
    {
        linear, classic, natural, logarithmic, sigmoid, naturalgain, sigmoidgain, power, noaccel
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccelArgs
    {
        public double offset;
        public double accel;
        public double limit;
        public double exponent;
        public double midpoint;
        public double powerScale;
        public double powerExponent;
        public double weight;
        public double rate;
        public double scaleCap;
        public double gainCap;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2 <T>
    {
        public T x;
        public T y;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public class DriverSettings
    {
        private static readonly IntPtr UnmanagedSettingsHandle = 
            Marshal.AllocHGlobal(Marshal.SizeOf<DriverSettings>());

        public double rotation;
        public bool combineMagnitudes;
        public Vec2<AccelMode> modes;
        public Vec2<AccelArgs> args;
        public Vec2<double> sensitivity;
        public double minimumTime;

        public static DriverSettings GetActive()
        {
            DriverInterop.GetActiveSettings(UnmanagedSettingsHandle);
            return Marshal.PtrToStructure<DriverSettings>(UnmanagedSettingsHandle);
        }

        public static void SetActive(DriverSettings settings)
        {
            Marshal.StructureToPtr(settings, UnmanagedSettingsHandle, false);
            DriverInterop.SetActiveSettings(UnmanagedSettingsHandle);
        }

        public void SendToDriver()
        {
            SetActive(this);
        }

        public void SendToDriverAndUpdate(ManagedAccel accel)
        {
            SendToDriver();
            accel.UpdateFromSettings(UnmanagedSettingsHandle);
        }

        public bool verify()
        {
            /*
            if (args.accel < 0 || args.rate < 0) 
                    bad_arg("accel can not be negative, use a negative weight to compensate");
            if (args.rate > 1) bad_arg("rate can not be greater than 1");
            if (args.exponent <= 1) bad_arg("exponent must be greater than 1");
            if (args.limit <= 1) bad_arg("limit must be greater than 1");
            if (args.power_scale <= 0) bad_arg("scale must be positive");
            if (args.power_exp <= 0) bad_arg("exponent must be positive");
            if (args.midpoint < 0) bad_arg("midpoint must not be negative");
            if (args.time_min <= 0) bad_arg("min time must be positive");
            */
            return true;
        }
    }
}

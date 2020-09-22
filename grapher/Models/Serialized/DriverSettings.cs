using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace grapher.Models.Serialized
{
    #region Enumerations

    public enum AccelMode
    {
        linear, classic, natural, naturalgain, power, logarithm, motivity, noaccel
    }

    #endregion Enumerations

    #region Structs

    [StructLayout(LayoutKind.Sequential)]
    public struct AccelArgs
    {
        public double offset;
        public double legacy_offset;
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

    #endregion Structs

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public class DriverSettings
    {
        #region Fields

        private static readonly IntPtr UnmanagedSettingsHandle = 
            Marshal.AllocHGlobal(Marshal.SizeOf<DriverSettings>());
        private static object UnmanagedSettingsLock = new object();

        public double rotation;
        public bool combineMagnitudes;
        public Vec2<AccelMode> modes;
        public Vec2<AccelArgs> args;
        public Vec2<double> sensitivity;
        public double minimumTime;

        #endregion Fields

        #region Methods

        public static DriverSettings GetActive()
        {
            DriverInterop.GetActiveSettings(UnmanagedSettingsHandle);
            return Marshal.PtrToStructure<DriverSettings>(UnmanagedSettingsHandle);
        }

        public static void SetActive(DriverSettings settings, Action<IntPtr> unmanagedActionBefore = null)
        {
            new Thread(() =>
            {
                lock (UnmanagedSettingsLock)
                {
                    Marshal.StructureToPtr(settings, UnmanagedSettingsHandle, false);
                    unmanagedActionBefore?.Invoke(UnmanagedSettingsHandle);
                    DriverInterop.SetActiveSettings(UnmanagedSettingsHandle);
                }
            }).Start();

        }

        public void SendToDriver(Action<IntPtr> unmanagedActionBefore = null)
        {
            SetActive(this, unmanagedActionBefore);
        }

        public void SendToDriverAndUpdate(ManagedAccel accel, Action betweenAccelAndWrite = null)
        {
            SendToDriver(settingsHandle =>
            {
                accel.UpdateFromSettings(settingsHandle);
                betweenAccelAndWrite?.Invoke();
            });
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

        #endregion Methods
    }
}

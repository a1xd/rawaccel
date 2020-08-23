using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Serialized
{
    public class SettingsManager
    {
        public SettingsManager(
            ManagedAccel activeAccel,
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWrite)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
        }

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        private Field DpiField { get; set; }

        private Field PollRateField { get; set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        public void UpdateActiveSettings(
            int mode,
            double degrees,
            double sensitivityX,
            double sensitivityY,
            double weightX,
            double weightY,
            double capX,
            double capY,
            double offset,
            double accel,
            double limitOrExp,
            double midpoint,
            double gainCap)
        {
            ActiveAccel.UpdateAccel(
                mode,
                degrees,
                sensitivityX,
                sensitivityY,
                weightX,
                weightY,
                capX,
                capY,
                offset,
                accel,
                limitOrExp,
                midpoint,
                gainCap);

            RawAccelSettings.AccelerationSettings = new modifier_args(ActiveAccel);
            RawAccelSettings.GUISettings = new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data
                };

            RawAccelSettings.Save();
        }

        public void UpdateActiveAccelFromFileSettings()
        {
            ActiveAccel.UpdateAccel(
                RawAccelSettings.AccelerationSettings.acc_fn_args.accel_mode,
                RawAccelSettings.AccelerationSettings.degrees,
                RawAccelSettings.AccelerationSettings.sens.x,
                RawAccelSettings.AccelerationSettings.sens.y,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.weight.x,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.weight.y,
                RawAccelSettings.AccelerationSettings.acc_fn_args.cap.x,
                RawAccelSettings.AccelerationSettings.acc_fn_args.cap.y,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.offset,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.accel,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.exponent,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.midpoint,
                RawAccelSettings.AccelerationSettings.acc_fn_args.acc_args.gain_cap);
            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
        }

        public void Startup()
        {
            ActiveAccel.ReadFromDriver();

            if(RawAccelSettings.Exists())
            {
                RawAccelSettings = RawAccelSettings.Load();
                if (RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup)
                {
                    UpdateActiveAccelFromFileSettings();
                }
            }
            else
            {
                RawAccelSettings = new RawAccelSettings(
                    ActiveAccel,
                    new GUISettings
                    {
                        AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                        DPI = (int)DpiField.Data,
                        PollRate = (int)PollRateField.Data
                    });
                RawAccelSettings.Save();
            }
        }
    }
}

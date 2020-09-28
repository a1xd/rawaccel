using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.Threading;

namespace grapher.Models.Serialized
{
    public class SettingsManager
    {
        #region Constructors

        public SettingsManager(
            ManagedAccel activeAccel,
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWrite,
            ToolStripMenuItem showLastMouseMove,
            ToolStripMenuItem showVelocityAndGain)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
        }

        #endregion Constructors

        #region Properties

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        private Field DpiField { get; set; }

        private Field PollRateField { get; set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        private ToolStripMenuItem ShowLastMouseMoveMenuItem { get; set; }

        private ToolStripMenuItem ShowVelocityAndGainMoveMenuItem { get; set; }

        #endregion Properties

        #region Methods

        public SettingsErrors TryUpdateActiveSettings(DriverSettings settings)
        {
            var errors = TryUpdateAccel(settings);

            if (errors.Empty())
            {
                RawAccelSettings.AccelerationSettings = settings;
                RawAccelSettings.GUISettings = new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data,
                    ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                    ShowVelocityAndGain = ShowVelocityAndGainMoveMenuItem.Checked,
                };

                RawAccelSettings.Save();
            }

            return errors;
        }

        public void UpdateActiveAccelFromFileSettings(DriverSettings settings)
        {
            TryUpdateAccel(settings);

            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
            ShowLastMouseMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowLastMouseMove;
            ShowVelocityAndGainMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowVelocityAndGain;
        }

        public SettingsErrors TryUpdateAccel(DriverSettings settings)
        {
            var errors = SendToDriverSafe(settings);
            if (errors.Empty()) ActiveAccel.UpdateFromSettings(settings);
            return errors;
        }

        public static void SendToDriver(DriverSettings settings)
        {
            new Thread(() => DriverInterop.Write(settings)).Start();
        }

        public static SettingsErrors SendToDriverSafe(DriverSettings settings)
        {
            var errors = DriverInterop.GetSettingsErrors(settings);
            if (errors.Empty()) SendToDriver(settings);
            return errors;
        }

        public void Startup()
        {
            if (RawAccelSettings.Exists())
            {
                try
                {
                    RawAccelSettings = RawAccelSettings.Load();
                    if (RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup)
                    {
                        UpdateActiveAccelFromFileSettings(RawAccelSettings.AccelerationSettings);
                    }
                    return;
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"bad settings: {e}");
                }
            }

            RawAccelSettings = new RawAccelSettings(
                DriverInterop.GetActiveSettings(),
                new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data,
                    ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                });
            RawAccelSettings.Save();
        }

        #endregion Methods
    }
}

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

        public void UpdateActiveSettings(DriverSettings settings)
        {
            ActiveAccel.UpdateFromSettings(settings);
            SendToDriver(settings);

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

        public void UpdateActiveAccelFromFileSettings(DriverSettings settings)
        { 
            ActiveAccel.UpdateFromSettings(settings);
            SendToDriver(settings);

            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
            ShowLastMouseMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowLastMouseMove;
            ShowVelocityAndGainMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowVelocityAndGain;
        }

        public static void SendToDriver(DriverSettings settings)
        {
            new Thread(() => DriverInterop.SetActiveSettings(settings)).Start();
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
                catch (JsonSerializationException e)
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

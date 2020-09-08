using Newtonsoft.Json;
using System;
using System.Windows.Forms;

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
            ToolStripMenuItem showLastMouseMove)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
        }

        #endregion Constructors

        #region Properties

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        private Field DpiField { get; set; }

        private Field PollRateField { get; set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        private ToolStripMenuItem ShowLastMouseMoveMenuItem { get; set; }

        #endregion Properties

        #region Methods

        public void UpdateActiveSettings(DriverSettings settings, Action afterAccelSettingsUpdate = null)
        {
            settings.SendToDriverAndUpdate(ActiveAccel, () =>
            {
                RawAccelSettings.AccelerationSettings = settings;
                RawAccelSettings.GUISettings = new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data,
                    ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                };

                RawAccelSettings.Save();

                afterAccelSettingsUpdate?.Invoke();
            });
        }

        public void UpdateActiveAccelFromFileSettings(DriverSettings settings)
        {
            settings.SendToDriverAndUpdate(ActiveAccel);

            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
            ShowLastMouseMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowLastMouseMove;
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
                DriverSettings.GetActive(),
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

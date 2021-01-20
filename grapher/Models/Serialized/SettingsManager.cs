using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Drawing;
using grapher.Models.Devices;

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
            ToolStripMenuItem showVelocityAndGain,
            DeviceIDManager deviceIDManager)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
            DeviceIDManager = deviceIDManager;
        }

        #endregion Constructors

        #region Properties

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        public Field DpiField { get; private set; }

        public Field PollRateField { get; private set; }

        public DeviceIDManager DeviceIDManager { get; }

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
                RawAccelSettings.GUISettings = MakeGUISettingsFromFields();
                RawAccelSettings.Save();
            }

            return errors;
        }

        public void UpdateFieldsFromGUISettings()
        {
            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            ShowLastMouseMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowLastMouseMove;
            ShowVelocityAndGainMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowVelocityAndGain;
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
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

        public GUISettings MakeGUISettingsFromFields()
        {
            return new GUISettings
            {
                DPI = (int)DpiField.Data,
                PollRate = (int)PollRateField.Data,
                ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                ShowVelocityAndGain = ShowVelocityAndGainMoveMenuItem.Checked,
                AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked
            };
        }

        // Returns true when file settings are active
        public bool Startup()
        {
            if (RawAccelSettings.Exists())
            {
                try
                {
                    RawAccelSettings = RawAccelSettings.Load(() => MakeGUISettingsFromFields());
                    UpdateFieldsFromGUISettings();
                    if (RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup)
                    {
                        TryUpdateAccel(RawAccelSettings.AccelerationSettings);
                    }
                    return RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"bad settings: {e}");
                }
            }

            RawAccelSettings = new RawAccelSettings(
                DriverInterop.GetActiveSettings(),
                MakeGUISettingsFromFields());
            RawAccelSettings.Save();
            return true;
        }

        #endregion Methods
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
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
            ToolStripMenuItem streamingMode,
            DeviceIDManager deviceIDManager)
        {
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
            StreamingModeMenuItem = streamingMode;
            DeviceIDManager = deviceIDManager;

            SetActiveFields(activeAccel);

            GuiSettings = GUISettings.MaybeLoad();

            if (GuiSettings is null)
            {
                GuiSettings = MakeGUISettingsFromFields();
                GuiSettings.Save();
            }
            else
            {
                UpdateFieldsFromGUISettings();
            }

            UserSettings = InitUserSettings();
        }

        #endregion Constructors

        #region Properties

        public GUISettings GuiSettings { get; private set; }

        public ManagedAccel ActiveAccel { get; private set; }

        public ExtendedSettings ActiveSettings { get; private set; }

        public DriverSettings UserSettings { get; private set; }

        public Field DpiField { get; private set; }

        public Field PollRateField { get; private set; }

        public DeviceIDManager DeviceIDManager { get; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        private ToolStripMenuItem ShowLastMouseMoveMenuItem { get; set; }

        private ToolStripMenuItem ShowVelocityAndGainMoveMenuItem { get; set; }
        private ToolStripMenuItem StreamingModeMenuItem{ get; set; }
        #endregion Properties

        #region Methods

        public void DisableDriver()
        {
            var defaultSettings = new ExtendedSettings();
            ActiveSettings = defaultSettings;
            ActiveAccel.Settings = defaultSettings;
            new Thread(() => ActiveAccel.Activate()).Start();
        }

        public void UpdateFieldsFromGUISettings()
        {
            DpiField.SetToEntered(GuiSettings.DPI);
            PollRateField.SetToEntered(GuiSettings.PollRate);
            ShowLastMouseMoveMenuItem.Checked = GuiSettings.ShowLastMouseMove;
            ShowVelocityAndGainMoveMenuItem.Checked = GuiSettings.ShowVelocityAndGain;
            StreamingModeMenuItem.Checked = GuiSettings.StreamingMode;
            AutoWriteMenuItem.Checked = GuiSettings.AutoWriteToDriverOnStartup;
        }

        public SettingsErrors TryActivate(DriverSettings settings)
        {
            var errors = new SettingsErrors(settings);

            if (errors.Empty())
            {
                GuiSettings = MakeGUISettingsFromFields();
                GuiSettings.Save();

                UserSettings = settings;
                File.WriteAllText(Constants.DefaultSettingsFileName, RaConvert.Settings(settings));

                ActiveSettings = new ExtendedSettings(settings);
                ActiveAccel.Settings = ActiveSettings;

                new Thread(() => ActiveAccel.Activate()).Start();
            }

            return errors;
        }

        public void SetHiddenOptions(DriverSettings settings)
        {
            settings.snap = UserSettings.snap;
            settings.maximumSpeed = UserSettings.maximumSpeed;
            settings.minimumSpeed = UserSettings.minimumSpeed;
            settings.minimumTime = UserSettings.minimumTime;
            settings.maximumTime = UserSettings.maximumTime;
            settings.ignore = UserSettings.ignore;
            settings.directionalMultipliers = UserSettings.directionalMultipliers;
        }

        public GUISettings MakeGUISettingsFromFields()
        {
            return new GUISettings
            {
                DPI = (int)DpiField.Data,
                PollRate = (int)PollRateField.Data,
                ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                ShowVelocityAndGain = ShowVelocityAndGainMoveMenuItem.Checked,
                AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                StreamingMode = StreamingModeMenuItem.Checked
            };
        }

        public bool TableActive()
        {
            return ActiveSettings.tables.x != null || ActiveSettings.tables.y != null;
        }

        public void SetActiveFields(ManagedAccel activeAccel)
        {
            ActiveAccel = activeAccel;
            ActiveSettings = activeAccel.Settings;
        }

        private DriverSettings InitUserSettings()
        {
            var path = Constants.DefaultSettingsFileName;
            if (File.Exists(path))
            {
                try
                {
                    DriverSettings settings = RaConvert.Settings(File.ReadAllText(path));

                    if (!GuiSettings.AutoWriteToDriverOnStartup || 
                        TableActive() || 
                        TryActivate(settings).Empty())
                    {
                        return settings;
                    }

                }
                catch (JsonException e)
                {
                    System.Diagnostics.Debug.WriteLine($"bad settings: {e}");
                }
            }

            if (!TableActive())
            {
                File.WriteAllText(path, RaConvert.Settings(ActiveSettings.baseSettings));
                return ActiveSettings.baseSettings;
            }
            else
            {
                var defaultSettings = new DriverSettings();
                File.WriteAllText(path, RaConvert.Settings(defaultSettings));
                return defaultSettings;
            }
        }

        #endregion Methods
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Drawing;
using grapher.Models.Devices;
using System.Collections.Generic;

namespace grapher.Models.Serialized
{
    public class SettingsManager
    {
        #region Constructors

        public SettingsManager(
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWrite,
            ToolStripMenuItem showLastMouseMove,
            ToolStripMenuItem showVelocityAndGain,
            ToolStripMenuItem streamingMode,
            ToolStripMenuItem deviceMenuItem)
        {
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
            StreamingModeMenuItem = streamingMode;
            deviceMenuItem.Click += (s, e) => new DeviceMenuForm(this).ShowDialog();

            SystemDevices = new List<MultiHandleDevice>();
            ActiveHandles = new List<IntPtr>();

            // TODO - remove ActiveConfig/AutoWrite entirely?
            // shouldn't be needed with internal profiles support
            ActiveConfig = DriverConfig.GetActive();

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

            UserConfig = InitUserSettings();
        }

        #endregion Constructors

        #region Properties

        public GUISettings GuiSettings { get; private set; }

        public DriverConfig ActiveConfig { get; private set; }

        public Profile ActiveProfile 
        {
            get => ActiveConfig.profiles[0];
        }

        public ManagedAccel ActiveAccel
        {
            get => ActiveConfig.accels[0];
        }

        public DriverConfig UserConfig { get; private set; }

        public Profile UserProfile 
        {
            get => UserConfig.profiles[0];
        }

        public Field DpiField { get; private set; }

        public Field PollRateField { get; private set; }

        public IList<MultiHandleDevice> SystemDevices { get; private set; }

        public List<IntPtr> ActiveHandles { get; private set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        private ToolStripMenuItem ShowLastMouseMoveMenuItem { get; set; }

        private ToolStripMenuItem ShowVelocityAndGainMoveMenuItem { get; set; }

        private ToolStripMenuItem StreamingModeMenuItem{ get; set; }
        #endregion Properties

        #region Methods

        public void DisableDriver()
        {
            ActiveConfig = DriverConfig.GetDefault();
            new Thread(() => DriverConfig.Deactivate()).Start();
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

        public bool TryActivate(DriverConfig settings, out string errors)
        {
            errors = settings.Errors();

            if (errors == null)
            {
                GuiSettings = MakeGUISettingsFromFields();
                GuiSettings.Save();

                UserConfig = settings;
                ActiveConfig = settings;
                File.WriteAllText(Constants.DefaultSettingsFileName, settings.ToJSON());

                new Thread(() => ActiveConfig.Activate()).Start();
            }

            return errors == null;
        }

        public void SetHiddenOptions(Profile settings)
        {
            settings.snap = UserProfile.snap;
            settings.maximumSpeed = UserProfile.maximumSpeed;
            settings.minimumSpeed = UserProfile.minimumSpeed;
            settings.directionalMultipliers = UserProfile.directionalMultipliers;
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

        private void SetActiveHandles()
        {
            ActiveHandles.Clear();
            // TODO
            foreach (var sysDev in SystemDevices)
            {
                ActiveHandles.AddRange(sysDev.handles);
            }
        }

        private void OnProfileSwitch()
        {
            SetActiveHandles();
        }

        public void OnDeviceChangeMessage()
        {
            SystemDevices = MultiHandleDevice.GetList();
            SetActiveHandles();
        }

        private DriverConfig InitUserSettings()
        {
            var path = Constants.DefaultSettingsFileName;
            if (File.Exists(path))
            {
                try
                {
                    var (cfg, err) = DriverConfig.Convert(File.ReadAllText(path));

                    if (!GuiSettings.AutoWriteToDriverOnStartup || 
                        (err == null && TryActivate(cfg, out string _)))
                    {
                        return cfg;
                    }

                }
                catch (JsonException e)
                {
                    System.Diagnostics.Debug.WriteLine($"bad settings: {e}");
                }
            }

            File.WriteAllText(path, ActiveConfig.ToJSON());
            return ActiveConfig;
        }

        #endregion Methods
    }
}

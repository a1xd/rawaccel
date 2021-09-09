using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

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

            UserConfig = InitActiveAndGetUserConfig();
        }

        #endregion Constructors

        #region Fields

        private DriverConfig ActiveConfigField;

        #endregion Fields

        #region Properties

        public GUISettings GuiSettings { get; private set; }

        public DriverConfig ActiveConfig 
        {
            get => ActiveConfigField;

            private set
            {
                if (ActiveConfigField != value)
                {
                    ActiveConfigField = value;
                    ActiveProfileNamesSet = new HashSet<string>(value.profiles.Select(p => p.name));
                }
            }
        }

        public Profile ActiveProfile 
        {
            get => ActiveConfigField.profiles[0];
            private set => ActiveConfigField.SetProfileAt(0, value);
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

        public HashSet<string> ActiveProfileNamesSet { get; private set; }

        public Field DpiField { get; private set; }

        public Field PollRateField { get; private set; }

        public IList<MultiHandleDevice> SystemDevices { get; private set; }

        public List<IntPtr> ActiveHandles { get; }

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

        public bool TryActivate(Profile settings, out string errors)
        {
            var old = ActiveProfile;
            ActiveProfile = settings;
            bool success = TryActivate(ActiveConfig, out errors);
            if (!success)
            {
                ActiveProfile = old;
            }
            return success;
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

            bool ActiveProfileIsFirst = ActiveProfile == ActiveConfig.profiles[0];

            foreach (var dev in SystemDevices) MaybeAdd(dev);

            void MaybeAdd(MultiHandleDevice dev)
            {
                foreach (var settings in ActiveConfig.devices)
                {
                    if (settings.id == dev.id)
                    {
                        if (!settings.config.disable && 
                            ((ActiveProfileIsFirst &&
                                    (string.IsNullOrEmpty(settings.profile) || 
                                        !ActiveProfileNamesSet.Contains(settings.profile))) || 
                                ActiveProfile.name == settings.profile))
                        {
                            ActiveHandles.AddRange(dev.handles);
                        }

                        return;
                    }
                }

                if (ActiveProfileIsFirst && !ActiveConfig.defaultDeviceConfig.disable)
                {
                    ActiveHandles.AddRange(dev.handles);
                }
            }
        }

        public void OnProfileSelectionChange()
        {
            SetActiveHandles();
        }

        public void OnDeviceChangeMessage()
        {
            SystemDevices = MultiHandleDevice.GetList();
            SetActiveHandles();
        }

        private DriverConfig InitActiveAndGetUserConfig()
        {
            var path = Constants.DefaultSettingsFileName;
            if (File.Exists(path))
            {
                try
                {
                    var (cfg, err) = DriverConfig.Convert(File.ReadAllText(path));

                    if (err == null)
                    {
                        if (GuiSettings.AutoWriteToDriverOnStartup)
                        {
                            if (!TryActivate(cfg, out string _))
                            {
                                throw new Exception("deserialization succeeded but TryActivate failed");
                            }
                        }
                        else
                        {
                            ActiveConfig = DriverConfig.GetActive();
                        }

                        return cfg;
                    }
                }
                catch (JsonException e)
                {
                    System.Diagnostics.Debug.WriteLine($"bad settings: {e}");
                }
            }

            ActiveConfig = DriverConfig.GetActive();
            File.WriteAllText(path, ActiveConfig.ToJSON());
            return ActiveConfig;
        }

        #endregion Methods
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Drawing;
using grapher.Models.Devices;
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
            ToolStripMenuItem streamingMode)
        {
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
            StreamingModeMenuItem = streamingMode;

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

            UserConfigField = InitActiveAndGetUserConfig();
        }

        #endregion Constructors

        #region Fields

        private EventHandler DeviceChangeField;

        private DriverConfig ActiveConfigField;
        private DriverConfig UserConfigField;

        #endregion Fields

        #region Properties

        public GUISettings GuiSettings { get; private set; }

        public event EventHandler DeviceChange
        {
            add => DeviceChangeField += value;
            remove => DeviceChangeField -= value;
        }

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

        public DriverConfig UserConfig 
        { 
            get => UserConfigField;
            private set => UserConfigField = value;
        }

        public Profile UserProfile 
        {
            get => UserConfigField.profiles[0];
            private set => UserConfigField.SetProfileAt(0, value);
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

        public void ResetDriver()
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
            var old = UserProfile;
            UserProfile = settings;
            bool success = TryActivate(UserConfig, out errors);
            if (!success)
            {
                UserProfile = old;
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
            settings.lrSensRatio = UserProfile.lrSensRatio;
            settings.udSensRatio = UserProfile.udSensRatio;
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

        public void SetActiveHandles()
        {
            ActiveHandles.Clear();

            bool ActiveProfileIsFirst = ActiveProfile == ActiveConfig.profiles[0];

            foreach (var sysDev in SystemDevices)
            {
                var settings = ActiveConfig.devices.Find(d => d.id == sysDev.id);

                if (settings is null)
                {
                    if (ActiveProfileIsFirst && !ActiveConfig.defaultDeviceConfig.disable)
                    {
                        ActiveHandles.AddRange(sysDev.handles);
                    }
                }
                else if (!settings.config.disable &&
                            ((ActiveProfileIsFirst &&
                                    (string.IsNullOrEmpty(settings.profile) ||
                                        !ActiveProfileNamesSet.Contains(settings.profile))) ||
                                ActiveProfile.name == settings.profile))
                {
                    ActiveHandles.AddRange(sysDev.handles);
                }
            }
        }

        public void Submit(DeviceConfig newDefaultConfig, DeviceDialogItem[] items)
        {
            UserConfig.defaultDeviceConfig = newDefaultConfig;
            foreach (var item in items)
            {
                if (item.overrideDefaultConfig)
                {
                    if (item.oldSettings is null)
                    {
                        UserConfig.devices.Add(
                            new DeviceSettings
                            {
                                name = item.device.name,
                                profile = item.newProfile,
                                id = item.device.id,
                                config = item.newConfig
                            });
                    }
                    else
                    {
                        item.oldSettings.config = item.newConfig;
                        item.oldSettings.profile = item.newProfile;
                    }
                }
                else if (!(item.oldSettings is null))
                {
                    UserConfig.devices.Remove(item.oldSettings);
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

            DeviceChangeField?.Invoke(this, EventArgs.Empty);
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

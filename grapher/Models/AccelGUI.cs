using grapher.Models.Calculations;
using grapher.Models.Devices;
using grapher.Models.Mouse;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelGUI
    {

        #region Constructors

        public AccelGUI(
            RawAcceleration accelForm,
            AccelCalculator accelCalculator,
            AccelCharts accelCharts,
            SettingsManager settings,
            ApplyOptions applyOptions,
            Button writeButton,
            ButtonBase toggleButton,
            MouseWatcher mouseWatcher,
            ToolStripMenuItem scaleMenuItem,
            DeviceIDManager deviceIDManager)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            WriteButton = writeButton;
            ToggleButton = (CheckBox)toggleButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            DefaultButtonFont = WriteButton.Font;
            SmallButtonFont = new Font(WriteButton.Font.Name, WriteButton.Font.Size * Constants.SmallButtonSizeFactor);
            MouseWatcher = mouseWatcher;
            DeviceIDManager = deviceIDManager;

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
            WriteButton.Click += new System.EventHandler(OnWriteButtonClick);
            ToggleButton.Click += new System.EventHandler(OnToggleButtonClick);
            AccelForm.FormClosing += new FormClosingEventHandler(SaveGUISettingsOnClose);

            ButtonTimerInterval = Convert.ToInt32(DriverInterop.WriteDelayMs);
            ButtonTimer = new Timer();
            ButtonTimer.Tick += new System.EventHandler(OnButtonTimerTick);

            ChartRefresh = SetupChartTimer();

            bool settingsActive = Settings.Startup();
            SettingsNotDefault = !Settings.RawAccelSettings.IsDefaultEquivalent();

            if (settingsActive)
            {
                LastToggleChecked = SettingsNotDefault;
                ToggleButton.Enabled = LastToggleChecked;
                RefreshOnRead(Settings.RawAccelSettings.AccelerationSettings);
            }
            else
            {
                DriverSettings active = DriverInterop.GetActiveSettings();
                bool activeNotDefault = !RawAccelSettings.IsDefaultEquivalent(active);

                LastToggleChecked = activeNotDefault;
                ToggleButton.Enabled = SettingsNotDefault || activeNotDefault;
                RefreshOnRead(active);
            }

            SetupButtons();

            // TODO: The below removes an overlapping form from the anisotropy panel.
            // Figure out why and remove the overlap and below.
            ApplyOptions.Directionality.Show();
            ApplyOptions.Directionality.Hide();
        }

        #endregion Constructors

        #region Properties

        public RawAcceleration AccelForm { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public SettingsManager Settings { get; }

        public ApplyOptions ApplyOptions { get; }

        public Button WriteButton { get; }

        public CheckBox ToggleButton { get; }

        public Timer ButtonTimer { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        public DeviceIDManager DeviceIDManager { get; }

        public Action UpdateInputManagers { get; private set; }

        private Timer ChartRefresh { get; }

        private Font SmallButtonFont { get; }

        private Font DefaultButtonFont { get; }

        private bool SettingsNotDefault { get; set; }

        private bool LastToggleChecked { get; set; }

        private int ButtonTimerInterval { get; }

        #endregion Properties

        #region Methods

        private void SaveGUISettingsOnClose(Object sender, FormClosingEventArgs e)
        {
            var guiSettings = Settings.MakeGUISettingsFromFields();
            if (!Settings.RawAccelSettings.GUISettings.Equals(guiSettings))
            {
                Settings.RawAccelSettings.GUISettings = guiSettings;
                Settings.RawAccelSettings.Save();
            }
        }

        public void UpdateActiveSettingsFromFields()
        {
            var driverSettings = Settings.RawAccelSettings.AccelerationSettings;

            var newArgs = ApplyOptions.GetArgs();
            newArgs.x.speedCap = driverSettings.args.x.speedCap;
            newArgs.y.speedCap = driverSettings.args.y.speedCap;

            var settings = new DriverSettings
            {
                rotation = ApplyOptions.Rotation.Field.Data,
                snap = driverSettings.snap,
                sensitivity = new Vec2<double>
                {
                    x = ApplyOptions.Sensitivity.Fields.X,
                    y = ApplyOptions.Sensitivity.Fields.Y
                },
                combineMagnitudes = ApplyOptions.IsWhole,
                modes = ApplyOptions.GetModes(),
                args = newArgs,
                minimumTime = driverSettings.minimumTime,
                directionalMultipliers = driverSettings.directionalMultipliers,
                domainArgs = ApplyOptions.Directionality.GetDomainArgs(),
                rangeXY = ApplyOptions.Directionality.GetRangeXY(),
                deviceID = DeviceIDManager.ID,
            };

            ButtonDelay(WriteButton);
            SettingsErrors errors = Settings.TryUpdateActiveSettings(settings);
            if (errors.Empty())
            {
                SettingsNotDefault = !Settings.RawAccelSettings.IsDefaultEquivalent();
                LastToggleChecked = SettingsNotDefault;
                RefreshOnRead(Settings.RawAccelSettings.AccelerationSettings);
            }
            else
            {
                new MessageDialog(errors.ToString(), "bad input").ShowDialog();
            }
        }

        public void RefreshOnRead(DriverSettings args)
        {
            UpdateShownActiveValues(args);
            UpdateGraph(args);

            UpdateInputManagers = () =>
            {
                MouseWatcher.UpdateHandles(args.deviceID);
                DeviceIDManager.Update(args.deviceID);
            };

            UpdateInputManagers();
        }

        public void UpdateGraph(DriverSettings args)
        {
            AccelCharts.Calculate(
                Settings.ActiveAccel,
                args);
            AccelCharts.Bind();
        }

        public void UpdateShownActiveValues(DriverSettings args)
        {
            AccelForm.ResetAutoScroll();
            AccelCharts.ShowActive(args);
            ApplyOptions.SetActiveValues(args);
        }

        private Timer SetupChartTimer()
        {
            Timer chartTimer = new Timer();
            chartTimer.Enabled = true;
            chartTimer.Interval = 10;
            chartTimer.Tick += new System.EventHandler(OnChartTimerTick);
            return chartTimer;
        }

        private void SetupButtons()
        {
            WriteButton.Top = Constants.SensitivityChartAloneHeight - Constants.ButtonVerticalOffset;
            
            ToggleButton.Appearance = Appearance.Button;
            ToggleButton.FlatStyle = FlatStyle.System;
            ToggleButton.TextAlign = ContentAlignment.MiddleCenter;
            ToggleButton.Size = WriteButton.Size;
            ToggleButton.Top = WriteButton.Top;

            SetButtonDefaults();
        }

        private void SetButtonDefaults()
        {
            ToggleButton.Checked = LastToggleChecked;

            ToggleButton.Font = DefaultButtonFont;
            ToggleButton.Text = ToggleButton.Checked ? "Disable" : "Enable";
            ToggleButton.Update();

            WriteButton.Font = DefaultButtonFont;
            WriteButton.Text = Constants.WriteButtonDefaultText;
            WriteButton.Enabled = ToggleButton.Checked || !ToggleButton.Enabled;
            WriteButton.Update();
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph(Settings.RawAccelSettings.AccelerationSettings);
        }

        private void OnWriteButtonClick(object sender, EventArgs e)
        {
            UpdateActiveSettingsFromFields();
        }

        private void OnToggleButtonClick(object sender, EventArgs e)
        {
            var settings = ToggleButton.Checked ?
                Settings.RawAccelSettings.AccelerationSettings :
                DriverInterop.DefaultSettings;

            LastToggleChecked = ToggleButton.Checked;
            ButtonDelay(ToggleButton);

            SettingsManager.SendToDriver(settings);
            Settings.ActiveAccel.UpdateFromSettings(settings);
            RefreshOnRead(settings);
        }

        private void OnButtonTimerTick(object sender, EventArgs e)
        {
            ButtonTimer.Stop();
            ToggleButton.Enabled = SettingsNotDefault;
            SetButtonDefaults();
        }

        private void StartButtonTimer()
        {
            ButtonTimer.Interval = ButtonTimerInterval;
            ButtonTimer.Start();
        }

        private void ButtonDelay(ButtonBase btn)
        {
            ToggleButton.Checked = false;

            ToggleButton.Enabled = false;
            WriteButton.Enabled = false;

            btn.Font = SmallButtonFont;
            btn.Text = $"{Constants.ButtonDelayText} : {ButtonTimerInterval} ms";

            ToggleButton.Update();
            WriteButton.Update();

            StartButtonTimer();
        }

        private void OnChartTimerTick(object sender, EventArgs e)
        {
            AccelCharts.DrawLastMovement();
            MouseWatcher.UpdateLastMove();
        }

        #endregion Methods
    }

}

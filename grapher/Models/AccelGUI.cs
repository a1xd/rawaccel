using grapher.Models.Calculations;
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
            ToolStripMenuItem scaleMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            WriteButton = writeButton;
            ToggleButton = (CheckBox)toggleButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            Settings.Startup();
            RefreshOnRead(Settings.RawAccelSettings.AccelerationSettings);
            AccelForm.DoResize();

            DefaultButtonFont = WriteButton.Font;
            SmallButtonFont = new Font(WriteButton.Font.Name, WriteButton.Font.Size * 0.666f);

            MouseWatcher = mouseWatcher;

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
            WriteButton.Click += new System.EventHandler(OnWriteButtonClick);
            ToggleButton.Click += new System.EventHandler(OnToggleButtonClick);
            AccelForm.FormClosing += new FormClosingEventHandler(SaveGUISettingsOnClose);

            ButtonTimerInterval = Convert.ToInt32(DriverInterop.WriteDelayMs);
            ButtonTimer = new Timer();
            ButtonTimer.Tick += new System.EventHandler(OnButtonTimerTick);
            SetupButtons();

            ChartRefresh = SetupChartTimer();
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

            var settings = new DriverSettings
            {
                rotation = ApplyOptions.Rotation.Field.Data,
                sensitivity = new Vec2<double>
                {
                    x = ApplyOptions.Sensitivity.Fields.X,
                    y = ApplyOptions.Sensitivity.Fields.Y
                },
                combineMagnitudes = ApplyOptions.IsWhole,
                modes = ApplyOptions.GetModes(),
                args = ApplyOptions.GetArgs(),
                minimumTime = driverSettings.minimumTime
            };

            WriteButtonDelay();
            SettingsErrors errors = Settings.TryUpdateActiveSettings(settings);
            if (errors.Empty())
            {
                RefreshToggleStateFromNewSettings();
                RefreshOnRead(Settings.RawAccelSettings.AccelerationSettings);
            }
            else
            {
                throw new Exception($"Bad arguments: \n {SettingsManager.ErrorStringFrom(errors)}");
            }
        }

        public void RefreshOnRead(DriverSettings args)
        {
            UpdateShownActiveValues(args);
            UpdateGraph(args);
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
            WriteButton.Top = AccelCharts.Top + AccelCharts.TopChartHeight - Constants.ButtonVerticalOffset;
            
            ToggleButton.Appearance = Appearance.Button;
            ToggleButton.FlatStyle = FlatStyle.System;
            ToggleButton.TextAlign = ContentAlignment.MiddleCenter;
            ToggleButton.Size = WriteButton.Size;
            ToggleButton.Top = WriteButton.Top;

            RefreshToggleStateFromNewSettings();
            SetToggleButtonDefault();
            SetWriteButtonDefault();
        }

        private void RefreshToggleStateFromNewSettings()
        {
            SettingsNotDefault = !Settings.RawAccelSettings.IsDefaultEquivalent();
            LastToggleChecked = SettingsNotDefault;
        }

        private void SetWriteButtonDefault()
        {
            WriteButton.Font = DefaultButtonFont;
            WriteButton.Text = Constants.WriteButtonDefaultText;
            WriteButton.Enabled = ToggleButton.Checked || !ToggleButton.Enabled;
            WriteButton.Update();
        }

        private void SetToggleButtonDefault()
        {
            ToggleButton.Checked = LastToggleChecked;
            ToggleButton.Enabled = SettingsNotDefault;
            ToggleButton.Font = DefaultButtonFont;
            ToggleButton.Text = ToggleButton.Checked ? "Enabled" : "Disabled";
            ToggleButton.Update();
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

            ToggleButtonDelay();

            SettingsManager.SendToDriver(settings);
            Settings.ActiveAccel.UpdateFromSettings(settings);
            RefreshOnRead(settings);
        }

        private void OnButtonTimerTick(object sender, EventArgs e)
        {
            ButtonTimer.Stop();
            SetToggleButtonDefault();
            SetWriteButtonDefault();
        }

        private void StartButtonTimer()
        {
            ButtonTimer.Interval = ButtonTimerInterval;
            ButtonTimer.Start();
        }

        private void WriteButtonDelay()
        {
            WriteButton.Font = SmallButtonFont;
            WriteButton.Text = $"{Constants.ButtonDelayText} : {ButtonTimerInterval} ms";
            WriteButton.Enabled = false;
            WriteButton.Update();

            if (ToggleButton.Enabled)
            {
                LastToggleChecked = ToggleButton.Checked;
                ToggleButton.Checked = false;
                ToggleButton.Enabled = false;
                ToggleButton.Update();
            }
            StartButtonTimer();
        }

        private void ToggleButtonDelay()
        { 
            LastToggleChecked = ToggleButton.Checked;
            ToggleButton.Checked = false;
            ToggleButton.Enabled = false;
            ToggleButton.Font = SmallButtonFont;
            ToggleButton.Text = $"{Constants.ButtonDelayText} : {ButtonTimerInterval} ms";
            ToggleButton.Update();

            WriteButton.Enabled = false;
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

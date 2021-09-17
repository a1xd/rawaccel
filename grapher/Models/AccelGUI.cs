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
            ButtonBase resetButton,
            MouseWatcher mouseWatcher,
            ToolStripMenuItem scaleMenuItem,
            DeviceIDManager deviceIDManager)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            WriteButton = writeButton;
            ResetButton = (CheckBox)resetButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            DefaultButtonFont = WriteButton.Font;
            SmallButtonFont = new Font(WriteButton.Font.Name, WriteButton.Font.Size * Constants.SmallButtonSizeFactor);
            MouseWatcher = mouseWatcher;
            DeviceIDManager = deviceIDManager;

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
            WriteButton.Click += new System.EventHandler(OnWriteButtonClick);
            ResetButton.Click += new System.EventHandler(ResetDriverEventHandler);
            AccelForm.FormClosing += new FormClosingEventHandler(SaveGUISettingsOnClose);

            ButtonTimerInterval = Convert.ToInt32(DriverSettings.WriteDelayMs);
            ButtonTimer = new Timer();
            ButtonTimer.Tick += new System.EventHandler(OnButtonTimerTick);

            ChartRefresh = SetupChartTimer();

            RefreshUser();
            RefreshActive();
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

        public CheckBox ResetButton { get; }

        public Timer ButtonTimer { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        public DeviceIDManager DeviceIDManager { get; }

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
            if (!Settings.GuiSettings.Equals(guiSettings))
            {
                guiSettings.Save();
            }
        }

        public DriverSettings MakeSettingsFromFields()
        {
            var settings = new DriverSettings();

            settings.rotation = ApplyOptions.Rotation.Field.Data;
            settings.sensitivity = new Vec2<double>
            {
                x = ApplyOptions.Sensitivity.Fields.X,
                y = ApplyOptions.Sensitivity.Fields.Y
            };
            settings.combineMagnitudes = ApplyOptions.IsWhole;
            ApplyOptions.SetArgs(ref settings.args);
            settings.domainArgs = ApplyOptions.Directionality.GetDomainArgs();
            settings.rangeXY = ApplyOptions.Directionality.GetRangeXY();
            settings.deviceID = DeviceIDManager.ID;

            Settings.SetHiddenOptions(settings);

            return settings;
        }

        public void UpdateActiveSettingsFromFields()
        {
            string error_message;

            try
            {
                ButtonDelay(WriteButton);

                var settings = MakeSettingsFromFields();
                SettingsErrors errors = Settings.TryActivate(settings);
                if (errors.Empty())
                {
                    RefreshActive();
                    return;
                }
                else
                {
                    error_message = errors.ToString();
                }
            }
            catch (ApplicationException e)
            {
                error_message = e.Message;
            }

            new MessageDialog(error_message, "bad input").ShowDialog();
        }


        public void UpdateInputManagers()
        {
            MouseWatcher.UpdateHandles(Settings.ActiveSettings.baseSettings.deviceID);
            DeviceIDManager.Update(Settings.ActiveSettings.baseSettings.deviceID);
        }

        public void RefreshActive()
        {
            UpdateShownActiveValues(Settings.UserSettings);
            UpdateGraph();
            UpdateInputManagers();
        }

        public void RefreshUser()
        {
            UpdateShownActiveValues(Settings.UserSettings);
        }

        public void UpdateGraph()
        {
            AccelCharts.Calculate(
                Settings.ActiveAccel,
                Settings.ActiveSettings.baseSettings);
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

            ResetButton.Appearance = Appearance.Button;
            ResetButton.FlatStyle = FlatStyle.System;
            ResetButton.TextAlign = ContentAlignment.MiddleCenter;
            ResetButton.Size = WriteButton.Size;
            ResetButton.Top = WriteButton.Top;

            SetButtonDefaults();
        }

        private void SetButtonDefaults()
        {
            ResetButton.Font = DefaultButtonFont;
            ResetButton.Text = Constants.ResetButtonText;
            ResetButton.Enabled = true;
            ResetButton.Update();

            WriteButton.Font = DefaultButtonFont;
            WriteButton.Text = Constants.WriteButtonDefaultText;
            WriteButton.Enabled = true;
            WriteButton.Update();
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void OnWriteButtonClick(object sender, EventArgs e)
        {
            UpdateActiveSettingsFromFields();
        }

        private void ResetDriverEventHandler(object sender, EventArgs e)
        {
            ButtonDelay(ResetButton);
            Settings.DisableDriver();
            RefreshActive();
        }

        private void OnButtonTimerTick(object sender, EventArgs e)
        {
            ButtonTimer.Stop();
            SetButtonDefaults();
        }

        private void StartButtonTimer()
        {
            ButtonTimer.Interval = ButtonTimerInterval;
            ButtonTimer.Start();
        }

        private void ButtonDelay(ButtonBase btn)
        {
            ResetButton.Enabled = false;
            WriteButton.Enabled = false;

            btn.Font = SmallButtonFont;
            btn.Text = $"{Constants.ButtonDelayText} : {ButtonTimerInterval} ms";

            ResetButton.Update();
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

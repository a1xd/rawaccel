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
            ToolStripMenuItem scaleMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            WriteButton = writeButton;
            DisableButton = (CheckBox)toggleButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            DefaultButtonFont = WriteButton.Font;
            SmallButtonFont = new Font(WriteButton.Font.Name, WriteButton.Font.Size * Constants.SmallButtonSizeFactor);
            MouseWatcher = mouseWatcher;

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
            WriteButton.Click += new System.EventHandler(OnWriteButtonClick);
            DisableButton.Click += new System.EventHandler(DisableDriverEventHandler);
            AccelForm.FormClosing += new FormClosingEventHandler(SaveGUISettingsOnClose);

            ButtonTimerInterval = Convert.ToInt32(DriverConfig.WriteDelayMs);
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

        public CheckBox DisableButton { get; }

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
            if (!Settings.GuiSettings.Equals(guiSettings))
            {
                guiSettings.Save();
            }
        }

        public void UpdateActiveSettingsFromFields()
        {
            var settings = new Profile();

            settings.rotation = ApplyOptions.Rotation.Field.Data;
            settings.sensitivity = ApplyOptions.Sensitivity.Fields.X;

            // TODO - separate sensitivity fields, add new label for ratio
            settings.yxSensRatio = ApplyOptions.Sensitivity.Fields.Y;
            settings.combineMagnitudes = ApplyOptions.IsWhole;
            ApplyOptions.SetArgs(ref settings.argsX, ref settings.argsY);

            var (domWeights, lpNorm) = ApplyOptions.Directionality.GetDomainArgs();
            settings.domainXY = domWeights;
            settings.lpNorm = lpNorm;

            settings.rangeXY = ApplyOptions.Directionality.GetRangeXY();

            Settings.SetHiddenOptions(settings);

            ButtonDelay(WriteButton);

            if (!Settings.TryActivate(settings, out string errors))
            {
                new MessageDialog(errors, "bad input").ShowDialog();
            }
            else
            {
                RefreshActive();
            }
        }

        public void RefreshActive()
        {
            UpdateShownActiveValues(Settings.ActiveProfile);
            UpdateGraph();
        }

        public void RefreshUser()
        {
            UpdateShownActiveValues(Settings.UserProfile);
        }

        public void UpdateGraph()
        {
            AccelCharts.Calculate(Settings.ActiveAccel, Settings.ActiveProfile);
            AccelCharts.Bind();
        }

        public void UpdateShownActiveValues(Profile args)
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
            
            DisableButton.Appearance = Appearance.Button;
            DisableButton.FlatStyle = FlatStyle.System;
            DisableButton.TextAlign = ContentAlignment.MiddleCenter;
            DisableButton.Size = WriteButton.Size;
            DisableButton.Top = WriteButton.Top;

            SetButtonDefaults();
        }

        private void SetButtonDefaults()
        {
            DisableButton.Font = DefaultButtonFont;
            DisableButton.Text = "Disable";
            DisableButton.Enabled = true;
            DisableButton.Update();

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

        private void DisableDriverEventHandler(object sender, EventArgs e)
        {
            ButtonDelay(DisableButton);
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
            DisableButton.Enabled = false;
            WriteButton.Enabled = false;

            btn.Font = SmallButtonFont;
            btn.Text = $"{Constants.ButtonDelayText} : {ButtonTimerInterval} ms";

            DisableButton.Update();
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

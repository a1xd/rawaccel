using grapher.Models.Calculations;
using grapher.Models.Mouse;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.Windows.Forms;

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
            Label mouseMoveLabel,
            ToolStripMenuItem scaleMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            WriteButton = writeButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            Settings.Startup();
            RefreshOnRead();

            MouseWatcher = new MouseWatcher(AccelForm, mouseMoveLabel, AccelCharts);

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
            WriteButton.Click += new System.EventHandler(OnWriteButtonClick);

            ButtonTimer = SetupButtonTimer();
            SetupWriteButton();
        }

        #endregion Constructors

        #region Properties

        public RawAcceleration AccelForm { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public SettingsManager Settings { get; }

        public ApplyOptions ApplyOptions { get; }

        public Button WriteButton { get; }

        public Timer ButtonTimer { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        #endregion Properties

        #region Methods

        public void UpdateActiveSettingsFromFields()
        {
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
                minimumTime = .4
            };

            WriteButtonDelay();
            Settings.UpdateActiveSettings(settings);
            RefreshOnRead();
        }

        public void RefreshOnRead()
        {
            UpdateShownActiveValues();
            UpdateGraph();
        }

        public void UpdateGraph()
        {
            AccelCharts.Calculate(
                Settings.ActiveAccel, 
                Settings.RawAccelSettings.AccelerationSettings);
            AccelCharts.Bind();
        }

        public void UpdateShownActiveValues()
        {
            var settings = Settings.RawAccelSettings.AccelerationSettings;

            AccelCharts.ShowActive(settings);
            ApplyOptions.SetActiveValues(settings);
        }

        private Timer SetupButtonTimer()
        {
            Timer buttonTimer = new Timer();
            buttonTimer.Enabled = true;
            buttonTimer.Interval = Convert.ToInt32(ManagedAccel.WriteDelay);
            buttonTimer.Tick += new System.EventHandler(OnButtonTimerTick);
            return buttonTimer;
        }

        private void SetupWriteButton()
        {
            WriteButton.Top = AccelCharts.Top + AccelCharts.TopChartHeight - Constants.WriteButtonVerticalOffset;
            SetWriteButtonDefault();
        }

        private void SetWriteButtonDefault()
        {
            WriteButton.Text = Constants.WriteButtonDefaultText;
            WriteButton.Enabled = true;
        }

        private void SetWriteButtonDelay()
        {
            WriteButton.Enabled = false;
            WriteButton.Text = $"{Constants.WriteButtonDelayText} : {ButtonTimer.Interval} ms";
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void OnWriteButtonClick(object sender, EventArgs e)
        {
            UpdateActiveSettingsFromFields();
        }

        private void OnButtonTimerTick(object sender, EventArgs e)
        {
            ButtonTimer.Stop();
            SetWriteButtonDefault();
        }

        private void WriteButtonDelay()
        {
            SetWriteButtonDelay();
            ButtonTimer.Start();
        }

        #endregion Methods
    }

}

using grapher.Models.Calculations;
using grapher.Models.Charts;
using grapher.Models.Charts.ChartState;
using grapher.Models.Serialized;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace grapher
{
    public class AccelCharts
    {
        #region Constructors

        public AccelCharts(
            Form form,
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            ToolStripMenuItem enableVelocityAndGain,
            ToolStripMenuItem enableLastMouseMove,
            Button writeButton,
            AccelCalculator accelCalculator)
        {
            var estimated = new EstimatedPoints();
            var estimatedX = new EstimatedPoints();
            var estimatedY = new EstimatedPoints();
            SetupCharts(sensitivityChart, velocityChart, gainChart, estimated, estimatedX, estimatedY);
            var accelData = new AccelData(estimated, estimatedX, estimatedY);
            ChartStateManager = new ChartStateManager(sensitivityChart, velocityChart, gainChart, accelData, accelCalculator);

            ContainingForm = form;
            EnableVelocityAndGain = enableVelocityAndGain;
            EnableLastValue = enableLastMouseMove;
            WriteButton = writeButton;


            Rectangle screenRectangle = ContainingForm.RectangleToScreen(ContainingForm.ClientRectangle);
            FormBorderHeight = screenRectangle.Top - ContainingForm.Top;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableVelocityGainCheckStateChange);

            EnableLastValue.CheckedChanged += new System.EventHandler(OnEnableLastMouseMoveCheckStateChange);

            ChartState = ChartStateManager.InitialState();
            ChartState.Activate();
            HideVelocityAndGain();
        }

        #endregion Constructors

        #region Properties

        public Form ContainingForm { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        private ToolStripMenuItem EnableLastValue { get; }

        private Button WriteButton { get; }

        public AccelData AccelData
        {
            get
            {
                return ChartState.Data;
            }
        }

        public int Left
        {
            get
            {
                return ChartState.SensitivityChart.Left;
            }
        }

        public int Top
        {
            get
            {
                return ChartState.SensitivityChart.Top;
            }
        }

        public int TopChartHeight
        {
            get
            {
                return ChartState.SensitivityChart.Height;
            }
        }

        private int FormBorderHeight { get; }

        private ChartState ChartState { get; set; }

        private ChartStateManager ChartStateManager { get; }


        #endregion Properties

        #region Methods

        public void MakeDots(int x, int y, double timeInMs)
        {
            ChartState.MakeDots(x, y, timeInMs);
        }

        public void DrawLastMovement()
        {
            if (EnableLastValue.Checked)
            {
                ChartState.DrawLastMovement();
            }
        }

        public void Bind()
        {
            ChartState.Bind();
        }

        public void ShowActive(DriverSettings driverSettings)
        {
            ChartState = ChartStateManager.DetermineState(driverSettings);
            ChartState.Activate();
            UpdateFormWidth();
            Bind();
        }

        public void SetWidened()
        {
            ChartState.SetWidened();
            UpdateFormWidth();
            AlignWriteButton();
        }

        public void SetNarrowed()
        {
            ChartState.SetNarrowed();
            UpdateFormWidth();
            AlignWriteButton();
        }

        public void Calculate(ManagedAccel accel, DriverSettings settings)
        {
            ChartState.SetUpCalculate(settings);
            ChartState.Calculate(accel, settings);
        }

        private static void SetupCharts(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            EstimatedPoints estimated,
            EstimatedPoints estimatedX,
            EstimatedPoints estimatedY)
        {
            sensitivityChart.SetPointBinds(estimated.Sensitivity, estimatedX.Sensitivity, estimatedY.Sensitivity);
            velocityChart.SetPointBinds(estimated.Velocity, estimatedX.Velocity, estimatedY.Velocity);
            gainChart.SetPointBinds(estimated.Gain, estimatedX.Gain, estimatedY.Gain);

            sensitivityChart.SetTop(0);
            velocityChart.SetHeight(sensitivityChart.Height);
            velocityChart.SetTop(sensitivityChart.Height + Constants.ChartSeparationVertical);
            gainChart.SetHeight(sensitivityChart.Height);
            gainChart.SetTop(velocityChart.Top + velocityChart.Height + Constants.ChartSeparationVertical);

            sensitivityChart.Show();
        }

        private void OnEnableClick(object sender, EventArgs e)
        {
            EnableVelocityAndGain.Checked = !EnableVelocityAndGain.Checked;
        }

        private void OnEnableVelocityGainCheckStateChange(object sender, EventArgs e)
        {
            if (EnableVelocityAndGain.Checked)
            {
                ShowVelocityAndGain();
            }
            else
            {
                HideVelocityAndGain();
            }
        }

        private void OnEnableLastMouseMoveCheckStateChange(object sender, EventArgs e)
        {
            if (!EnableLastValue.Checked)
            {
                ChartState.ClearLastValue();
            }
        }

        private void ShowVelocityAndGain()
        {
            ChartState.ShowVelocityAndGain(ContainingForm, FormBorderHeight);
        }

        private void HideVelocityAndGain()
        {
            ChartState.HideVelocityAndGain(ContainingForm, FormBorderHeight);
        }

        private void UpdateFormWidth()
        {
            ContainingForm.Width = ChartState.SensitivityChart.Left + ChartState.SensitivityChart.Width;
        }

        private void AlignWriteButton()
        {
            WriteButton.Left = ChartState.SensitivityChart.Left / 2 - WriteButton.Width / 2;
        }

        #endregion Methods
    }
}

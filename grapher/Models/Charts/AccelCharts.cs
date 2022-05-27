using grapher.Models.Calculations;
using grapher.Models.Calculations.Data;
using grapher.Models.Charts;
using grapher.Models.Charts.ChartState;
using System;
using System.Windows.Forms;

namespace grapher
{
    public class AccelCharts
    {
        #region Constructors

        public AccelCharts(
            RawAcceleration form,
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            TableLayoutPanel chartContainer,
            ToolStripMenuItem enableVelocityAndGain,
            ToolStripMenuItem enableLastMouseMove,
            Button writeButton,
            AccelCalculator accelCalculator)
        {
            var estimated = new EstimatedPoints();
            var estimatedX = new EstimatedPoints();
            var estimatedY = new EstimatedPoints();
            SetupCharts(sensitivityChart, velocityChart, gainChart, estimated, estimatedX, estimatedY);
            ChartStateManager = new ChartStateManager(
                sensitivityChart,
                velocityChart,
                gainChart,
                chartContainer,
                accelCalculator,
                estimated,
                estimatedX,
                estimatedY);

            ContainingForm = form;
            EnableVelocityAndGain = enableVelocityAndGain;
            EnableLastValue = enableLastMouseMove;

            WriteButton = writeButton;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableVelocityGainCheckStateChange);

            EnableLastValue.CheckedChanged += new System.EventHandler(OnEnableLastMouseMoveCheckStateChange);

            ChartState = ChartStateManager.InitialState();
            ChartState.Activate();
            HideVelocityAndGain();
        }

        #endregion Constructors

        #region Properties

        public RawAcceleration ContainingForm { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        private ToolStripMenuItem EnableLastValue { get; }

        private Button WriteButton { get; }

        public IAccelData AccelData
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

        private int FormBorderHeight { get; }

        private ChartState ChartState { get; set; }

        private ChartStateManager ChartStateManager { get; }


        #endregion Properties

        #region Methods

        public void MakeDots(double x, double y, double timeInMs)
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

        public void ShowActive(Profile args)
        {
            ChartState = ChartStateManager.DetermineState(args);
            ChartState.Activate();
            Bind();
        }

        public void Redraw()
        {
            ChartState.Redraw();
        }

        public void Calculate(ManagedAccel accel, Profile settings)
        {
            ChartState.SetUpCalculate();
            ChartState.Calculate(accel, settings);
        }

        public void SetLogarithmic(bool x, bool y)
        {
            ChartState.SetLogarithmic(x, y);
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
            gainChart.SetLegendPosition(ChartXY.GainLegendPosition);
        }

        private void OnEnableClick(object sender, EventArgs e)
        {
            EnableVelocityAndGain.Checked = !EnableVelocityAndGain.Checked;
        }

        private void OnEnableVelocityGainCheckStateChange(object sender, EventArgs e)
        {
            ContainingForm.ResetAutoScroll();
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
            ChartState.ShowVelocityAndGain();
        }

        private void HideVelocityAndGain()
        {
            ChartState.HideVelocityAndGain();
        }

        private void AlignWriteButton()
        {
            WriteButton.Left = ChartState.SensitivityChart.Left / 2 - WriteButton.Width / 2;
        }

        #endregion Methods
    }
}

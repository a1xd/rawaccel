using grapher.Models.Calculations;
using grapher.Models.Calculations.Data;
using System;
using System.Windows.Forms;

namespace grapher.Models.Charts.ChartState
{
    public abstract class ChartState
    {
        public ChartState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            TableLayoutPanel chartContainer,
            AccelCalculator calculator)
        {
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            ChartContainer = chartContainer;
            Calculator = calculator;
            TwoDotsPerGraph = false;
        }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public TableLayoutPanel ChartContainer { get; }

        public IAccelData Data { get; protected set; }

        public AccelCalculator Calculator { get; }

        public virtual Profile Settings { get; set; }

        internal bool TwoDotsPerGraph { get; set; }

        public virtual void MakeDots(double x, double y, double timeInMs)
        {
            Data.CalculateDots(x, y, timeInMs);
        }

        public abstract void Bind();

        public abstract void Activate();

        public virtual void Calculate(ManagedAccel accel, Profile settings)
        {
            Data.CreateGraphData(accel, settings);
        }

        public void Redraw()
        {
            SensitivityChart.Update();
            VelocityChart.Update();
            GainChart.Update();
        }

        public virtual void SetUpCalculate()
        {
            Data.Clear();
            Calculator.ScaleByMouseSettings();
        }

        public void DrawLastMovement()
        {
            SensitivityChart.DrawLastMovementValue(TwoDotsPerGraph);
            VelocityChart.DrawLastMovementValue(TwoDotsPerGraph);
            GainChart.DrawLastMovementValue(TwoDotsPerGraph);
        }

        public void ClearLastValue()
        {
            SensitivityChart.ClearLastValue();
            VelocityChart.ClearLastValue();
            GainChart.ClearLastValue();
        }

        public void ShowVelocityAndGain()
        {
            ChartContainer.RowCount = Constants.VelocityAndGainRowCount;
            ChartContainer.RowStyles[0].Height = Constants.VelocityAndGainRowHeight;
            VelocityChart.Show();
            GainChart.Show();
        }

        public void HideVelocityAndGain()
        {
            ChartContainer.RowCount = Constants.RegularRowCount;
            ChartContainer.RowStyles[0].Height = Constants.RegularRowHeight;
            SensitivityChart.SetHeight(Constants.SensitivityChartAloneHeight);
            VelocityChart.Hide();
            GainChart.Hide();
        }

        public void SetLogarithmic(bool x, bool y)
        {
            if (x)
            {
                ChartXY.SetLogarithmic(SensitivityChart.ChartX);
                ChartXY.SetLogarithmic(GainChart.ChartX);
            }
            else
            {
                ChartXY.SetStandard(SensitivityChart.ChartX);
                ChartXY.SetStandard(GainChart.ChartX);
            }

            if (y)
            {
                ChartXY.SetLogarithmic(SensitivityChart.ChartY);
                ChartXY.SetLogarithmic(GainChart.ChartY);
            }
            {
                ChartXY.SetStandard(SensitivityChart.ChartY);
                ChartXY.SetStandard(GainChart.ChartY);
            }
        }
    }
}

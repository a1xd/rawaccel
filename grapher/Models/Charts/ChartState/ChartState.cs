using grapher.Models.Calculations;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Charts.ChartState
{
    public abstract class ChartState
    {
        public ChartState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            AccelData accelData,
            AccelCalculator calculator)
        {
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            Data = accelData;
            Calculator = calculator;
            TwoDotsPerGraph = false;
        }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public AccelData Data { get; }

        public AccelCalculator Calculator { get; }

        public virtual DriverSettings Settings { get; set; }

        internal bool TwoDotsPerGraph { get; set; }

        public abstract void MakeDots(int x, int y, double timeInMs);

        public abstract void Bind();

        public abstract void Activate();

        public abstract void Calculate(ManagedAccel accel, DriverSettings settings);

        public virtual void SetUpCalculate(DriverSettings settings)
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

        public void SetWidened()
        {
            SensitivityChart.SetWidened();
            VelocityChart.SetWidened();
            GainChart.SetWidened();
        }

        public void SetNarrowed()
        {
            SensitivityChart.SetNarrowed();
            VelocityChart.SetNarrowed();
            GainChart.SetNarrowed();
        }

        public void ClearLastValue()
        {
            SensitivityChart.ClearLastValue();
            VelocityChart.ClearLastValue();
            GainChart.ClearLastValue();
        }

        public void ShowVelocityAndGain(Form form, int borderHeight)
        {
            VelocityChart.Show();
            GainChart.Show();
            form.Height = SensitivityChart.Height +
                                    Constants.ChartSeparationVertical +
                                    VelocityChart.Height +
                                    Constants.ChartSeparationVertical +
                                    GainChart.Height +
                                    borderHeight;
        }

        public void HideVelocityAndGain(Form form, int borderHeight)
        {
            VelocityChart.Hide();
            GainChart.Hide();
            form.Height = SensitivityChart.Height + borderHeight;
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

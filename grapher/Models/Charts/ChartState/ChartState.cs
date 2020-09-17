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
            AccelData accelData)
        {
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            AccelData = accelData;
        }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public AccelData AccelData { get; }

        public virtual DriverSettings Settings { get; set; }

        public abstract void MakeDots(int x, int y, double timeInMs);

        public abstract void Bind();

        public abstract void Activate();

        public void DrawLastMovement()
        {
            SensitivityChart.DrawLastMovementValue();
            VelocityChart.DrawLastMovementValue();
            GainChart.DrawLastMovementValue();
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
    }
}

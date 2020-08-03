using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelCharts
    {
        public const int ChartSeparation = 10;

        /// <summary> Needed to show full contents in form. Unsure why. </summary>
        public const int FormHeightPadding = 35;

        public AccelCharts(
            Form form,
            Chart sensitivityChart,
            Chart velocityChart,
            Chart gainChart,
            ToolStripMenuItem enableVelocityAndGain)
        {
            ContaingForm = form;
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            EnableVelocityAndGain = enableVelocityAndGain;

            SensitivityChart.Top = 0;
            VelocityChart.Height = SensitivityChart.Height;
            VelocityChart.Top = SensitivityChart.Height + ChartSeparation;
            GainChart.Height = SensitivityChart.Height;
            GainChart.Top = VelocityChart.Top + VelocityChart.Height + ChartSeparation;

            Rectangle screenRectangle = ContaingForm.RectangleToScreen(ContaingForm.ClientRectangle);
            FormBorderHeight = screenRectangle.Top - ContaingForm.Top;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableCheckStateChange);

            HideVelocityAndGain();
        }

        public Form ContaingForm { get; }

        public Chart SensitivityChart { get; }

        public Chart VelocityChart { get; }

        public Chart GainChart { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        private int FormBorderHeight { get; }

        private void OnEnableClick(object sender, EventArgs e)
        {
            EnableVelocityAndGain.Checked = !EnableVelocityAndGain.Checked;
        }

        private void OnEnableCheckStateChange(object sender, EventArgs e)
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

        private void ShowVelocityAndGain()
        {
            VelocityChart.Show();
            GainChart.Show();
            ContaingForm.Height = SensitivityChart.Height + 
                                    ChartSeparation +
                                    VelocityChart.Height +
                                    ChartSeparation +
                                    GainChart.Height +
                                    FormBorderHeight;
        }

        private void HideVelocityAndGain()
        {
            VelocityChart.Hide();
            GainChart.Hide();
            ContaingForm.Height = SensitivityChart.Height + FormBorderHeight;
        }
    }
}

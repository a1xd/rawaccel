using grapher.Models.Calculations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelCharts
    {
        public struct ChartPoint
        {
            public double X;
            public double Y;
        }

        public struct EstimatedPoints
        {
            public ChartPoint CombinedVelocity;
            public ChartPoint CombinedSensitivity;
            public ChartPoint CombinedGain;

            public ChartPoint XVelocity;
            public ChartPoint XSensitivity;
            public ChartPoint XGain;

            public ChartPoint YVelocity;
            public ChartPoint YSensitivity;
            public ChartPoint YGain;
        }

        public const int ChartSeparationVertical = 10;

        /// <summary> Needed to show full contents in form. Unsure why. </summary>
        public const int FormHeightPadding = 35;

        public AccelCharts(
            Form form,
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            ToolStripMenuItem enableVelocityAndGain,
            ICollection<CheckBox> checkBoxesXY)
        {
            ContaingForm = form;
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            EnableVelocityAndGain = enableVelocityAndGain;
            CheckBoxesXY = checkBoxesXY;
            AccelData = new AccelData();

            Estimated = new EstimatedPoints
            {
                CombinedVelocity = new ChartPoint { X = 0, Y = 0 },
                CombinedSensitivity = new ChartPoint { X = 0, Y = 0 },
                CombinedGain = new ChartPoint { X = 0, Y = 0 },

                XVelocity = new ChartPoint { X = 0, Y = 0 },
                XSensitivity = new ChartPoint { X = 0, Y = 0 },
                XGain = new ChartPoint { X = 0, Y = 0 },

                YVelocity = new ChartPoint { X = 0, Y = 0 },
                YSensitivity = new ChartPoint { X = 0, Y = 0 },
                YGain = new ChartPoint { X = 0, Y = 0 },
            };

            SensitivityChart.SetTop(0);
            VelocityChart.SetHeight(SensitivityChart.Height);
            VelocityChart.SetTop(SensitivityChart.Height + ChartSeparationVertical);
            GainChart.SetHeight(SensitivityChart.Height);
            GainChart.SetTop(VelocityChart.Top + VelocityChart.Height + ChartSeparationVertical);

            Rectangle screenRectangle = ContaingForm.RectangleToScreen(ContaingForm.ClientRectangle);
            FormBorderHeight = screenRectangle.Top - ContaingForm.Top;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableCheckStateChange);

            HideVelocityAndGain();
            Combined = false;
            ShowCombined();
        }

        public Form ContaingForm { get; }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        public AccelData AccelData { get; }

        private EstimatedPoints Estimated;

        private ICollection<CheckBox> CheckBoxesXY { get; }

        private bool Combined { get; set; }

        private int FormBorderHeight { get; }

        public void MakeDots(int x, int y)
        {
            AccelData.CalculateDots(x, y, ref Estimated);
            SensitivityChart.DrawPoints(Estimated.CombinedSensitivity, Estimated.XSensitivity, Estimated.YSensitivity);
            VelocityChart.DrawPoints(Estimated.CombinedVelocity, Estimated.XVelocity, Estimated.YVelocity);
            GainChart.DrawPoints(Estimated.CombinedGain, Estimated.XGain, Estimated.YGain);
        }

        public void Bind()
        {
            if (Combined)
            {
                SensitivityChart.Bind(AccelData.Combined.AccelPoints);
                VelocityChart.Bind(AccelData.Combined.VelocityPoints);
                GainChart.Bind(AccelData.Combined.GainPoints);
            }
            else
            {
                SensitivityChart.BindXY(AccelData.X.AccelPoints, AccelData.Y.AccelPoints);
                VelocityChart.BindXY(AccelData.X.VelocityPoints, AccelData.Y.VelocityPoints);
                GainChart.BindXY(AccelData.X.GainPoints, AccelData.Y.GainPoints);
            }
        }

        public void RefreshXY()
        {
            if (CheckBoxesXY.All(box => box.Checked))
            {
                ShowCombined();
            }
            else
            {
                ShowXandYSeparate();
            }
        }

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
                                    ChartSeparationVertical +
                                    VelocityChart.Height +
                                    ChartSeparationVertical +
                                    GainChart.Height +
                                    FormBorderHeight;
        }

        private void HideVelocityAndGain()
        {
            VelocityChart.Hide();
            GainChart.Hide();
            ContaingForm.Height = SensitivityChart.Height + FormBorderHeight;
        }

        private void ShowXandYSeparate()
        {
            if (Combined)
            {
                Combined = false;

                SensitivityChart.SetSeparate();
                VelocityChart.SetSeparate();
                GainChart.SetSeparate();
                UpdateFormWidth();
                Bind();
            }
        }

        private void ShowCombined()
        {
            if (!Combined)
            {
                Combined = true;

                SensitivityChart.SetCombined();
                VelocityChart.SetCombined();
                GainChart.SetCombined();
                UpdateFormWidth();
                Bind();
            }
        }

        private void UpdateFormWidth()
        {
            ContaingForm.Width = SensitivityChart.Left + SensitivityChart.Width;
        }
    }
}

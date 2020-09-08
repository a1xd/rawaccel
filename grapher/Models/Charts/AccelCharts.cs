using grapher.Models.Calculations;
using grapher.Models.Charts;
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
        #region Constructors

        public AccelCharts(
            Form form,
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            ToolStripMenuItem enableVelocityAndGain)
        {
            Estimated = new EstimatedPoints();
            EstimatedX = new EstimatedPoints();
            EstimatedY = new EstimatedPoints();
            AccelData = new AccelData(Estimated, EstimatedX, EstimatedY);

            ContaingForm = form;
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            EnableVelocityAndGain = enableVelocityAndGain;

            SensitivityChart.SetPointBinds(Estimated.Sensitivity, EstimatedX.Sensitivity, EstimatedY.Sensitivity);
            VelocityChart.SetPointBinds(Estimated.Velocity, EstimatedX.Velocity, EstimatedY.Velocity);
            GainChart.SetPointBinds(Estimated.Gain, EstimatedX.Gain, EstimatedY.Gain);

            SensitivityChart.SetTop(0);
            VelocityChart.SetHeight(SensitivityChart.Height);
            VelocityChart.SetTop(SensitivityChart.Height + Constants.ChartSeparationVertical);
            GainChart.SetHeight(SensitivityChart.Height);
            GainChart.SetTop(VelocityChart.Top + VelocityChart.Height + Constants.ChartSeparationVertical);

            Rectangle screenRectangle = ContaingForm.RectangleToScreen(ContaingForm.ClientRectangle);
            FormBorderHeight = screenRectangle.Top - ContaingForm.Top;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableCheckStateChange);

            HideVelocityAndGain();
            Combined = false;
            ShowCombined();
        }

        #endregion Constructors

        #region Properties

        public Form ContaingForm { get; }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        public AccelData AccelData { get; }

        private EstimatedPoints Estimated { get; }

        private EstimatedPoints EstimatedX { get; }

        private EstimatedPoints EstimatedY { get; }

        private bool Combined { get; set; }

        private int FormBorderHeight { get; }

        #endregion Properties

        #region Methods

        public void MakeDots(int x, int y, double timeInMs)
        {
            if (Combined)
            {
                AccelData.CalculateDots(x, y, timeInMs);
            }
            else
            {
                AccelData.CalculateDotsXY(x, y, timeInMs);
            }
        }

        public void DrawPoints()
        {
            SensitivityChart.DrawPoints();
            VelocityChart.DrawPoints();
            GainChart.DrawPoints();
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

        public void RefreshXY(bool isWhole)
        {
            if (isWhole)
            {
                ShowCombined();
            }
            else
            {
                ShowXandYSeparate();
            }
        }

        public void SetWidened()
        {
            SensitivityChart.SetWidened();
            VelocityChart.SetWidened();
            GainChart.SetWidened();
            UpdateFormWidth();
        }

        public void SetNarrowed()
        {
            SensitivityChart.SetNarrowed();
            VelocityChart.SetNarrowed();
            GainChart.SetNarrowed();
            UpdateFormWidth();
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
                                    Constants.ChartSeparationVertical +
                                    VelocityChart.Height +
                                    Constants.ChartSeparationVertical +
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

        #endregion Methods
    }
}

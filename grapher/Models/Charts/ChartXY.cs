using grapher.Models.Charts;
using grapher.Models.Mouse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static grapher.AccelCharts;

namespace grapher
{
    public class ChartXY
    {
        #region Constants

        public const int ChartSeparationHorizontal = 10;

        #endregion Constants

        #region Constructors

        public ChartXY(Chart chartX, Chart chartY)
        {
            ChartX = chartX;
            ChartY = chartY;

            ChartY.Top = ChartX.Top;
            ChartY.Height = ChartX.Height;
            ChartY.Width = ChartX.Width;
            ChartY.Left = ChartX.Left + ChartX.Width + ChartSeparationHorizontal;

            SetupChart(ChartX);
            SetupChart(ChartY);
        }

        #endregion Constructors

        #region Properties

        public Chart ChartX { get; }

        public Chart ChartY { get; }

        public int Height { 
            get
            {
                return ChartX.Height;
            }
        }

        public int Width { 
            get
            {
                if (Combined)
                {
                    return ChartX.Width;
                }
                else
                {
                    return ChartY.Left + ChartY.Width - ChartX.Left;
                }
            }
        }

        public int Top { 
            get
            {
                return ChartX.Top;
            }
        }

        public int Left { 
            get
            {
                return ChartX.Left;
            }
        }

        public bool Combined { get; private set; }

        private PointData CombinedPointData { get; set; }

        private PointData XPointData { get; set; }

        private PointData YPointData { get; set; }

        #endregion Properties

        #region Methods

        public static void SetupChart(Chart chart)
        {
            chart.ChartAreas[0].AxisX.RoundAxisValues();

            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            chart.ChartAreas[0].AxisY.ScaleView.MinSize = 0.01;
            chart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = 0.001;

            chart.ChartAreas[0].CursorY.Interval = 0.001;

            chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].CursorY.AutoScroll = true;

            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserEnabled = true;

            chart.Series[1].Points.Clear();
            chart.Series[1].Points.AddXY(0, 0);
        }

        public static void DrawPoint(Chart chart, PointData point)
        {
            if (chart.Visible)
            {
                point.Get(out var x, out var y);
                chart.Series[1].Points.DataBindXY(x, y);
                chart.Update();
            }
        }

        public void SetPointBinds(PointData combined, PointData x, PointData y)
        {
            CombinedPointData = combined;
            XPointData = x;
            YPointData = y;
        }

        public void DrawPoints()
        {
            if(Combined)
            {
                DrawPoint(ChartX, CombinedPointData);
            }
            else
            {
                DrawPoint(ChartX, XPointData);
                DrawPoint(ChartY, YPointData);
            }
        }

        public void Bind(IDictionary data)
        {
            ChartX.Series[0].Points.DataBindXY(data.Keys, data.Values);
        }

        public void BindXY(IDictionary dataX, IDictionary dataY)
        {
            ChartX.Series[0].Points.DataBindXY(dataX.Keys, dataX.Values);
            ChartY.Series[0].Points.DataBindXY(dataY.Keys, dataY.Values);
        }

        public void SetCombined()
        {
            if (!Combined)
            {
                ChartY.Hide();
                Combined = true;
            }
        }

        public void SetSeparate()
        {
            if (Combined)
            {
                if (ChartX.Visible)
                {
                    ChartY.Show();
                }

                Combined = false;
            }
        }

        public void Hide()
        {
            ChartX.Hide();
            ChartY.Hide();
        }

        public void Show()
        {
            ChartX.Show();

            if (!Combined)
            {
                ChartY.Show();
            }
        }

        public void SetTop(int top)
        {
            ChartX.Top = top;
            ChartY.Top = top;
        }

        public void SetHeight(int height)
        {
            ChartX.Height = height;
            ChartY.Height = height;
        }

        #endregion Methods
    }
}

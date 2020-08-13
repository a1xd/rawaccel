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
        #region Consts

        public const int ChartSeparationHorizontal = 10;

        #endregion Consts

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

        public static void DrawPoint(Chart chart, ChartPoint point)
        {
            chart.Series[1].Points[0].XValue = point.X; 
            chart.Series[1].Points[0].YValues[0] = point.Y; 
        }

        public void DrawPoints(ChartPoint CombinedPoint, ChartPoint XPoint, ChartPoint YPoint)
        {
            if (Combined)
            {
                DrawPoint(ChartX, CombinedPoint);
            }
            else
            {
                DrawPoint(ChartX, XPoint);
            }

            if (ChartY.Visible)
            {
                DrawPoint(ChartY, YPoint);
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

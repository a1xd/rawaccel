using grapher.Models.Mouse;
using System;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class ChartXY
    {
        #region Fields

        public static ElementPosition DefaultLegendPosition = new ElementPosition(25, 0, 50, 25);
        public static ElementPosition GainLegendPosition = new ElementPosition(25, -1, 50, 25);

        #endregion Fields

        #region Constructors

        public ChartXY(Chart chartX, Chart chartY, string title)
        {
            ChartX = chartX;
            ChartY = chartY;
            Title = title;

            ChartY.Top = ChartX.Top;
            ChartY.Height = ChartX.Height;
            ChartY.Width = ChartX.Width;
            ChartY.Left = ChartX.Left + ChartX.Width + Constants.ChartSeparationHorizontal;

            SetupChart(ChartX);
            SetupChart(ChartY);

            Combined = false;
            SetCombined();
            Visible = true;
        }

        #endregion Constructors

        private const double VerticalMargin = 0.1;

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

        public bool Combined { get; set; }

        public bool Visible { get; set; }

        public string Title { get; }

        private PointData CombinedPointData { get; set; }

        private PointData XPointData { get; set; }

        private PointData YPointData { get; set; }

        #endregion Properties

        #region Methods
        
        public static void SetupChart(Chart chart)
        {
            ChartArea area = chart.ChartAreas[0];
            Legend legend = chart.Legends[0];
            Title title = chart.Titles[0];

            area.AxisX.RoundAxisValues();

            area.AxisX.ScaleView.Zoomable = true;
            area.AxisY.ScaleView.Zoomable = true;

            area.AxisY.ScaleView.MinSize = 0.01;
            area.AxisY.ScaleView.SmallScrollSize = 0.001;
            
            area.AxisX.LabelStyle.Format = "0.##";
            area.AxisY.LabelStyle.Format = "0.##";

            area.CursorY.Interval = 0.001;

            area.CursorX.AutoScroll = true;
            area.CursorY.AutoScroll = true;

            area.CursorX.IsUserSelectionEnabled = true;
            area.CursorY.IsUserSelectionEnabled = true;

            area.CursorX.IsUserEnabled = true;
            area.CursorY.IsUserEnabled = true;

            chart.Series[1].Points.Clear();
            chart.Series[1].Points.AddXY(0, 0);
            
            area.AxisX.TitleFont = new System.Drawing.Font(area.AxisX.TitleFont.Name, Constants.ChartAxisFontSize, System.Drawing.FontStyle.Bold);
            area.AxisY.TitleFont = area.AxisX.TitleFont;

            title.Font = new System.Drawing.Font(title.Font.Name, Constants.ChartTitleFontSize, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
            
            chart.Series[0].BorderWidth = Constants.ChartSeriesLineWidth;
            chart.Series[0].MarkerSize = Constants.ChartSeriesLineWidth * 2;
            chart.Series[2].BorderWidth = Constants.ChartSeriesLineWidth;
            chart.Series[2].MarkerSize = Constants.ChartSeriesLineWidth * 2;

            for (int i = 1; i < chart.Series.Count; i += 2)
            {
                chart.Series[i].MarkerSize = Constants.DotMarkerSize;
            }

            area.AxisX.MinorGrid.Enabled = true;
            area.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dot;

            title.Alignment = System.Drawing.ContentAlignment.MiddleCenter;

            legend.DockedToChartArea = area.Name;
            legend.LegendStyle = LegendStyle.Row;
            legend.IsTextAutoFit = true;
            legend.MaximumAutoSize = 100;
            legend.AutoFitMinFontSize = 5;
            
            ElementPosition legendPosNew = DefaultLegendPosition;
            legend.Position = legendPosNew;

            System.Drawing.Color bgTrans = System.Drawing.Color.Transparent;

            area.BackColor = bgTrans;
            legend.BackColor = bgTrans;
        }

        public static void DrawPoint(Chart chart, PointData pointOne, PointData pointTwo = null)
        {
            if (chart.Visible)
            {
                pointOne.Get(out var x, out var y);
                chart.Series[1].Points.DataBindXY(x, y);
                if (pointTwo != null)
                {
                    pointTwo.Get(out x, out y);
                    chart.Series[3].Points.DataBindXY(x, y);
                }
            }
        }

        public static void SetLogarithmic(Chart chart)
        {
            /*
            chart.ChartAreas[0].AxisX.Minimum = 0.001;
            chart.ChartAreas[0].AxisX.Maximum = 3500;
            chart.ChartAreas[0].AxisY.Minimum = 0.001;
            chart.ChartAreas[0].AxisY.Maximum = 10;
            chart.ChartAreas[0].AxisX.IsLogarithmic = true;
            chart.ChartAreas[0].AxisY.IsLogarithmic = true;
            */
        }

        public static void SetStandard(Chart chart)
        {
            /*
            chart.ChartAreas[0].AxisX.IsLogarithmic = false;
            chart.ChartAreas[0].AxisY.IsLogarithmic = false;
            */
        }

        public void ClearSecondDots()
        {
            ChartX.Series[3].Points.Clear();
        }

        public void Update()
        {
            ChartX.Update();
            if (ChartY.Visible)
            {
                ChartY.Update();
            }
        }

        public void SetPointBinds(PointData combined, PointData x, PointData y)
        {
            CombinedPointData = combined;
            XPointData = x;
            YPointData = y;
        }

        public void DrawLastMovementValue(bool twoDotsPerGraph = false)
        {
            if(Combined)
            {
                if (twoDotsPerGraph)
                {
                    DrawPoint(ChartX, XPointData, YPointData);
                }
                else
                {
                    DrawPoint(ChartX, CombinedPointData);
                }
            }
            else
            {
                DrawPoint(ChartX, XPointData);
                DrawPoint(ChartY, YPointData);
            }
        }

        public void ClearLastValue()
        {
            ChartX.Series[1].Points.Clear();
            ChartY.Series[1].Points.Clear();
        }

        public void Bind(IDictionary data)
        {
            ChartX.Series[0].Points.DataBindXY(data.Keys, data.Values);
            ChartX.Series[2].IsVisibleInLegend = false;
            ChartX.Series[2].Points.Clear();
        }

        public void BindXY(IDictionary dataX, IDictionary dataY)
        {
            ChartX.Series[0].Points.DataBindXY(dataX.Keys, dataX.Values);
            ChartY.Series[0].Points.DataBindXY(dataY.Keys, dataY.Values);
            ChartX.Series[2].IsVisibleInLegend = false;
            ChartX.Series[2].Points.Clear();
        }

        public void BindXYCombined(IDictionary dataX, IDictionary dataY)
        {
            ChartX.Series[0].Points.DataBindXY(dataX.Keys, dataX.Values);
            ChartX.Series[2].Points.DataBindXY(dataY.Keys, dataY.Values);
            ChartX.Series[2].IsVisibleInLegend = true;
        }

        public void SetLegendPosition(ElementPosition position)
        {
            ChartX.Legends[0].Position = position;
            ChartY.Legends[0].Position = position;
        }

        private void VerifyRange(double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException($"invalid chart range: ({min}, {max})");
            }
        }

        public void SetMinMax(double min, double max)
        {
            VerifyRange(min, max);
            ChartX.ChartAreas[0].AxisY.Minimum = min * (1 - VerticalMargin);
            ChartX.ChartAreas[0].AxisY.Maximum = max * (1 + VerticalMargin);
        }

        public void SetMinMaxXY(double minX, double maxX, double minY, double maxY)
        {
            VerifyRange(minX, maxX);
            ChartX.ChartAreas[0].AxisY.Minimum = minX * (1 - VerticalMargin);
            ChartX.ChartAreas[0].AxisY.Maximum = maxX * (1 + VerticalMargin);

            VerifyRange(minY, maxY);
            ChartY.ChartAreas[0].AxisY.Minimum = minY * (1 - VerticalMargin);
            ChartY.ChartAreas[0].AxisY.Maximum = maxY * (1 + VerticalMargin);
        }

        public void SetCombined()
        {
            if (!Combined)
            {
                ChartY.Hide();
                Combined = true;
                ChartX.Titles[0].Text = Title;
            }
        }

        public void SetSeparate()
        {
            if (Combined)
            {
                if (Visible)
                {
                    ChartY.Show();
                }

                ChartX.Titles[0].Text = SetComponentTitle(Constants.XComponent);
                ChartY.Titles[0].Text = SetComponentTitle(Constants.YComponent);

                Combined = false;
            }
        }


        public void Hide()
        {

            if (Visible)
            {
                ChartX.Hide();
                ChartY.Hide();
                Visible = false;
            }
        }

        public void Show()
        {
            if (!Visible)
            {
                ChartX.Show();

                if (!Combined)
                {
                    ChartY.Show();
                }

                Visible = true;
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

        private string SetComponentTitle(string component)
        {
            return $"{Title} : {component}";
        }
        #endregion Methods
    }
}

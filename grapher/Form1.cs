using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public partial class RawAcceleration : Form
    {
        #region Constructor

        public RawAcceleration()
        {
            InitializeComponent();

            ManagedAcceleration = new ManagedAccel(5, 0, 0.3, 1.25, 15);
            AccelerationType = 5;
            Sensitivity = new VectorXY(1);
            Rotation = 0;
            Weight = new VectorXY(1);
            Cap = new VectorXY(0);
            Offset = 0;
            Acceleration = new Field("0.0", this.accelerationBox, 0);
            LimitOrExponent = 1.01;
            Midpoint = 0;

            UpdateGraph();
 
            this.AccelerationChart.ChartAreas[0].AxisX.RoundAxisValues();

            this.AccelerationChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.AccelerationChart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            this.AccelerationChart.ChartAreas[0].AxisY.ScaleView.MinSize = 0.01;
            this.AccelerationChart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = 0.001;

            this.AccelerationChart.ChartAreas[0].CursorY.Interval = 0.001;

            this.AccelerationChart.ChartAreas[0].CursorX.AutoScroll = true;
            this.AccelerationChart.ChartAreas[0].CursorY.AutoScroll = true;

            this.AccelerationChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.AccelerationChart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            this.AccelerationChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.AccelerationChart.ChartAreas[0].CursorY.IsUserEnabled = true;
        }

        #endregion Constructor

        #region Properties

        public ManagedAccel ManagedAcceleration { get; set; }

        private int AccelerationType { get; set; }

        private VectorXY Sensitivity { get; set; }

        private double Rotation { get; set; }

        private VectorXY Weight { get; set; }

        private VectorXY Cap { get; set; }

        private double Offset { get; set; }

        private Field Acceleration { get; set; }

        private double LimitOrExponent { get; set; }

        private double Midpoint { get; set; }

        #endregion Properties

        #region Methods

        public static double Magnitude(int x, int y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static double Magnitude(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void UpdateGraph()
        {
           var orderedPoints = new SortedDictionary<double, double>();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    var output = ManagedAcceleration.Accelerate(i, j, 1);

                    var inMagnitude = Magnitude(i,j);
                    var outMagnitude = Magnitude(output.Item1, output.Item2);
                    var ratio = inMagnitude > 0 ? outMagnitude / inMagnitude : Sensitivity.X;

                    if (!orderedPoints.ContainsKey(inMagnitude))
                    {
                        orderedPoints.Add(inMagnitude, ratio);
                    }
                }
            }

            var series = this.AccelerationChart.Series.FirstOrDefault();
            series.Points.Clear();

            foreach (var point in orderedPoints)
            {
                series.Points.AddXY(point.Key, point.Value);
            }
        }

        private bool TryHandleWithEnter(KeyEventArgs e, object sender, out double data)
        {
            bool validEntry = false;
            data = 0.0;

            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    data = Convert.ToDouble(((TextBox)sender).Text);
                    validEntry = true;
                }
                catch
                {
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            return validEntry;
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            ManagedAcceleration.UpdateAccel(
                AccelerationType, 
                Sensitivity.X,
                Sensitivity.Y,
                Weight.X,
                Weight.Y,
                Cap.X,
                Cap.Y,
                Offset,
                Acceleration.Data,
                LimitOrExponent,
                Midpoint);
            ManagedAcceleration.WriteToDriver();
            UpdateGraph();
        }

        private void sensitivityBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Sensitivity.SetBoth(data);
            }
        }

        private void rotationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Rotation = data;
            }
        }

        private void capBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Cap.SetBoth(data);
            }
        }

        private void offsetBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Offset = data;
            }
        }

        private void limitBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                LimitOrExponent = data;
            }
        }

        private void midpointBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Midpoint = data;
            }
        }

        #endregion Methods
    }
}

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
            AccelerationOptions = new AccelOptions(
                accelTypeDrop,
                new Option[]
                {
                    new Option(accelerationBox, constantOneLabel),
                    new Option(limitBox, constantTwoLabel),
                    new Option(midpointBox, constantThreeLabel)
                });
            Sensitivity = new FieldXY(sensitivityBoxX, sensitivityBoxY, sensXYLock, this, 1);
            Rotation = new Field(rotationBox, this, 0);
            Weight = new FieldXY(weightBoxFirst, weightBoxSecond, weightXYLock, this, 1);
            Cap = new FieldXY(capBoxX, capBoxY, capXYLock, this, 0);
            Offset = new Field(offsetBox, this, 0);
            Acceleration = new Field(accelerationBox, this, 0);
            LimitOrExponent = new Field(limitBox, this, 2);
            Midpoint = new Field(midpointBox, this, 0);

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

        private AccelOptions AccelerationOptions { get; set; }

        private FieldXY Sensitivity { get; set; }

        private Field Rotation { get; set; }

        private FieldXY Weight { get; set; }

        private FieldXY Cap { get; set; }

        private Field Offset { get; set; }

        private Field Acceleration { get; set; }

        private Field LimitOrExponent { get; set; }

        private Field Midpoint { get; set; }

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
                AccelerationOptions.AccelerationIndex, 
                Rotation.Data,
                Sensitivity.X,
                Sensitivity.Y,
                Weight.X,
                Weight.Y,
                Cap.X,
                Cap.Y,
                Offset.Data,
                Acceleration.Data,
                LimitOrExponent.Data,
                Midpoint.Data);
            ManagedAcceleration.WriteToDriver();
            UpdateGraph();
        }

        #endregion Methods

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

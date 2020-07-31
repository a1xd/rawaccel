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
            Sensitivity = new OptionXY(new FieldXY(sensitivityBoxX, sensitivityBoxY, sensXYLock, this, 1), sensitivityLabel);
            Rotation = new Option(new Field(rotationBox, this, 0), rotationLabel);
            Weight = new OptionXY(new FieldXY(weightBoxFirst, weightBoxSecond, weightXYLock, this, 1), weightLabel);
            Cap = new OptionXY(new FieldXY(capBoxX, capBoxY, capXYLock, this, 0), capLabel);
            Offset = new Option(new Field(offsetBox, this, 0), offsetLabel);

            Sensitivity.SetName("Sensitivity");
            Rotation.SetName("Rotation");
            Weight.SetName("Weight");
            Cap.SetName("Cap");
            Offset.SetName("Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            Acceleration = new Option(new Field(accelerationBox, this, 0), constantOneLabel);
            LimitOrExponent = new Option(new Field(limitBox, this, 2), constantTwoLabel);
            Midpoint = new Option(new Field(midpointBox, this, 0), constantThreeLabel);

            AccelerationOptions = new AccelOptions(
                accelTypeDrop,
                new Option[]
                {
                    Acceleration,
                    LimitOrExponent,
                    Midpoint,
                });

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

        private OptionXY Sensitivity { get; set; }

        private Option Rotation { get; set; }

        private OptionXY Weight { get; set; }

        private OptionXY Cap { get; set; }

        private Option Offset { get; set; }

        private Option Acceleration { get; set; }

        private Option LimitOrExponent { get; set; }

        private Option Midpoint { get; set; }

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
                    var ratio = inMagnitude > 0 ? outMagnitude / inMagnitude : Sensitivity.Fields.X;

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

        private void writeButton_Click(object sender, EventArgs e)
        {
            ManagedAcceleration.UpdateAccel(
                AccelerationOptions.AccelerationIndex, 
                Rotation.Field.Data,
                Sensitivity.Fields.X,
                Sensitivity.Fields.Y,
                Weight.Fields.X,
                Weight.Fields.Y,
                Cap.Fields.X,
                Cap.Fields.Y,
                Offset.Field.Data,
                Acceleration.Field.Data,
                LimitOrExponent.Field.Data,
                Midpoint.Field.Data);
            ManagedAcceleration.WriteToDriver();
            UpdateGraph();
        }

        #endregion Methods

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace grapher
{
    public enum accel_mode 
    {
        linear=1, classic, natural, logarithmic, sigmoid, power, noaccel
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct vec2d 
    {
        public double x;
        public double y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct accel_args
    {
        public double offset;
        public double accel;
        public double limit;
        public double exponent;
        public double midpoint;
        public double power_scale;
        public vec2d weight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct accel_fn_args
    {
        public accel_args acc_args;
        public int accel_mode;
        public double time_min;
        public vec2d cap;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct modifier_args
    {
        public double degrees;
        public vec2d sens;
        public accel_fn_args acc_fn_args;
    }

    public partial class RawAcceleration : Form
    {
        public struct MagnitudeData
        {
            public double magnitude;
            public int x;
            public int y;
        }

        #region Constructor

        public static ReadOnlyCollection<MagnitudeData> Magnitudes = GetMagnitudes();

        public RawAcceleration()
        {
            InitializeComponent();
            modifier_args args;

            args.degrees = 0;
            args.sens.x = 1;
            args.sens.y = 1;
            args.acc_fn_args.acc_args.offset = 0;
            args.acc_fn_args.acc_args.accel = 0.01;
            args.acc_fn_args.acc_args.limit = 2;
            args.acc_fn_args.acc_args.exponent = 1;
            args.acc_fn_args.acc_args.midpoint = 0;
            args.acc_fn_args.acc_args.power_scale = 1;
            args.acc_fn_args.acc_args.weight.x = 1;
            args.acc_fn_args.acc_args.weight.y = 1;
            args.acc_fn_args.accel_mode = (int)accel_mode.natural;
            args.acc_fn_args.time_min = 0.4;
            args.acc_fn_args.cap.x = 0;
            args.acc_fn_args.cap.y = 0;

            IntPtr args_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(args));
            Marshal.StructureToPtr(args, args_ptr, false);

            ManagedAcceleration = new ManagedAccel(args_ptr);

            Marshal.FreeHGlobal(args_ptr);

            Sensitivity = new OptionXY(sensitivityBoxX, sensitivityBoxY, sensXYLock, this, 1, sensitivityLabel, "Sensitivity");
            Rotation = new Option(rotationBox, this, 0, rotationLabel, "Rotation");
            Weight = new OptionXY(weightBoxFirst, weightBoxSecond, weightXYLock, this, 1, weightLabel, "Weight");
            Cap = new OptionXY(capBoxX, capBoxY, capXYLock, this, 0, capLabel, "Cap");
            Offset = new Option(offsetBox, this, 0, offsetLabel, "Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            Acceleration = new Option(new Field(accelerationBox, this, 0), constantOneLabel);
            LimitOrExponent = new Option(new Field(limitBox, this, 2), constantTwoLabel);
            Midpoint = new Option(new Field(midpointBox, this, 0), constantThreeLabel);

            AccelerationOptions = new AccelOptions(
                accelTypeDrop,
                new Option[]
                {
                    Offset,
                    Acceleration,
                    LimitOrExponent,
                    Midpoint,
                },
                new OptionXY[]
                {
                    Weight,
                    Cap,
                },
                writeButton);

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

        public static ReadOnlyCollection<MagnitudeData> GetMagnitudes()
        {
            var magnitudes = new List<MagnitudeData>();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    MagnitudeData magnitudeData;
                    magnitudeData.magnitude = Magnitude(i, j);
                    magnitudeData.x = i;
                    magnitudeData.y = j;
                    magnitudes.Add(magnitudeData);
                }
            }

            magnitudes.Sort((m1, m2) => m1.magnitude.CompareTo(m2.magnitude));

            return magnitudes.AsReadOnly();
        }

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

            foreach (var magnitudeData in Magnitudes)
            {
                var output = ManagedAcceleration.Accelerate(magnitudeData.x, magnitudeData.y, 1);

                var outMagnitude = Magnitude(output.Item1, output.Item2);
                var ratio = magnitudeData.magnitude > 0 ? outMagnitude / magnitudeData.magnitude : Sensitivity.Fields.X;

                if (!orderedPoints.ContainsKey(magnitudeData.magnitude))
                {
                    orderedPoints.Add(magnitudeData.magnitude, ratio);
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
    }
}

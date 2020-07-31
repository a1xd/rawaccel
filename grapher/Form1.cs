using System;
using System.Collections.Generic;
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

            var managedAccel = new ManagedAccel(args_ptr);

            Marshal.FreeHGlobal(args_ptr);

            var orderedPoints = new SortedDictionary<double, double>();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    var output = managedAccel.Accelerate(i, j, 1);

                    var inMagnitude = Magnitude(i,j);
                    var outMagnitude = Magnitude(output.Item1, output.Item2);
                    var ratio = inMagnitude > 0 ? outMagnitude / inMagnitude : 0;

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

            this.AccelerationChart.ChartAreas[0].AxisX.RoundAxisValues();
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
    }
}

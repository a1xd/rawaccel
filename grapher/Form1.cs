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

        #region Constructor


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

            var managedAcceleration = new ManagedAccel(args_ptr);

            Marshal.FreeHGlobal(args_ptr);

            var sensitivity = new OptionXY(sensitivityBoxX, sensitivityBoxY, sensXYLock, this, 1, sensitivityLabel, "Sensitivity");
            var rotation = new Option(rotationBox, this, 0, rotationLabel, "Rotation");
            var weight = new OptionXY(weightBoxFirst, weightBoxSecond, weightXYLock, this, 1, weightLabel, "Weight");
            var cap = new OptionXY(capBoxX, capBoxY, capXYLock, this, 0, capLabel, "Cap");
            var offset = new Option(offsetBox, this, 0, offsetLabel, "Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            var acceleration = new Option(new Field(accelerationBox, this, 0), constantOneLabel);
            var limitOrExponent = new Option(new Field(limitBox, this, 2), constantTwoLabel);
            var midpoint = new Option(new Field(midpointBox, this, 0), constantThreeLabel);

            var accelerationOptions = new AccelOptions(
                accelTypeDrop,
                new Option[]
                {
                    offset,
                    acceleration,
                    limitOrExponent,
                    midpoint,
                },
                new OptionXY[]
                {
                    weight,
                    cap,
                },
                writeButton);

            AccelGUI = new AccelGUI(
                this,
                AccelerationChart,
                VelocityChart,
                GainChart,
                managedAcceleration,
                accelerationOptions,
                sensitivity,
                rotation,
                weight,
                cap,
                offset,
                acceleration,
                limitOrExponent,
                midpoint);

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


            this.VelocityChart.ChartAreas[0].AxisX.RoundAxisValues();

            this.VelocityChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.VelocityChart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            this.VelocityChart.ChartAreas[0].AxisY.ScaleView.MinSize = 0.01;
            this.VelocityChart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = 0.001;

            this.VelocityChart.ChartAreas[0].CursorY.Interval = 0.001;

            this.VelocityChart.ChartAreas[0].CursorX.AutoScroll = true;
            this.VelocityChart.ChartAreas[0].CursorY.AutoScroll = true;

            this.VelocityChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.VelocityChart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            this.VelocityChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.VelocityChart.ChartAreas[0].CursorY.IsUserEnabled = true;


            this.GainChart.ChartAreas[0].AxisX.RoundAxisValues();

            this.GainChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.GainChart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            this.GainChart.ChartAreas[0].AxisY.ScaleView.MinSize = 0.01;
            this.GainChart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = 0.001;

            this.GainChart.ChartAreas[0].CursorY.Interval = 0.001;

            this.GainChart.ChartAreas[0].CursorX.AutoScroll = true;
            this.GainChart.ChartAreas[0].CursorY.AutoScroll = true;

            this.GainChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.GainChart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            this.GainChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.GainChart.ChartAreas[0].CursorY.IsUserEnabled = true;
        }

        #endregion Constructor

        #region Properties

        public AccelGUI AccelGUI { get; }

        #endregion Properties

        #region Methods

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            AccelGUI.ManagedAcceleration.UpdateAccel(
                AccelGUI.AccelerationOptions.AccelerationIndex, 
                AccelGUI.Rotation.Field.Data,
                AccelGUI.Sensitivity.Fields.X,
                AccelGUI.Sensitivity.Fields.Y,
                AccelGUI.Weight.Fields.X,
                AccelGUI.Weight.Fields.Y,
                AccelGUI.Cap.Fields.X,
                AccelGUI.Cap.Fields.Y,
                AccelGUI.Offset.Field.Data,
                AccelGUI.Acceleration.Field.Data,
                AccelGUI.LimitOrExponent.Field.Data,
                AccelGUI.Midpoint.Field.Data);
            AccelGUI.ManagedAcceleration.WriteToDriver();
            AccelGUI.UpdateGraph();
        }

        #endregion Methods
    }
}

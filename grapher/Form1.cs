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
using grapher.Models.Calculations;
using grapher.Models.Options;

namespace grapher
{
    public enum accel_mode 
    {
        linear=1, classic, natural, logarithmic, sigmoid, power, naturalgain, sigmoidgain, noaccel
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
        public double gain_cap;
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
            args.acc_fn_args.acc_args.gain_cap = 0;
            args.acc_fn_args.accel_mode = (int)accel_mode.natural;
            args.acc_fn_args.time_min = 0.4;
            args.acc_fn_args.cap.x = 0;
            args.acc_fn_args.cap.y = 0;

            IntPtr args_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(args));
            Marshal.StructureToPtr(args, args_ptr, false);

            var managedAcceleration = new ManagedAccel(args_ptr);

            Marshal.FreeHGlobal(args_ptr);

            var accelCharts = new AccelCharts(
                                this,
                                new ChartXY(AccelerationChart, AccelerationChartY),
                                new ChartXY(VelocityChart, VelocityChartY),
                                new ChartXY(GainChart, GainChartY),
                                showVelocityGainToolStripMenuItem,
                                new CheckBox[] { sensXYLock, weightXYLock, capXYLock });

            ActiveValueTitle.AutoSize = false;
            ActiveValueTitle.Left = LockXYLabel.Left + LockXYLabel.Width;
            ActiveValueTitle.Width = AccelerationChart.Left - ActiveValueTitle.Left;
            ActiveValueTitle.TextAlign = ContentAlignment.MiddleCenter;

            var sensitivity = new OptionXY(
                sensitivityBoxX,
                sensitivityBoxY,
                sensXYLock,
                this,
                1,
                sensitivityLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(SensitivityActiveXLabel, ActiveValueTitle),
                    new ActiveValueLabel(SensitivityActiveYLabel, ActiveValueTitle)),
                "Sensitivity",
                accelCharts);

            var rotation = new Option(
                rotationBox,
                this,
                0,
                rotationLabel,
                new ActiveValueLabel(RotationActiveLabel, ActiveValueTitle),
                "Rotation");

            var weight = new OptionXY(
                weightBoxFirst,
                weightBoxSecond,
                weightXYLock,
                this,
                1,
                weightLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(WeightActiveXLabel, ActiveValueTitle),
                    new ActiveValueLabel(WeightActiveYLabel, ActiveValueTitle)),
                "Weight",
                accelCharts);

            var cap = new OptionXY(
                capBoxX,
                capBoxY,
                capXYLock,
                this,
                0,
                capLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(CapActiveXLabel, ActiveValueTitle),
                    new ActiveValueLabel(CapActiveYLabel, ActiveValueTitle)),
                "Cap",
                accelCharts);

            var offset = new Option(
                offsetBox,
                this,
                0,
                offsetLabel,
                new ActiveValueLabel(OffsetActiveLabel, ActiveValueTitle),
                "Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            var acceleration = new Option(
                new Field(accelerationBox, this, 0),
                constantOneLabel,
                new ActiveValueLabel(AccelerationActiveLabel, ActiveValueTitle));

            var limitOrExponent = new Option(
                new Field(limitBox, this, 2),
                constantTwoLabel,
                new ActiveValueLabel(LimitExpActiveLabel, ActiveValueTitle));

            var midpoint = new Option(
                new Field(midpointBox, this, 0),
                constantThreeLabel,
                new ActiveValueLabel(MidpointActiveLabel, ActiveValueTitle));

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
                writeButton,
                new ActiveValueLabel(AccelTypeActiveLabel, ActiveValueTitle));

            var capOptions = new CapOptions(
                sensitivityToolStripMenuItem,
                velocityGainToolStripMenuItem,
                cap,
                weight);

            AccelCalculator accelCalculator = new AccelCalculator(
                new Field(DPITextBox.TextBox, this, AccelCalculator.DefaultDPI),
                new Field(PollRateTextBox.TextBox, this, AccelCalculator.DefaultPollRate));

            AccelGUI = new AccelGUI(
                this,
                accelCalculator,
                accelCharts,
                managedAcceleration,
                accelerationOptions,
                sensitivity,
                rotation,
                weight,
                capOptions,
                offset,
                acceleration,
                limitOrExponent,
                midpoint,
                writeButton,
                MouseLabel,
                ScaleMenuItem,
                AutoWriteMenuItem);
        }

        #endregion Constructor

        #region Properties

        public AccelGUI AccelGUI { get; }

        #endregion Properties

        #region Methods

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00ff)
            {
                AccelGUI.MouseWatcher.ReadMouseMove(m);
            }

            base.WndProc(ref m);
        }

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
                AccelGUI.Cap.SensitivityCapX,
                AccelGUI.Cap.SensitivityCapY,
                AccelGUI.Offset.Field.Data,
                AccelGUI.Acceleration.Field.Data,
                AccelGUI.LimitOrExponent.Field.Data,
                AccelGUI.Midpoint.Field.Data,
                AccelGUI.Cap.VelocityGainCap);
            AccelGUI.UpdateGraph();
        }

        #endregion Methods

        private void RawAcceleration_Paint(object sender, PaintEventArgs e)
        {
            AccelGUI.AccelCharts.DrawPoints();
        }
    }
}

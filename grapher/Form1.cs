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
using grapher.Models.Serialized;

namespace grapher
{
    public partial class RawAcceleration : Form
    {

        #region Constructor


        public RawAcceleration()
        {
            InitializeComponent();

            ManagedAccel activeAccel = null;

            try
            {
                activeAccel = ManagedAccel.GetActiveAccel();
            }
            catch (DriverNotInstalledException)
            {
                throw;
            }

            var accelCharts = new AccelCharts(
                                this,
                                new ChartXY(AccelerationChart, AccelerationChartY),
                                new ChartXY(VelocityChart, VelocityChartY),
                                new ChartXY(GainChart, GainChartY),
                                showVelocityGainToolStripMenuItem);

            ActiveValueTitle.AutoSize = false;
            ActiveValueTitle.Left = LockXYLabel.Left + LockXYLabel.Width;
            ActiveValueTitle.Width = AccelerationChart.Left - ActiveValueTitle.Left;
            ActiveValueTitle.TextAlign = ContentAlignment.MiddleCenter;

            var applyOptions = new ApplyOptions(wholeVectorToolStripMenuItem, byVectorComponentToolStripMenuItem);

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
                "Sensitivity");

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
                "Weight");

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
                "Cap");

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

            var accelCalculator = new AccelCalculator(
                new Field(DPITextBox.TextBox, this, AccelCalculator.DefaultDPI),
                new Field(PollRateTextBox.TextBox, this, AccelCalculator.DefaultPollRate));

            var settings = new SettingsManager(
                activeAccel,
                accelCalculator.DPI,
                accelCalculator.PollRate,
                AutoWriteMenuItem);

            AccelGUI = new AccelGUI(
                this,
                accelCalculator,
                accelCharts,
                settings,
                applyOptions,
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
            AccelGUI.UpdateActiveSettingsFromFields();
        }

        #endregion Methods

        private void RawAcceleration_Paint(object sender, PaintEventArgs e)
        {
            AccelGUI.AccelCharts.DrawPoints();
        }
    }
}

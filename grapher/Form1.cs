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
using grapher.Models;

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
            catch (DriverNotInstalledException ex)
            {
                MessageBox.Show($"Driver not installed.\n\n {ex.ToString()}");
                throw;
            }

            AccelGUI = AccelGUIFactory.Construct(
                this,
                activeAccel,
                AccelerationChart,
                AccelerationChartY,
                VelocityChart,
                VelocityChartY,
                GainChart,
                GainChartY,
                accelTypeDropX,
                accelTypeDropY,
                writeButton,
                toggleButton,
                showVelocityGainToolStripMenuItem,
                showLastMouseMoveToolStripMenuItem,
                wholeVectorToolStripMenuItem,
                byVectorComponentToolStripMenuItem,
                gainCapToolStripMenuItem,
                legacyCapToolStripMenuItem,
                gainOffsetToolStripMenuItem,
                legacyOffsetToolStripMenuItem,
                ScaleMenuItem,
                DPITextBox,
                PollRateTextBox,
                sensitivityBoxX,
                sensitivityBoxY,
                rotationBox,
                weightBoxX,
                weightBoxY,
                capBoxX,
                capBoxY,
                offsetBoxX,
                offsetBoxY,
                accelerationBoxX,
                accelerationBoxY,
                scaleBoxX,
                scaleBoxY,
                limitBoxX,
                limitBoxY,
                expBoxX,
                expBoxY,
                midpointBoxX,
                midpointBoxY,
                sensXYLock,
                ByComponentXYLock,
                LockXYLabel,
                sensitivityLabel,
                rotationLabel,
                weightLabelX,
                weightLabelY,
                capLabelX,
                capLabelY,
                offsetLabelX,
                offsetLabelY,
                constantOneLabelX,
                constantOneLabelY,
                scaleLabelX,
                scaleLabelY,
                limitLabelX,
                limitLabelY,
                expLabelX,
                expLabelY,
                constantThreeLabelX,
                constantThreeLabelY,
                ActiveValueTitle,
                ActiveValueTitleY,
                SensitivityActiveXLabel,
                SensitivityActiveYLabel,
                RotationActiveLabel,
                WeightActiveXLabel,
                WeightActiveYLabel,
                CapActiveXLabel,
                CapActiveYLabel,
                OffsetActiveXLabel,
                OffsetActiveYLabel,
                AccelerationActiveLabelX,
                AccelerationActiveLabelY,
                ScaleActiveXLabel,
                ScaleActiveYLabel,
                LimitActiveXLabel,
                LimitActiveYLabel,
                ExpActiveXLabel,
                ExpActiveYLabel,
                MidpointActiveXLabel,
                MidpointActiveYLabel,
                AccelTypeActiveLabelX,
                AccelTypeActiveLabelY,
                OptionSetXTitle,
                OptionSetYTitle,
                MouseLabel);
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

        public void ResetAutoScroll()
        {
            panel2.AutoScrollPosition = Constants.Origin;
        }

        public void DoResize()
        {
            ResetAutoScroll();

            var wa = Screen.PrimaryScreen.WorkingArea;
            var pref2 = panel2.GetPreferredSize(Constants.MaxSize);

            Size = new Size
            {
                Width = Math.Min(wa.Width - Location.X, panel1.Size.Width + pref2.Width),
                Height = Math.Min(wa.Height - Location.Y, pref2.Height + 48)
            };
        }

        private void RawAcceleration_Paint(object sender, PaintEventArgs e)
        {
            //AccelGUI.AccelCharts.DrawLastMovement();
        }

        #endregion Method

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

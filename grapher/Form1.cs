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
            catch (DriverNotInstalledException)
            {
                throw;
            }

            ActiveValueTitle.AutoSize = false;
            ActiveValueTitle.Left = accelerationBoxY.Left + accelerationBoxY.Width;
            ActiveValueTitle.Width = AccelerationChart.Left - ActiveValueTitle.Left;
            ActiveValueTitle.TextAlign = ContentAlignment.MiddleCenter;

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
                showVelocityGainToolStripMenuItem,
                wholeVectorToolStripMenuItem,
                byVectorComponentToolStripMenuItem,
                gainCapToolStripMenuItem,
                legacyCapToolStripMenuItem,
                gainOffsetToolStripMenuItem,
                legacyOffsetToolStripMenuItem,
                AutoWriteMenuItem,
                scaleByDPIToolStripMenuItem,
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
                limitBoxX,
                limitBoxY,
                midpointBoxX,
                midpointBoxY,
                sensXYLock,
                ByComponentXYLock,
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
                constantTwoLabelX,
                constantTwoLabelY,
                constantThreeLabelX,
                constantThreeLabelY,
                ActiveValueTitle,
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
                LimitExpActiveXLabel,
                LimitExpActiveYLabel,
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

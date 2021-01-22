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
using System.Reflection;
using System.Diagnostics;

namespace grapher
{
    public partial class RawAcceleration : Form
    {

        #region Constructor


        public RawAcceleration()
        {
            InitializeComponent();

            Version assemVersion = typeof(RawAcceleration).Assembly.GetName().Version;
            Version driverVersion = null;

            try
            {
                driverVersion = VersionHelper.ValidateAndGetDriverVersion(assemVersion);
            }
            catch (VersionException ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            
            ToolStripMenuItem HelpMenuItem = new ToolStripMenuItem("&Help");

            HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    new ToolStripMenuItem("&About", null, (s, e) => new AboutBox(driverVersion).ShowDialog())
            });

            menuStrip1.Items.AddRange(new ToolStripItem[] { HelpMenuItem });

            AccelGUI = AccelGUIFactory.Construct(
                this,
                ManagedAccel.GetActiveAccel(),
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
                gainCapToolStripMenuItem,
                legacyCapToolStripMenuItem,
                gainOffsetToolStripMenuItem,
                legacyOffsetToolStripMenuItem,
                AutoWriteMenuItem,
                UseSpecificDeviceMenuItem,
                ScaleMenuItem,
                DPITextBox,
                PollRateTextBox,
                DirectionalityPanel,
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
                DomainBoxX,
                DomainBoxY,
                RangeBoxX,
                RangeBoxY,
                LpNormBox,
                sensXYLock,
                ByComponentXYLock,
                FakeBox,
                WholeCheckBox,
                ByComponentCheckBox,
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
                MouseLabel,
                DirectionalityLabel,
                DirectionalityX,
                DirectionalityY,
                DirectionalityActiveValueTitle,
                LPNormLabel,
                LpNormActiveValue,
                DirectionalDomainLabel,
                DomainActiveValueX,
                DomainActiveValueY,
                DirectionalityRangeLabel,
                RangeActiveValueX,
                RangeActiveValueY);

            ResizeAndCenter();
        }

        #endregion Constructor

        #region Properties

        public AccelGUI AccelGUI { get; }

        #endregion Properties

        #region Methods

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00ff) // WM_INPUT
            {
                AccelGUI.MouseWatcher.ReadMouseMove(m);
            }
            else if (m.Msg == 0x00fe) // WM_INPUT_DEVICE_CHANGE
            {
                AccelGUI.UpdateInputManagers();
            }

            base.WndProc(ref m);
        }

        public void ResetAutoScroll()
        {
            chartsPanel.AutoScrollPosition = Constants.Origin;
        }

        public void ResizeAndCenter()
        {
            ResetAutoScroll();

            var workingArea = Screen.FromControl(this).WorkingArea;
            var chartsPreferredSize = chartsPanel.GetPreferredSize(Constants.MaxSize);

            Size = new Size
            {
                Width = Math.Min(workingArea.Width, optionsPanel.Size.Width + chartsPreferredSize.Width),
                Height = Math.Min(workingArea.Height, chartsPreferredSize.Height + 48)
            };

            Location = new Point
            {
                X = workingArea.X + (workingArea.Width - Size.Width) / 2,
                Y = workingArea.Y + (workingArea.Height - Size.Height) / 2
            };

        }

        #endregion Method
    }
}

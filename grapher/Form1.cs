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

            Version driverVersion = VersionHelper.ValidOrThrow();

            ToolStripMenuItem HelpMenuItem = new ToolStripMenuItem("&Help");

            HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    new ToolStripMenuItem("&About", null, (s, e) => new AboutBox(driverVersion).ShowDialog())
            });

            menuStrip1.Items.AddRange(new ToolStripItem[] { HelpMenuItem });

            AccelGUI = AccelGUIFactory.Construct(
                this,
                AccelerationChart,
                AccelerationChartY,
                VelocityChart,
                VelocityChartY,
                GainChart,
                GainChartY,
                accelTypeDropX,
                accelTypeDropY,
                XLutApplyDropdown,
                YLutApplyDropdown,
                CapTypeDropdownXClassic,
                CapTypeDropdownYClassic,
                CapTypeDropdownXPower,
                CapTypeDropdownYPower,
                writeButton,
                toggleButton,
                showVelocityGainToolStripMenuItem,
                showLastMouseMoveToolStripMenuItem,
                streamingModeToolStripMenuItem,
                AutoWriteMenuItem,
                DeviceMenuItem,
                ScaleMenuItem,
                DPITextBox,
                PollRateTextBox,
                DirectionalityPanel,
                sensitivityBoxX,
                VertHorzRatioBox,
                rotationBox,
                weightBoxX,
                weightBoxY,
                inCapBoxXClassic,
                inCapBoxYClassic,
                outCapBoxXClassic,
                outCapBoxYClassic,
                inCapBoxXPower,
                inCapBoxYPower,
                outCapBoxXPower,
                outCapBoxYPower,
                offsetBoxX,
                offsetBoxY,
                accelerationBoxX,
                accelerationBoxY,
                decayRateBoxX,
                decayRateBoxY,
                growthRateBoxX,
                growthRateBoxY,
                smoothBoxX,
                smoothBoxY,
                scaleBoxX,
                scaleBoxY,
                limitBoxX,
                limitBoxY,
                powerBoxX,
                powerBoxY,
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
                gainSwitchX,
                gainSwitchY,
                XLutActiveValuesBox,
                YLutActiveValuesBox,
                XLutPointsBox,
                YLutPointsBox,
                LockXYLabel,
                sensitivityLabel,
                VertHorzRatioLabel,
                rotationLabel,
                weightLabelX,
                weightLabelY,
                inCapLabelXClassic,
                inCapLabelYClassic,
                outCapLabelXClassic,
                outCapLabelYClassic,
                CapTypeLabelXClassic,
                CapTypeLabelYClassic,
                inCapLabelXPower,
                inCapLabelYPower,
                outCapLabelXPower,
                outCapLabelYPower,
                CapTypeLabelXPower,
                CapTypeLabelYPower,
                offsetLabelX,
                offsetLabelY,
                constantOneLabelX,
                constantOneLabelY,
                decayRateLabelX,
                decayRateLabelY,
                growthRateLabelX,
                growthRateLabelY,
                smoothLabelX,
                smoothLabelY,
                scaleLabelX,
                scaleLabelY,
                limitLabelX,
                limitLabelY,
                powerLabelX,
                powerLabelY,
                expLabelX,
                expLabelY,
                LUTTextLabelX,
                LUTTextLabelY,
                constantThreeLabelX,
                constantThreeLabelY,
                ActiveValueTitle,
                ActiveValueTitleY,
                SensitivityMultiplierActiveLabel,
                VertHorzRatioActiveLabel,
                RotationActiveLabel,
                WeightActiveXLabel,
                WeightActiveYLabel,
                InCapActiveXLabelClassic,
                InCapActiveYLabelClassic,
                OutCapActiveXLabelClassic,
                OutCapActiveYLabelClassic,
                CapTypeActiveXLabelClassic,
                CapTypeActiveYLabelClassic,
                InCapActiveXLabelPower,
                InCapActiveYLabelPower,
                OutCapActiveXLabelPower,
                OutCapActiveYLabelPower,
                CapTypeActiveXLabelPower,
                CapTypeActiveYLabelPower,
                OffsetActiveXLabel,
                OffsetActiveYLabel,
                AccelerationActiveLabelX,
                AccelerationActiveLabelY,
                DecayRateActiveXLabel,
                DecayRateActiveYLabel,
                GrowthRateActiveXLabel,
                GrowthRateActiveYLabel,
                SmoothActiveXLabel,
                SmoothActiveYLabel,
                ScaleActiveXLabel,
                ScaleActiveYLabel,
                LimitActiveXLabel,
                LimitActiveYLabel,
                PowerClassicActiveXLabel,
                PowerClassicActiveYLabel,
                ExpActiveXLabel,
                ExpActiveYLabel,
                MidpointActiveXLabel,
                MidpointActiveYLabel,
                AccelTypeActiveLabelX,
                AccelTypeActiveLabelY,
                gainSwitchActiveLabelX,
                gainSwitchActiveLabelY,
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
                RangeActiveValueY,
                XLutApplyLabel,
                YLutApplyLabel,
                LutApplyActiveXLabel,
                LutApplyActiveYLabel);

            ResizeAndCenter();
        }

        #endregion Constructor

        #region Properties

        public AccelGUI AccelGUI { get; }

        #endregion Properties

        #region Methods

        protected override void WndProc(ref Message m)
        {
            if (!(AccelGUI is null))
            {
                if (m.Msg == 0x00ff) // WM_INPUT
                {
                    AccelGUI.MouseWatcher.ReadMouseMove(m);
                }
                else if (m.Msg == 0x00fe) // WM_INPUT_DEVICE_CHANGE
                {
                    AccelGUI.Settings.OnDeviceChangeMessage();
                }
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

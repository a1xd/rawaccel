using grapher.Models.Calculations;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher.Models
{
    public static class AccelGUIFactory
    {
        #region Methods

        public static AccelGUI Construct(
            RawAcceleration form,
            ManagedAccel activeAccel,
            Chart accelerationChart,
            Chart accelerationChartY,
            Chart velocityChart,
            Chart velocityChartY,
            Chart gainChart,
            Chart gainChartY,
            ComboBox accelTypeDropX,
            ComboBox accelTypeDropY,
            Button writeButton,
            ToolStripMenuItem showVelocityGainToolStripMenuItem,
            ToolStripMenuItem wholeVectorToolStripMenuItem,
            ToolStripMenuItem byVectorComponentToolStripMenuItem,
            ToolStripMenuItem velocityGainCapToolStripMenuItem,
            ToolStripMenuItem legacyCapToolStripMenuItem,
            ToolStripMenuItem gainOffsetToolStripMenuItem,
            ToolStripMenuItem legacyOffsetToolStripMenuItem,
            ToolStripMenuItem autoWriteMenuItem,
            ToolStripMenuItem scaleMenuItem,
            ToolStripTextBox dpiTextBox,
            ToolStripTextBox pollRateTextBox,
            TextBox sensitivityBoxX,
            TextBox sensitivityBoxY,
            TextBox rotationBox,
            TextBox weightBoxX,
            TextBox weightBoxY,
            TextBox capBoxX,
            TextBox capBoxY,
            TextBox offsetBoxX,
            TextBox offsetBoxY,
            TextBox accelerationBoxX,
            TextBox accelerationBoxY,
            TextBox limitBoxX,
            TextBox limitBoxY,
            TextBox midpointBoxX,
            TextBox midpointBoxY,
            CheckBox sensXYLock,
            CheckBox byComponentXYLock,
            Label lockXYLabel,
            Label sensitivityLabel,
            Label rotationLabel,
            Label weightLabelX,
            Label weightLabelY,
            Label capLabelX,
            Label capLabelY,
            Label offsetLabelX,
            Label offsetLabelY,
            Label constantOneLabelX,
            Label constantOneLabelY,
            Label constantTwoLabelX,
            Label constantTwoLabelY,
            Label constantThreeLabelX,
            Label constantThreeLabelY,
            Label activeValueTitleX,
            Label activeValueTitleY,
            Label sensitivityActiveXLabel,
            Label sensitivityActiveYLabel,
            Label rotationActiveLabel,
            Label weightActiveXLabel,
            Label weightActiveYLabel,
            Label capActiveXLabel,
            Label capActiveYLabel,
            Label offsetActiveLabelX,
            Label offsetActiveLabelY,
            Label accelerationActiveLabelX,
            Label accelerationActiveLabelY,
            Label limitExpActiveLabelX,
            Label limitExpActiveLabelY,
            Label midpointActiveLabelX,
            Label midpointActiveLabelY,
            Label accelTypeActiveLabelX,
            Label accelTypeActiveLabelY,
            Label optionSetXTitle,
            Label optionSetYTitle,
            Label mouseLabel)
        {
            var accelCharts = new AccelCharts(
                                form,
                                new ChartXY(accelerationChart, accelerationChartY),
                                new ChartXY(velocityChart, velocityChartY),
                                new ChartXY(gainChart, gainChartY),
                                showVelocityGainToolStripMenuItem,
                                writeButton);

            var sensitivity = new OptionXY(
                sensitivityBoxX,
                sensitivityBoxY,
                sensXYLock,
                form,
                1,
                sensitivityLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(sensitivityActiveXLabel, activeValueTitleX),
                    new ActiveValueLabel(sensitivityActiveYLabel, activeValueTitleX)),
                "Sensitivity");

            var rotation = new Option(
                rotationBox,
                form,
                0,
                rotationLabel,
                0,
                new ActiveValueLabel(rotationActiveLabel, activeValueTitleX),
                "Rotation");

            var optionSetYLeft = rotation.Left + rotation.Width;

            var weightX = new Option(
                weightBoxX,
                form,
                1,
                weightLabelX,
                0,
                new ActiveValueLabel(weightActiveXLabel, activeValueTitleX),
                "Weight");

            var weightY = new Option(
                weightBoxY,
                form,
                1,
                weightLabelY,
                optionSetYLeft,
                new ActiveValueLabel(weightActiveYLabel, activeValueTitleY),
                "Weight");

            var capX = new Option(
                capBoxX,
                form,
                0,
                capLabelX,
                0,
                new ActiveValueLabel(capActiveXLabel, activeValueTitleX),
                "Cap");

            var capY = new Option(
                capBoxY,
                form,
                0,
                capLabelY,
                optionSetYLeft,
                new ActiveValueLabel(capActiveYLabel, activeValueTitleY),
                "Cap");

            var offsetX = new Option(
                offsetBoxX,
                form,
                0,
                offsetLabelX,
                0,
                new ActiveValueLabel(offsetActiveLabelX, activeValueTitleX),
                "Offset");

            var offsetY = new Option(
                offsetBoxY,
                form,
                0,
                offsetLabelY,
                optionSetYLeft,
                new ActiveValueLabel(offsetActiveLabelY, activeValueTitleY),
                "Offset");

            var offsetOptionsX = new OffsetOptions(
                gainOffsetToolStripMenuItem,
                legacyOffsetToolStripMenuItem,
                offsetX);

            var offsetOptionsY = new OffsetOptions(
                gainOffsetToolStripMenuItem,
                legacyOffsetToolStripMenuItem,
                offsetY);

            var accelerationX = new Option(
                new Field(accelerationBoxX, form, 0),
                constantOneLabelX,
                new ActiveValueLabel(accelerationActiveLabelX, activeValueTitleX),
                0);

            var accelerationY = new Option(
                new Field(accelerationBoxY, form, 0),
                constantOneLabelY,
                new ActiveValueLabel(accelerationActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var limitOrExponentX = new Option(
                new Field(limitBoxX, form, 2),
                constantTwoLabelX,
                new ActiveValueLabel(limitExpActiveLabelX, activeValueTitleX),
                0);

            var limitOrExponentY = new Option(
                new Field(limitBoxY, form, 2),
                constantTwoLabelY,
                new ActiveValueLabel(limitExpActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var midpointX = new Option(
                new Field(midpointBoxX, form, 0),
                constantThreeLabelX,
                new ActiveValueLabel(midpointActiveLabelX, activeValueTitleY),
                0);

            var midpointY = new Option(
                new Field(midpointBoxY, form, 0),
                constantThreeLabelY,
                new ActiveValueLabel(midpointActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var capOptionsX = new CapOptions(
                velocityGainCapToolStripMenuItem,
                legacyCapToolStripMenuItem,
                capX);

            var capOptionsY = new CapOptions(
                velocityGainCapToolStripMenuItem,
                legacyCapToolStripMenuItem,
                capY);

            var accelerationOptionsX = new AccelTypeOptions(
                accelTypeDropX,
                accelerationX,
                capOptionsX,
                weightX,
                offsetOptionsX,
                limitOrExponentX,
                midpointX,
                writeButton,
                new ActiveValueLabel(accelTypeActiveLabelX, activeValueTitleX));

            var accelerationOptionsY = new AccelTypeOptions(
                accelTypeDropY,
                accelerationY,
                capOptionsY,
                weightY,
                offsetOptionsY,
                limitOrExponentY,
                midpointY,
                writeButton,
                new ActiveValueLabel(accelTypeActiveLabelY, activeValueTitleY));

            var optionsSetX = new AccelOptionSet(
                optionSetXTitle,
                activeValueTitleX,
                rotationBox.Top + rotationBox.Height + Constants.OptionVerticalSeperation,
                accelerationOptionsX);

            var optionsSetY = new AccelOptionSet(
                optionSetYTitle,
                activeValueTitleY,
                rotationBox.Top + rotationBox.Height + Constants.OptionVerticalSeperation,
                accelerationOptionsY);

            var applyOptions = new ApplyOptions(
                wholeVectorToolStripMenuItem,
                byVectorComponentToolStripMenuItem,
                byComponentXYLock,
                optionsSetX,
                optionsSetY,
                sensitivity,
                rotation,
                lockXYLabel,
                accelCharts);

            var accelCalculator = new AccelCalculator(
                new Field(dpiTextBox.TextBox, form, Constants.DefaultDPI),
                new Field(pollRateTextBox.TextBox, form, Constants.DefaultPollRate));

            var settings = new SettingsManager(
                activeAccel,
                accelCalculator.DPI,
                accelCalculator.PollRate,
                autoWriteMenuItem);

            return new AccelGUI(
                form,
                accelCalculator,
                accelCharts,
                settings,
                applyOptions,
                writeButton,
                mouseLabel,
                scaleMenuItem);
        }

        #endregion Methods
    }
}

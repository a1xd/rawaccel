using grapher.Models.Calculations;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ToolStripMenuItem sensitivityToolStripMenuItem,
            ToolStripMenuItem velocityGainToolStripMenuItem,
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
            Label activeValueTitle,
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
                                showVelocityGainToolStripMenuItem);

            var sensitivity = new OptionXY(
                sensitivityBoxX,
                sensitivityBoxY,
                sensXYLock,
                form,
                1,
                sensitivityLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(sensitivityActiveXLabel, activeValueTitle),
                    new ActiveValueLabel(sensitivityActiveYLabel, activeValueTitle)),
                "Sensitivity");

            var rotation = new Option(
                rotationBox,
                form,
                0,
                rotationLabel,
                0,
                new ActiveValueLabel(rotationActiveLabel, activeValueTitle),
                "Rotation");

            var optionSetYLeft = rotation.Left + rotation.Width;

            var weightX = new Option(
                weightBoxX,
                form,
                1,
                weightLabelX,
                0,
                new ActiveValueLabel(weightActiveXLabel, activeValueTitle),
                "Weight");

            var weightY = new Option(
                weightBoxY,
                form,
                1,
                weightLabelY,
                optionSetYLeft,
                new ActiveValueLabel(weightActiveYLabel, activeValueTitle),
                "Weight");

            var capX = new Option(
                capBoxX,
                form,
                0,
                capLabelX,
                0,
                new ActiveValueLabel(capActiveXLabel, activeValueTitle),
                "Cap");

            var capY = new Option(
                capBoxY,
                form,
                0,
                capLabelY,
                optionSetYLeft,
                new ActiveValueLabel(capActiveYLabel, activeValueTitle),
                "Cap");

            var offsetX = new Option(
                offsetBoxX,
                form,
                0,
                offsetLabelX,
                0,
                new ActiveValueLabel(offsetActiveLabelX, activeValueTitle),
                "Offset");

            var offsetY = new Option(
                offsetBoxY,
                form,
                0,
                offsetLabelY,
                optionSetYLeft,
                new ActiveValueLabel(offsetActiveLabelY, activeValueTitle),
                "Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            var accelerationX = new Option(
                new Field(accelerationBoxX, form, 0),
                constantOneLabelX,
                new ActiveValueLabel(accelerationActiveLabelX, activeValueTitle),
                0);

            var accelerationY = new Option(
                new Field(accelerationBoxY, form, 0),
                constantOneLabelY,
                new ActiveValueLabel(accelerationActiveLabelY, activeValueTitle),
                optionSetYLeft);

            var limitOrExponentX = new Option(
                new Field(limitBoxX, form, 2),
                constantTwoLabelX,
                new ActiveValueLabel(limitExpActiveLabelX, activeValueTitle),
                0);

            var limitOrExponentY = new Option(
                new Field(limitBoxY, form, 2),
                constantTwoLabelY,
                new ActiveValueLabel(limitExpActiveLabelY, activeValueTitle),
                optionSetYLeft);

            var midpointX = new Option(
                new Field(midpointBoxX, form, 0),
                constantThreeLabelX,
                new ActiveValueLabel(midpointActiveLabelX, activeValueTitle),
                0);

            var midpointY = new Option(
                new Field(midpointBoxY, form, 0),
                constantThreeLabelY,
                new ActiveValueLabel(midpointActiveLabelY, activeValueTitle),
                optionSetYLeft);

            var accelerationOptionsX = new AccelTypeOptions(
                accelTypeDropX,
                new Option[]
                {
                    offsetX,
                    accelerationX,
                    limitOrExponentX,
                    midpointX,
                    capX,
                    weightX
                },
                writeButton,
                new ActiveValueLabel(accelTypeActiveLabelX, activeValueTitle));

            var accelerationOptionsY = new AccelTypeOptions(
                accelTypeDropY,
                new Option[]
                {
                    offsetY,
                    accelerationY,
                    limitOrExponentY,
                    midpointY,
                    capY,
                    weightY
                },
                writeButton,
                new ActiveValueLabel(accelTypeActiveLabelY, activeValueTitle));

            var capOptionsX = new CapOptions(
                sensitivityToolStripMenuItem,
                velocityGainToolStripMenuItem,
                capX);

            var capOptionsY = new CapOptions(
                sensitivityToolStripMenuItem,
                velocityGainToolStripMenuItem,
                capY);

            var optionsSetX = new AccelOptionSet(
                optionSetXTitle,
                rotationBox.Top + rotationBox.Height + Constants.OptionVerticalSeperation,
                accelerationOptionsX,
                accelerationX,
                capOptionsX,
                weightX,
                offsetX,
                limitOrExponentX,
                midpointX); ;

            var optionsSetY = new AccelOptionSet(
                optionSetYTitle,
                rotationBox.Top + rotationBox.Height + Constants.OptionVerticalSeperation,
                accelerationOptionsY,
                accelerationY,
                capOptionsY,
                weightY,
                offsetY,
                limitOrExponentY,
                midpointY);

            var applyOptions = new ApplyOptions(
                wholeVectorToolStripMenuItem,
                byVectorComponentToolStripMenuItem,
                byComponentXYLock,
                optionsSetX,
                optionsSetY);

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
                sensitivity,
                rotation,
                writeButton,
                mouseLabel,
                scaleMenuItem);
        }

        #endregion Methods
    }
}

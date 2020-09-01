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
            ComboBox accelTypeDrop,
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
            TextBox offsetBox,
            TextBox accelerationBox,
            TextBox limitBox,
            TextBox midpointBox,
            CheckBox sensXYLock,
            CheckBox weightXYLock,
            CheckBox capXYLock,
            Label sensitivityLabel,
            Label rotationLabel,
            Label weightLabel,
            Label capLabel,
            Label offsetLabel,
            Label constantOneLabel,
            Label constantTwoLabel,
            Label constantThreeLabel,
            Label activeValueTitle,
            Label sensitivityActiveXLabel,
            Label sensitivityActiveYLabel,
            Label rotationActiveLabel,
            Label weightActiveXLabel,
            Label weightActiveYLabel,
            Label capActiveXLabel,
            Label capActiveYLabel,
            Label offsetActiveLabel,
            Label accelerationActiveLabel,
            Label limitExpActiveLabel,
            Label midpointActiveLabel,
            Label accelTypeActiveLabel,
            Label mouseLabel)
        {
            var accelCharts = new AccelCharts(
                                form,
                                new ChartXY(accelerationChart, accelerationChartY),
                                new ChartXY(velocityChart, velocityChartY),
                                new ChartXY(gainChart, gainChartY),
                                showVelocityGainToolStripMenuItem);

            var applyOptions = new ApplyOptions(wholeVectorToolStripMenuItem, byVectorComponentToolStripMenuItem);

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
                new ActiveValueLabel(rotationActiveLabel, activeValueTitle),
                "Rotation");

            var weight = new OptionXY(
                weightBoxX,
                weightBoxY,
                weightXYLock,
                form,
                1,
                weightLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(weightActiveXLabel, activeValueTitle),
                    new ActiveValueLabel(weightActiveYLabel, activeValueTitle)),
                "Weight");

            var cap = new OptionXY(
                capBoxX,
                capBoxY,
                capXYLock,
                form,
                0,
                capLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(capActiveXLabel, activeValueTitle),
                    new ActiveValueLabel(capActiveYLabel, activeValueTitle)),
                "Cap");

            var offset = new Option(
                offsetBox,
                form,
                0,
                offsetLabel,
                new ActiveValueLabel(offsetActiveLabel, activeValueTitle),
                "Offset");

            // The name and layout of these options is handled by AccelerationOptions object.
            var acceleration = new Option(
                new Field(accelerationBox, form, 0),
                constantOneLabel,
                new ActiveValueLabel(accelerationActiveLabel, activeValueTitle));

            var limitOrExponent = new Option(
                new Field(limitBox, form, 2),
                constantTwoLabel,
                new ActiveValueLabel(limitExpActiveLabel, activeValueTitle));

            var midpoint = new Option(
                new Field(midpointBox, form, 0),
                constantThreeLabel,
                new ActiveValueLabel(midpointActiveLabel, activeValueTitle));

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
                new ActiveValueLabel(accelTypeActiveLabel, activeValueTitle));

            var capOptions = new CapOptions(
                sensitivityToolStripMenuItem,
                velocityGainToolStripMenuItem,
                cap,
                weight);

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
                mouseLabel,
                scaleMenuItem);
        }

        #endregion Methods
    }
}

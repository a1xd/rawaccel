using grapher.Models.Calculations;
using grapher.Models.Devices;
using grapher.Models.Mouse;
using grapher.Models.Options;
using grapher.Models.Options.Directionality;
using grapher.Models.Serialized;
using System;
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
            ButtonBase toggleButton,
            ToolStripMenuItem showVelocityGainToolStripMenuItem,
            ToolStripMenuItem showLastMouseMoveMenuItem,
            ToolStripMenuItem velocityGainCapToolStripMenuItem,
            ToolStripMenuItem legacyCapToolStripMenuItem,
            ToolStripMenuItem gainOffsetToolStripMenuItem,
            ToolStripMenuItem legacyOffsetToolStripMenuItem,
            ToolStripMenuItem autoWriteMenuItem,
            ToolStripMenuItem useSpecificDeviceMenuItem,
            ToolStripMenuItem scaleMenuItem,
            ToolStripTextBox dpiTextBox,
            ToolStripTextBox pollRateTextBox,
            Panel directionalityPanel,
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
            TextBox scaleBoxX,
            TextBox scaleBoxY,
            TextBox limitBoxX,
            TextBox limitBoxY,
            TextBox expBoxX,
            TextBox expBoxY,
            TextBox midpointBoxX,
            TextBox midpointBoxY,
            TextBox domainBoxX,
            TextBox domainBoxY,
            TextBox rangeBoxX,
            TextBox rangeBoxY,
            TextBox lpNormBox,
            CheckBox sensXYLock,
            CheckBox byComponentXYLock,
            CheckBox fakeBox,
            CheckBox wholeCheckBox,
            CheckBox byComponentCheckBox,
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
            Label scaleLabelX,
            Label scaleLabelY,
            Label limitLabelX,
            Label limitLabelY,
            Label expLabelX,
            Label expLabelY,
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
            Label scaleActiveLabelX,
            Label scaleActiveLabelY,
            Label limitActiveLabelX,
            Label limitActiveLabelY,
            Label expActiveLabelX,
            Label expActiveLabelY,
            Label midpointActiveLabelX,
            Label midpointActiveLabelY,
            Label accelTypeActiveLabelX,
            Label accelTypeActiveLabelY,
            Label optionSetXTitle,
            Label optionSetYTitle,
            Label mouseLabel,
            Label directionalityLabel,
            Label directionalityX,
            Label directionalityY,
            Label direcionalityActiveValueTitle,
            Label lpNormLabel,
            Label lpNormActiveLabel,
            Label domainLabel,
            Label domainActiveValueX,
            Label domainActiveValueY,
            Label rangeLabel,
            Label rangeActiveValueX,
            Label rangeActiveValueY)
        {
            fakeBox.Checked = false;
            fakeBox.Hide();

            var accelCalculator = new AccelCalculator(
                new Field(dpiTextBox.TextBox, form, Constants.DefaultDPI, 1),
                new Field(pollRateTextBox.TextBox, form, Constants.DefaultPollRate, 1));

            var accelCharts = new AccelCharts(
                                form,
                                new ChartXY(accelerationChart, accelerationChartY, Constants.SensitivityChartTitle),
                                new ChartXY(velocityChart, velocityChartY, Constants.VelocityChartTitle),
                                new ChartXY(gainChart, gainChartY, Constants.GainChartTitle),
                                showVelocityGainToolStripMenuItem,
                                showLastMouseMoveMenuItem,
                                writeButton,
                                accelCalculator);

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
                "Sens Multiplier");

            var rotation = new Option(
                rotationBox,
                form,
                0,
                rotationLabel,
                0,
                new ActiveValueLabel(rotationActiveLabel, activeValueTitleX),
                "Rotation");

            var optionSetYLeft = rotation.Left + rotation.Width;

            var directionalityLeft = directionalityPanel.Left;

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

            var scaleX = new Option(
                new Field(scaleBoxX, form, 0),
                scaleLabelX,
                new ActiveValueLabel(scaleActiveLabelX, activeValueTitleX),
                0);

            var scaleY = new Option(
                new Field(scaleBoxY, form, 0),
                scaleLabelY,
                new ActiveValueLabel(scaleActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var limitX = new Option(
                new Field(limitBoxX, form, 2),
                limitLabelX,
                new ActiveValueLabel(limitActiveLabelX, activeValueTitleX),
                0);

            var limitY = new Option(
                new Field(limitBoxY, form, 2),
                limitLabelY,
                new ActiveValueLabel(limitActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var exponentX = new Option(
                new Field(expBoxX, form, 2),
                expLabelX,
                new ActiveValueLabel(expActiveLabelX, activeValueTitleX),
                0);

            var exponentY = new Option(
                new Field(expBoxY, form, 2),
                expLabelY,
                new ActiveValueLabel(expActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var midpointX = new Option(
                new Field(midpointBoxX, form, 0),
                constantThreeLabelX,
                new ActiveValueLabel(midpointActiveLabelX, activeValueTitleX),
                0);

            var midpointY = new Option(
                new Field(midpointBoxY, form, 0),
                constantThreeLabelY,
                new ActiveValueLabel(midpointActiveLabelY, activeValueTitleY),
                optionSetYLeft);

            var lpNorm = new Option(
                new Field(lpNormBox, form, 2),
                lpNormLabel,
                new ActiveValueLabel(lpNormActiveLabel, direcionalityActiveValueTitle),
                directionalityLeft);

            var domain = new OptionXY(
                domainBoxX,
                domainBoxY,
                fakeBox,
                form,
                1,
                domainLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(domainActiveValueX, direcionalityActiveValueTitle),
                    new ActiveValueLabel(domainActiveValueY, direcionalityActiveValueTitle)),
                false);

            var range = new OptionXY(
                rangeBoxX,
                rangeBoxY,
                fakeBox,
                form,
                1,
                rangeLabel,
                new ActiveValueLabelXY(
                    new ActiveValueLabel(rangeActiveValueX, direcionalityActiveValueTitle),
                    new ActiveValueLabel(rangeActiveValueY, direcionalityActiveValueTitle)),
                false);

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
                scaleX,
                capOptionsX,
                weightX,
                offsetOptionsX,
                limitX,
                exponentX,
                midpointX,
                writeButton,
                new ActiveValueLabel(accelTypeActiveLabelX, activeValueTitleX));

            var accelerationOptionsY = new AccelTypeOptions(
                accelTypeDropY,
                accelerationY,
                scaleY,
                capOptionsY,
                weightY,
                offsetOptionsY,
                limitY,
                exponentY,
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

            var directionalOptions = new DirectionalityOptions(
                directionalityPanel,
                directionalityLabel,
                directionalityX,
                directionalityY,
                direcionalityActiveValueTitle,
                lpNorm,
                domain,
                range,
                wholeCheckBox,
                byComponentCheckBox,
                260);

            var applyOptions = new ApplyOptions(
                byComponentXYLock,
                optionsSetX,
                optionsSetY,
                directionalOptions,
                sensitivity,
                rotation,
                lockXYLabel,
                accelCharts);

            var deviceIdManager = new DeviceIDManager(useSpecificDeviceMenuItem);

            var settings = new SettingsManager(
                activeAccel,
                accelCalculator.DPI,
                accelCalculator.PollRate,
                autoWriteMenuItem,
                showLastMouseMoveMenuItem,
                showVelocityGainToolStripMenuItem,
                deviceIdManager);

            var mouseWatcher = new MouseWatcher(form, mouseLabel, accelCharts, settings);

            return new AccelGUI(
                form,
                accelCalculator,
                accelCharts,
                settings,
                applyOptions,
                writeButton,
                toggleButton,
                mouseWatcher,
                scaleMenuItem,
                deviceIdManager);
        }

        #endregion Methods
    }
}

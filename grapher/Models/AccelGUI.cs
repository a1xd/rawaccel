using grapher.Models.Calculations;
using grapher.Models.Mouse;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelGUI
    {

        #region constructors

        public AccelGUI(
            RawAcceleration accelForm,
            AccelCalculator accelCalculator,
            AccelCharts accelCharts,
            SettingsManager settings,
            AccelOptions accelOptions,
            OptionXY sensitivity,
            Option rotation,
            OptionXY weight,
            CapOptions cap,
            Option offset,
            Option acceleration,
            Option limtOrExp,
            Option midpoint,
            Button writeButton,
            Label mouseMoveLabel,
            ToolStripMenuItem scaleMenuItem,
            ToolStripMenuItem autoWriteMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            AccelerationOptions = accelOptions;
            Sensitivity = sensitivity;
            Rotation = rotation;
            Weight = weight;
            Cap = cap;
            Offset = offset;
            Acceleration = acceleration;
            LimitOrExponent = limtOrExp;
            Midpoint = midpoint;
            WriteButton = writeButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            Settings.Startup();
            UpdateGraph();

            MouseWatcher = new MouseWatcher(AccelForm, mouseMoveLabel, AccelCharts);

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
        }

        #endregion constructors

        #region properties

        public RawAcceleration AccelForm { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public SettingsManager Settings { get; }

        public AccelOptions AccelerationOptions { get; }

        public OptionXY Sensitivity { get; }

        public Option Rotation { get; }

        public OptionXY Weight { get; }

        public CapOptions Cap { get; }

        public Option Offset { get; }

        public Option Acceleration { get; }

        public Option LimitOrExponent { get; }

        public Option Midpoint { get; }

        public Button WriteButton { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        #endregion properties

        #region methods

        public void UpdateActiveSettingsFromFields()
        {
            Settings.UpdateActiveSettings(
                AccelerationOptions.AccelerationIndex, 
                Rotation.Field.Data,
                Sensitivity.Fields.X,
                Sensitivity.Fields.Y,
                Weight.Fields.X,
                Weight.Fields.Y,
                Cap.SensitivityCapX,
                Cap.SensitivityCapY,
                Offset.Field.Data,
                Acceleration.Field.Data,
                LimitOrExponent.Field.Data,
                Midpoint.Field.Data,
                Cap.VelocityGainCap);
            UpdateGraph();
        }

        public void UpdateGraph()
        {
            AccelCalculator.Calculate(AccelCharts.AccelData, Settings.ActiveAccel);
            AccelCharts.Bind();
            UpdateActiveValueLabels();
        }

        public void UpdateActiveValueLabels()
        {
            Sensitivity.SetActiveValues(Settings.ActiveAccel.SensitivityX, Settings.ActiveAccel.SensitivityY);
            Rotation.SetActiveValue(Settings.ActiveAccel.Rotation);
            AccelerationOptions.SetActiveValue(Settings.ActiveAccel.Type);
            Offset.SetActiveValue(Settings.ActiveAccel.Offset);
            Acceleration.SetActiveValue(Settings.ActiveAccel.Acceleration);
            Cap.SetActiveValues(Settings.ActiveAccel.GainCap, Settings.ActiveAccel.CapX, Settings.ActiveAccel.CapY, Settings.ActiveAccel.GainCapEnabled);
            Weight.SetActiveValues(Settings.ActiveAccel.WeightX, Settings.ActiveAccel.WeightY);
            LimitOrExponent.SetActiveValue(Settings.ActiveAccel.LimitExp);
            Midpoint.SetActiveValue(Settings.ActiveAccel.Midpoint);
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }
        #endregion methods
    }

}

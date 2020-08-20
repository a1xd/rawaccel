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
            ManagedAccel managedAccel,
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
            ManagedAcceleration = managedAccel;
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

            ManagedAcceleration.ReadFromDriver();
            SavedSettings = StartupLoad(AccelCalculator.DPI, AccelCalculator.PollRate, autoWriteMenuItem);
            UpdateGraph();

            MouseWatcher = new MouseWatcher(AccelForm, mouseMoveLabel, AccelCharts);

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
        }

        #endregion constructors

        #region properties

        public RawAcceleration AccelForm { get; }

        public RawAccelSettings SavedSettings { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public ManagedAccel ManagedAcceleration { get; }

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

        public void UpdateGraph()
        {
            AccelCalculator.Calculate(AccelCharts.AccelData, ManagedAcceleration);
            AccelCharts.Bind();
            UpdateActiveValueLabels();
            SavedSettings.Save();
        }

        public void UpdateActiveValueLabels()
        {
            Sensitivity.SetActiveValues(ManagedAcceleration.SensitivityX, ManagedAcceleration.SensitivityY);
            Rotation.SetActiveValue(ManagedAcceleration.Rotation);
            AccelerationOptions.SetActiveValue(ManagedAcceleration.Type);
            Offset.SetActiveValue(ManagedAcceleration.Offset);
            Acceleration.SetActiveValue(ManagedAcceleration.Acceleration);
            Cap.SetActiveValues(ManagedAcceleration.GainCap, ManagedAcceleration.CapX, ManagedAcceleration.CapY, ManagedAcceleration.GainCapEnabled);
            Weight.SetActiveValues(ManagedAcceleration.WeightX, ManagedAcceleration.WeightY);
            LimitOrExponent.SetActiveValue(ManagedAcceleration.LimitExp);
            Midpoint.SetActiveValue(ManagedAcceleration.Midpoint);
        }

        private RawAccelSettings StartupLoad(Field dpiField, Field pollRateField, ToolStripMenuItem autoWriteMenuItem)
        {
            if (RawAccelSettings.Exists())
            {
                var settings = RawAccelSettings.Load();
                settings.GUISettings.BindToGUI(dpiField, pollRateField, autoWriteMenuItem);
                return settings;
            }
            else
            {
                return new RawAccelSettings(
                    ManagedAcceleration,
                    new GUISettings(
                        AccelCalculator.DPI,
                        AccelCalculator.PollRate,
                        autoWriteMenuItem));
            }
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }
        #endregion methods
    }

}

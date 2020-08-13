using grapher.Models.Calculations;
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
            Button writeButton)
        {
            AccelForm = accelForm;
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
            AccelData = new AccelData();

            ManagedAcceleration.ReadFromDriver();
            UpdateGraph();
        }

        #endregion constructors

        #region properties

        public RawAcceleration AccelForm { get; }

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

        public AccelData AccelData { get; }

        #endregion properties

        #region methods


        public void UpdateGraph()
        {
            AccelCalculator.Calculate(AccelData, ManagedAcceleration, Sensitivity.Fields.X);
            AccelCharts.Bind(AccelData);
        }

        #endregion methods
    }

}

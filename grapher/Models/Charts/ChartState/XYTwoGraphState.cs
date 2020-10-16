using grapher.Models.Calculations;
using grapher.Models.Serialized;
using System;

namespace grapher.Models.Charts.ChartState
{
    public class XYTwoGraphState : ChartState
    {
        private DriverSettings _settings;

        public XYTwoGraphState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            AccelData accelData,
            AccelCalculator accelCalculator)
            : base(
                  sensitivityChart,
                  velocityChart,
                  gainChart,
                  accelData,
                  accelCalculator)
        { }

        public override DriverSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                ShouldStripSens = AccelCalculator.ShouldStripSens(ref value);
                if (ShouldStripSens)
                {
                    Sensitivity = AccelCalculator.GetSens(ref value);
                }
            }
        }

        private bool ShouldStripSens { get; set; }

        private (double, double) Sensitivity { get; set; }

        public override void Activate()
        {
            SensitivityChart.SetSeparate();
            VelocityChart.SetSeparate();
            GainChart.SetSeparate();

            SensitivityChart.ClearSecondDots();
            VelocityChart.ClearSecondDots();
            GainChart.ClearSecondDots();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            double xCalc = x; 
            double yCalc = y;

            if (ShouldStripSens)
            {
                (xCalc, yCalc) = AccelCalculator.StripSens(xCalc, yCalc, Sensitivity.Item1, Sensitivity.Item2);
            }

            Data.CalculateDotsXY((int)Math.Round(xCalc), (int)Math.Round(yCalc), timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.BindXY(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXY(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXY(Data.X.GainPoints, Data.Y.GainPoints);

            SensitivityChart.SetMinMaxXY(Data.X.MinAccel, Data.X.MaxAccel, Data.Y.MinAccel, Data.Y.MaxAccel);
            GainChart.SetMinMaxXY(Data.X.MinGain, Data.X.MaxGain, Data.Y.MinGain, Data.Y.MaxGain);
        }

        public override void Calculate(ManagedAccel accel, DriverSettings settings)
        {
            Calculator.Calculate(Data.X, accel, settings.sensitivity.x, Calculator.SimulatedInputX);
            Calculator.Calculate(Data.Y, accel, settings.sensitivity.y, Calculator.SimulatedInputY);
        }
    }
}

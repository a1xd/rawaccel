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
            AccelData accelData)
            : base(
                  sensitivityChart,
                  velocityChart,
                  gainChart,
                  accelData)
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
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            double xCalc = x; 
            double yCalc = y;

            if (ShouldStripSens)
            {
                (xCalc, yCalc) = AccelCalculator.StripSens(xCalc, yCalc, Sensitivity.Item1, Sensitivity.Item2);
            }

            AccelData.CalculateDotsXY((int)Math.Round(xCalc), (int)Math.Round(yCalc), timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.BindXY(AccelData.X.AccelPoints, AccelData.Y.AccelPoints);
            VelocityChart.BindXY(AccelData.X.VelocityPoints, AccelData.Y.VelocityPoints);
            GainChart.BindXY(AccelData.X.GainPoints, AccelData.Y.GainPoints);
        }
    }
}

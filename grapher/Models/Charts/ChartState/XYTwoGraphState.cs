using grapher.Models.Calculations;
using grapher.Models.Calculations.Data;
using grapher.Models.Serialized;
using System;

namespace grapher.Models.Charts.ChartState
{
    public class XYTwoGraphState : ChartState
    {
        public XYTwoGraphState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            EstimatedPoints xPoints,
            EstimatedPoints yPoints,
            AccelCalculator accelCalculator)
            : base(
                  sensitivityChart,
                  velocityChart,
                  gainChart,
                  accelCalculator)
        {
            Data = new AccelDataXYComponential(xPoints, yPoints, accelCalculator);
        }

        public override DriverSettings Settings { get; set; }

        public override void Activate()
        {
            SensitivityChart.SetSeparate();
            VelocityChart.SetSeparate();
            GainChart.SetSeparate();

            SensitivityChart.ClearSecondDots();
            VelocityChart.ClearSecondDots();
            GainChart.ClearSecondDots();
        }

        public override void Bind()
        {
            SensitivityChart.BindXY(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXY(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXY(Data.X.GainPoints, Data.Y.GainPoints);

            SensitivityChart.SetMinMaxXY(Data.X.MinAccel, Data.X.MaxAccel, Data.Y.MinAccel, Data.Y.MaxAccel);
            GainChart.SetMinMaxXY(Data.X.MinGain, Data.X.MaxGain, Data.Y.MinGain, Data.Y.MaxGain);
        }
    }
}

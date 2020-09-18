using grapher.Models.Calculations;

namespace grapher.Models.Charts.ChartState
{
    public class XYOneGraphState : ChartState
    {
        public XYOneGraphState(
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

        public override void Activate()
        {
            SensitivityChart.SetSeparate();
            VelocityChart.SetSeparate();
            GainChart.SetSeparate();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            Data.CalculateDotsXY(x, y, timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.BindXY(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXY(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXY(Data.X.GainPoints, Data.Y.GainPoints);
        }
    }
}

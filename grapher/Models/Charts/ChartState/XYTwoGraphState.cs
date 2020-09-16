using grapher.Models.Calculations;

namespace grapher.Models.Charts.ChartState
{
    public class XYTwoGraphState : ChartState
    {
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

        public override void Activate()
        {
            SensitivityChart.SetSeparate();
            VelocityChart.SetSeparate();
            GainChart.SetSeparate();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            AccelData.CalculateDotsXY(x, y, timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.BindXY(AccelData.X.AccelPoints, AccelData.Y.AccelPoints);
            VelocityChart.BindXY(AccelData.X.VelocityPoints, AccelData.Y.VelocityPoints);
            GainChart.BindXY(AccelData.X.GainPoints, AccelData.Y.GainPoints);
        }
    }
}

using grapher.Models.Calculations;

namespace grapher.Models.Charts.ChartState
{
    public class CombinedState : ChartState
    {
        public CombinedState(
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
            SensitivityChart.SetCombined();
            VelocityChart.SetCombined();
            GainChart.SetCombined();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            AccelData.CalculateDots(x, y, timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.Bind(AccelData.Combined.AccelPoints);
            VelocityChart.Bind(AccelData.Combined.VelocityPoints);
            GainChart.Bind(AccelData.Combined.GainPoints);
        }
    }
}

using grapher.Models.Calculations;

namespace grapher.Models.Charts.ChartState
{
    public class XYOneGraphState : ChartState
    {
        public XYOneGraphState(
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

    }
}

using grapher.Models.Calculations;
using grapher.Models.Calculations.Data;
using System.Windows.Forms;

namespace grapher.Models.Charts.ChartState
{
    public class XYOneGraphState : ChartState
    {
        public XYOneGraphState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            TableLayoutPanel chartContainer,
            EstimatedPoints xPoints,
            EstimatedPoints yPoints,
            AccelCalculator accelCalculator)
            : base(
                  sensitivityChart,
                  velocityChart,
                  gainChart,
                  chartContainer,
                  accelCalculator)
        {
            DataDirectional  = new AccelDataXYDirectional(xPoints, yPoints, accelCalculator);
            Data = DataDirectional;
            TwoDotsPerGraph = true;
        }

        private AccelDataXYDirectional DataDirectional { get; }

        public override void Activate()
        {
            SensitivityChart.SetCombined();
            VelocityChart.SetCombined();
            GainChart.SetCombined();
        }

        public override void Bind()
        {
            SensitivityChart.BindXYCombined(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXYCombined(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXYCombined(Data.X.GainPoints, Data.Y.GainPoints);
            SensitivityChart.SetMinMax(DataDirectional.SensitivityMin, DataDirectional.SensitivityMax);
            GainChart.SetMinMax(DataDirectional.GainMin, DataDirectional.GainMax);
        }
    }
}

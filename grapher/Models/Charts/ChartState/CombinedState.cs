using grapher.Models.Calculations;
using grapher.Models.Calculations.Data;
using System.Windows.Forms;

namespace grapher.Models.Charts.ChartState
{
    public class CombinedState : ChartState
    {
        public CombinedState(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            TableLayoutPanel chartContainer,
            EstimatedPoints points,
            AccelCalculator accelCalculator)
            : base(
                  sensitivityChart,
                  velocityChart,
                  gainChart,
                  chartContainer,
                  accelCalculator)
        {
            Data = new AccelDataCombined(points, accelCalculator);
        }

        public override void Activate()
        {
            SensitivityChart.SetCombined();
            VelocityChart.SetCombined();
            GainChart.SetCombined();

            SensitivityChart.ClearSecondDots();
            VelocityChart.ClearSecondDots();
            GainChart.ClearSecondDots();
        }

        public override void Bind()
        {
            SensitivityChart.Bind(Data.X.AccelPoints);
            VelocityChart.Bind(Data.X.VelocityPoints);
            GainChart.Bind(Data.X.GainPoints);
            SensitivityChart.SetMinMax(Data.X.MinAccel, Data.X.MaxAccel);
            GainChart.SetMinMax(Data.X.MinGain, Data.X.MaxGain);
        }
    }
}

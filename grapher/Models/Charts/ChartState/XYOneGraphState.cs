using grapher.Models.Calculations;
using grapher.Models.Serialized;

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
        {
            TwoDotsPerGraph = true;
        }

        public override void Activate()
        {
            SensitivityChart.SetCombined();
            VelocityChart.SetCombined();
            GainChart.SetCombined();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            Data.CalculateDotsCombinedDiffSens(x, y, timeInMs, Settings);
        }

        public override void Bind()
        {
            SensitivityChart.BindXYCombined(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXYCombined(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXYCombined(Data.X.GainPoints, Data.Y.GainPoints);
        }

        public override void Calculate(ManagedAccel accel, DriverSettings settings)
        {
            Calculator.CalculateCombinedDiffSens(Data, accel, settings, Calculator.MagnitudesCombined);
        }
    }
}

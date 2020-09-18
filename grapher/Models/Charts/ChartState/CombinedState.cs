using grapher.Models.Calculations;
using grapher.Models.Serialized;

namespace grapher.Models.Charts.ChartState
{
    public class CombinedState : ChartState
    {
        public CombinedState(
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
            SensitivityChart.SetCombined();
            VelocityChart.SetCombined();
            GainChart.SetCombined();
        }

        public override void MakeDots(int x, int y, double timeInMs)
        {
            Data.CalculateDots(x, y, timeInMs);
        }

        public override void Bind()
        {
            SensitivityChart.Bind(Data.Combined.AccelPoints);
            VelocityChart.Bind(Data.Combined.VelocityPoints);
            GainChart.Bind(Data.Combined.GainPoints);
        }

        public override void Calculate(ManagedAccel accel, DriverSettings settings)
        {
            Calculator.Calculate(Data.Combined, accel, settings.sensitivity.x, Calculator.MagnitudesCombined, true, settings);
        }
    }
}

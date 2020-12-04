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

        public override void MakeDots(double x, double y, double timeInMsRecip)
        {
            Data.CalculateDotsCombinedDiffSens(x, y, timeInMsRecip, Settings);
        }

        public override void Bind()
        {
            SensitivityChart.BindXYCombined(Data.X.AccelPoints, Data.Y.AccelPoints);
            VelocityChart.BindXYCombined(Data.X.VelocityPoints, Data.Y.VelocityPoints);
            GainChart.BindXYCombined(Data.X.GainPoints, Data.Y.GainPoints);
            SensitivityChart.SetMinMax(Data.Combined.MinAccel, Data.Combined.MaxAccel);
            GainChart.SetMinMax(Data.Combined.MinGain, Data.Combined.MaxGain);
        }

        public override void Calculate(ManagedAccel accel, DriverSettings settings)
        {
            Calculator.CalculateCombinedDiffSens(Data, accel, settings, Calculator.SimulatedInputCombined);
        }
    }
}

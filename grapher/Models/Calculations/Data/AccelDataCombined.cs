using grapher.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations.Data
{
    public class AccelDataCombined : IAccelData
    {
        public AccelDataCombined(EstimatedPoints points, AccelCalculator calculator)
        {
            X = new AccelChartData();
            Points = points;
            Calculator = calculator;
        }

        public AccelChartData X { get; }

        public AccelChartData Y { get => X; }

        private EstimatedPoints Points { get; }

        private AccelCalculator Calculator { get; }

        public void CalculateDots(double x, double y, double timeInMs)
        {
            var outVelocity = AccelCalculator.Velocity(x, y, timeInMs);

            (var inCombVel, var combSens, var combGain) = X.FindPointValuesFromOut(outVelocity);
            Points.Velocity.Set(inCombVel, outVelocity);
            Points.Sensitivity.Set(inCombVel, combSens);
            Points.Gain.Set(inCombVel, combGain);

        }

        public void Clear()
        {
            X.Clear();
        }

        public void CreateGraphData(ManagedAccel accel, DriverSettings settings)
        {
            Clear();
            Calculator.Calculate(X, accel, settings.sensitivity.x, Calculator.SimulatedInputCombined);
        }
    }
}

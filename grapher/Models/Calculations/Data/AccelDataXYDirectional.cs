using grapher.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations.Data
{
    public class AccelDataXYDirectional : IAccelData
    {
        public AccelDataXYDirectional(
            EstimatedPoints xPoints,
            EstimatedPoints yPoints,
            AccelCalculator calculator)
        {
            X = new AccelChartData();
            Y = new AccelChartData();
            XPoints = xPoints;
            YPoints = yPoints;
            Calculator = calculator;
        }

        public AccelChartData X { get; }

        public AccelChartData Y { get; }

        private EstimatedPoints XPoints { get; }

        private EstimatedPoints YPoints { get; }

        private AccelCalculator Calculator { get; }

        public void CalculateDots(double x, double y, double timeInMs)
        {
            (var xStripped, var yStripped) = AccelCalculator.StripSens(x, y, settings.sensitivity.x, settings.sensitivity.y);
            var outVelocity = AccelCalculator.Velocity(xStripped, yStripped, timeInMs);

            if (OutVelocityToPoints.TryGetValue(outVelocity, out var points))
            {
                XPoints.Sensitivity.Set(points.Item1, points.Item2);
                XPoints.Velocity.Set(points.Item1, points.Item3);
                XPoints.Gain.Set(points.Item1, points.Item4);
                YPoints.Sensitivity.Set(points.Item1, points.Item5);
                YPoints.Velocity.Set(points.Item1, points.Item6);
                YPoints.Gain.Set(points.Item1, points.Item7);
            }
            else
            {
                var index = Combined.GetVelocityIndex(outVelocity);
                var inVelocity = Combined.VelocityPoints.ElementAt(index).Key;
                var xPoints = X.ValuesAtIndex(index);
                var yPoints = Y.ValuesAtIndex(index);
                OutVelocityToPoints.Add(outVelocity, (inVelocity, xPoints.Item1, xPoints.Item2, xPoints.Item3, yPoints.Item1, yPoints.Item2, yPoints.Item3));
                EstimatedX.Sensitivity.Set(inVelocity, xPoints.Item1);
                EstimatedX.Velocity.Set(inVelocity, xPoints.Item2);
                EstimatedX.Gain.Set(inVelocity, xPoints.Item3);
                EstimatedY.Sensitivity.Set(inVelocity, yPoints.Item1);
                EstimatedY.Velocity.Set(inVelocity, yPoints.Item2);
                EstimatedY.Gain.Set(inVelocity, yPoints.Item3);
            }

        }

        public void Clear()
        {
            X.Clear();
            Y.Clear();
        }

        public void CreateGraphData(ManagedAccel accel, DriverSettings settings)
        {
            Clear();
        }

    }
}

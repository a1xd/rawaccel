using grapher.Models.Charts;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;

namespace grapher.Models.Calculations
{
    public class AccelData
    {
        #region Constructors

        public AccelData(
            EstimatedPoints combined,
            EstimatedPoints x,
            EstimatedPoints y)
        {
            Combined = new AccelChartData();
            X = new AccelChartData();
            Y = new AccelChartData();

            Estimated = combined;
            EstimatedX = x;
            EstimatedY = y;

            OutVelocityToPoints = new Dictionary<double, (double, double, double, double, double, double, double)>();
        }

        #endregion Constructors

        #region Properties

        public AccelChartData Combined { get; }

        public AccelChartData X { get; }

        public AccelChartData Y { get; }

        private EstimatedPoints Estimated { get; }

        private EstimatedPoints EstimatedX { get; }

        private EstimatedPoints EstimatedY { get; }

        private Dictionary<double, (double, double, double, double, double, double, double)> OutVelocityToPoints { get; }

        #endregion Properties

        #region Methods

        public void Clear()
        {
            Combined.Clear();
            X.Clear();
            Y.Clear();
            OutVelocityToPoints.Clear();
        }

        public void CalculateDots(double x, double y, double timeInMs)
        {
            var outVelocity = AccelCalculator.Velocity(x, y, timeInMs);

            (var inCombVel, var combSens, var combGain) = Combined.FindPointValuesFromOut(outVelocity);
            Estimated.Velocity.Set(inCombVel, outVelocity);
            Estimated.Sensitivity.Set(inCombVel, combSens);
            Estimated.Gain.Set(inCombVel, combGain);
        }

        public void CalculateDotsXY(double x, double y, double timeInMs)
        {
            var outX = Math.Abs(x) / timeInMs;
            var outY = Math.Abs(y) / timeInMs;

            (var inXVelocity, var xSensitivity, var xGain) = X.FindPointValuesFromOut(outX);
            EstimatedX.Velocity.Set(inXVelocity, outX);
            EstimatedX.Sensitivity.Set(inXVelocity, xSensitivity);
            EstimatedX.Gain.Set(inXVelocity, xGain);

            (var inYVelocity, var ySensitivity, var yGain) = Y.FindPointValuesFromOut(outY);
            EstimatedY.Velocity.Set(inYVelocity, outY);
            EstimatedY.Sensitivity.Set(inYVelocity, ySensitivity);
            EstimatedY.Gain.Set(inYVelocity, yGain);
        }

        public void CalculateDotsCombinedDiffSens(double x, double y, double timeInMs, DriverSettings settings)
        {
            (var xStripped, var yStripped) = AccelCalculator.StripSens(x, y, settings.sensitivity.x, settings.sensitivity.y);
            var outVelocity = AccelCalculator.Velocity(xStripped, yStripped, timeInMs);

            if (OutVelocityToPoints.TryGetValue(outVelocity, out var points))
            {
                EstimatedX.Sensitivity.Set(points.Item1, points.Item2);
                EstimatedX.Velocity.Set(points.Item1, points.Item3);
                EstimatedX.Gain.Set(points.Item1, points.Item4);
                EstimatedY.Sensitivity.Set(points.Item1, points.Item5);
                EstimatedY.Velocity.Set(points.Item1, points.Item6);
                EstimatedY.Gain.Set(points.Item1, points.Item7);
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

        #endregion Methods
    }
}

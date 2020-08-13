using grapher.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static grapher.AccelCharts;

namespace grapher.Models.Calculations
{
    public class AccelData
    {

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
        }

        public AccelChartData Combined { get; }

        public AccelChartData X { get; }

        public AccelChartData Y { get; }

        private EstimatedPoints Estimated { get; }

        private EstimatedPoints EstimatedX { get; }

        private EstimatedPoints EstimatedY { get; }

        public void Clear()
        {
            Combined.Clear();
            X.Clear();
            Y.Clear();
        }

        public void CalculateDots(int x, int y)
        {
            var magnitude = AccelCalculator.Magnitude(x, y);

            (var inCombVel, var combSens, var combGain) = Combined.FindInValuesFromOut(magnitude);
            Estimated.Velocity.Set(inCombVel, magnitude);
            Estimated.Sensitivity.Set(inCombVel, combSens);
            Estimated.Gain.Set(inCombVel, combGain);
        }

        public void CalculateDotsXY(int x, int y)
        {
            var magnitudeX = Math.Abs(x);
            var magnitudeY = Math.Abs(y);

            (var inXVel, var xSens, var xGain) = X.FindInValuesFromOut(magnitudeX);
            EstimatedX.Velocity.Set(inXVel, magnitudeX);
            EstimatedX.Sensitivity.Set(inXVel, xSens);
            EstimatedX.Gain.Set(inXVel, xGain);

            (var inYVel, var ySens, var yGain) = Y.FindInValuesFromOut(magnitudeY);
            EstimatedY.Velocity.Set(inYVel, magnitudeY);
            EstimatedY.Sensitivity.Set(inYVel, ySens);
            EstimatedY.Gain.Set(inYVel, yGain);
        }

    }
}

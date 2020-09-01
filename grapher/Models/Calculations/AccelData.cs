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
        }

        #endregion Constructors

        #region Properties

        public AccelChartData Combined { get; }

        public AccelChartData X { get; }

        public AccelChartData Y { get; }

        private EstimatedPoints Estimated { get; }

        private EstimatedPoints EstimatedX { get; }

        private EstimatedPoints EstimatedY { get; }

        #endregion Properties

        #region Methods

        public void Clear()
        {
            Combined.Clear();
            X.Clear();
            Y.Clear();
        }

        public void CalculateDots(int x, int y, double timeInMs)
        {
            var magnitude = AccelCalculator.Velocity(x, y, timeInMs);

            (var inCombVel, var combSens, var combGain) = Combined.FindPointValuesFromOut(magnitude);
            Estimated.Velocity.Set(inCombVel, magnitude);
            Estimated.Sensitivity.Set(inCombVel, combSens);
            Estimated.Gain.Set(inCombVel, combGain);
        }

        public void CalculateDotsXY(int x, int y, double timeInMs)
        {
            var outX = Math.Abs(x);
            var outY = Math.Abs(y);

            (var inXVelocity, var xSensitivity, var xGain) = X.FindPointValuesFromOut(outX);
            EstimatedX.Velocity.Set(inXVelocity, outX);
            EstimatedX.Sensitivity.Set(inXVelocity, xSensitivity);
            EstimatedX.Gain.Set(inXVelocity, xGain);

            (var inYVelocity, var ySensitivity, var yGain) = Y.FindPointValuesFromOut(outY);
            EstimatedY.Velocity.Set(inYVelocity, outY);
            EstimatedY.Sensitivity.Set(inYVelocity, ySensitivity);
            EstimatedY.Gain.Set(inYVelocity, yGain);
        }

        #endregion Methods
    }
}

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

        public AccelData()
        {
            Combined = new AccelChartData();
            X = new AccelChartData();
            Y = new AccelChartData();
        }

        public AccelChartData Combined { get; }

        public AccelChartData X { get; }

        public AccelChartData Y { get; }

        public void Clear()
        {
            Combined.Clear();
            X.Clear();
            Y.Clear();
        }

        public void CalculateDots(int x, int y, ref EstimatedPoints estimation)
        {
            var magnitude = AccelCalculator.Magnitude(x, y);

            estimation.CombinedVelocity.Y = magnitude;
            estimation.XVelocity.Y = Math.Abs(x);
            estimation.YVelocity.Y = Math.Abs(y);

            (var inCombVel, var combSens, var combGain) = Combined.FindInValuesFromOut(magnitude);
            estimation.CombinedVelocity.X = inCombVel;
            estimation.CombinedSensitivity.X = inCombVel;
            estimation.CombinedGain.X = inCombVel;
            estimation.CombinedSensitivity.Y = combSens;
            estimation.CombinedGain.Y = combGain;

            (var inXVel, var xSens, var xGain) = X.FindInValuesFromOut(estimation.XVelocity.Y);
            estimation.XVelocity.X = inXVel;
            estimation.XSensitivity.X = inXVel;
            estimation.XGain.X = inXVel;
            estimation.XSensitivity.Y = xSens;
            estimation.XGain.Y = xGain;

            (var inYVel, var ySens, var yGain) = Y.FindInValuesFromOut(estimation.YVelocity.Y);
            estimation.YVelocity.X = inYVel;
            estimation.YSensitivity.X = inYVel;
            estimation.YGain.X = inYVel;
            estimation.YSensitivity.Y = ySens;
            estimation.YGain.Y = yGain;
        }
    }
}

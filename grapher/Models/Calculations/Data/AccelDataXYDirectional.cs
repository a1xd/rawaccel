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
            XPoints = xPoints;
            YPoints = yPoints;
            Calculator = calculator;
            AngleToData = new AccelChartData[Constants.AngleDivisions];
            FillAngleData();
        }

        public AccelChartData X { get => AngleToData[0]; }

        public AccelChartData Y { get => AngleToData[Constants.AngleDivisions-1]; }

        public double SensitivityMax { get => X.MaxAccel; }

        public double SensitivityMin { get => X.MinAccel; }

        public double GainMax { get => X.MaxGain; }

        public double GainMin { get => X.MinGain; }

        private AccelChartData[] AngleToData { get; }

        private EstimatedPoints XPoints { get; }

        private EstimatedPoints YPoints { get; }

        private AccelCalculator Calculator { get; }

        public void CalculateDots(double x, double y, double timeInMs)
        {
            var outVelocity = AccelCalculator.Velocity(x, y, timeInMs);
            var outAngle = Math.Atan2(Math.Abs(y),Math.Abs(x));
            var nearestAngleDivision = AccelCalculator.NearestAngleDivision(outAngle);
            var data = AngleToData[nearestAngleDivision];
            var index = data.GetVelocityIndex(outVelocity);
            var inVelocity = data.VelocityPoints.ElementAt(index).Key;
            var xPoints = X.ValuesAtIndex(index);
            var yPoints = Y.ValuesAtIndex(index);
            XPoints.Sensitivity.Set(inVelocity, xPoints.Item1);
            XPoints.Velocity.Set(inVelocity, xPoints.Item2);
            XPoints.Gain.Set(inVelocity, xPoints.Item3);
            YPoints.Sensitivity.Set(inVelocity, yPoints.Item1);
            YPoints.Velocity.Set(inVelocity, yPoints.Item2);
            YPoints.Gain.Set(inVelocity, yPoints.Item3);
        }

        public void Clear()
        {
            foreach (var data in AngleToData)
            {
                data.Clear();
            }
        }

        public void CreateGraphData(ManagedAccel accel, DriverSettings settings)
        {
            Clear();
            Calculator.CalculateDirectional(AngleToData, accel, settings, Calculator.SimulatedDirectionalInput);
        }

        private void FillAngleData()
        {
            for(int i=0; i < Constants.AngleDivisions; i++)
            {
                AngleToData[i] = new AccelChartData();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace grapher.Models.Calculations
{
    public class AccelChartData
    {
        #region Constructors

        public AccelChartData()
        {
            AccelPoints = new SortedDictionary<double, double>();
            VelocityPoints = new SortedDictionary<double, double>();
            GainPoints = new SortedDictionary<double, double>();
            OrderedVelocityPointsList = new List<double>();
            OutVelocityToPoints = new Dictionary<double, (double, double, double)>();
        }

        #endregion Constructors

        #region Properties

        public SortedDictionary<double, double> AccelPoints { get; }

        public double MaxAccel { get; set; }

        public double MinAccel { get; set; }

        public double MaxGain { get; set; }

        public double MinGain { get; set; }

        public SortedDictionary<double, double> VelocityPoints { get; }

        public SortedDictionary<double, double> GainPoints { get; }

        public List<double> OrderedVelocityPointsList { get; }

        public Dictionary<double, (double, double, double)> OutVelocityToPoints { get; }

        #endregion Properties

        #region Methods

        public void Clear()
        {
            AccelPoints.Clear();
            VelocityPoints.Clear();
            GainPoints.Clear();
            OrderedVelocityPointsList.Clear();
            OutVelocityToPoints.Clear();
        }

        public (double, double, double) FindPointValuesFromOut(double outVelocityValue)
        {
            if (OutVelocityToPoints.TryGetValue(outVelocityValue, out var values))
            {
                return values;
            }
            else
            {
                var velIdx = GetVelocityIndex(outVelocityValue);

                values = (VelocityPoints.ElementAt(velIdx).Key, AccelPoints.ElementAt(velIdx).Value, GainPoints.ElementAt(velIdx).Value);
                OutVelocityToPoints.Add(outVelocityValue, values);
                return values;
            }
        }

        public (double, double, double) ValuesAtIndex(int index)
        {
            return (AccelPoints.ElementAt(index).Value, VelocityPoints.ElementAt(index).Value, GainPoints.ElementAt(index).Value);
        }

        public (double, double, double) ValuesAtInVelocity(double inVelocity)
        {
            return (AccelPoints[inVelocity], VelocityPoints[inVelocity], GainPoints[inVelocity]);
        }

        public int GetVelocityIndex(double outVelocityValue)
        {
            var velIdx = OrderedVelocityPointsList.BinarySearch(outVelocityValue);

            if (velIdx < 0)
            {
                velIdx = ~velIdx;
            }

            velIdx = Math.Min(velIdx, VelocityPoints.Count - 1);
            velIdx = Math.Max(velIdx, 0);

            return velIdx;
        }

        #endregion Methods
    }
}

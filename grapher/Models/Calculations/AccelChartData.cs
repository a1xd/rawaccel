using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations
{
    public class AccelChartData
    {
        public AccelChartData()
        {
            AccelPoints = new SortedDictionary<double, double>();
            VelocityPoints = new SortedDictionary<double, double>();
            GainPoints = new SortedDictionary<double, double>();
        }

        public SortedDictionary<double, double> AccelPoints { get; }

        public SortedDictionary<double, double> VelocityPoints { get; }

        public SortedDictionary<double, double> GainPoints { get; }

        public void Clear()
        {
            AccelPoints.Clear();
            VelocityPoints.Clear();
            GainPoints.Clear();
        }

        public (double, double, double) FindInValuesFromOut(double outVelocityValue)
        {
            var velIdx = ~VelocityPoints.Values.ToList().BinarySearch(outVelocityValue);

            if (velIdx < 0)
            {
                velIdx = ~velIdx;
            }

            return (VelocityPoints.ElementAt(velIdx).Key, AccelPoints.ElementAt(velIdx).Value, GainPoints.ElementAt(velIdx).Value);
        }
    }
}

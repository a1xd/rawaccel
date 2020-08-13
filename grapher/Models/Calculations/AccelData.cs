using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations
{
    public class AccelData
    {
        public AccelData()
        {
            OrderedAccelPoints = new SortedDictionary<double, double>();
            OrderedVelocityPoints = new SortedDictionary<double, double>();
            OrderedGainPoints = new SortedDictionary<double, double>();
        }

        public SortedDictionary<double, double> OrderedAccelPoints { get; }

        public SortedDictionary<double, double> OrderedVelocityPoints { get; }

        public SortedDictionary<double, double> OrderedGainPoints { get; }

        public void Clear()
        {
            OrderedAccelPoints.Clear();
            OrderedVelocityPoints.Clear();
            OrderedGainPoints.Clear();
        }

    }
}

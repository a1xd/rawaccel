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
    }
}

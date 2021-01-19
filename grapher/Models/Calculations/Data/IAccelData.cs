using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations.Data
{
    public interface IAccelData
    {
        void CalculateDots(double x, double y, double timeInMs);

        void CreateGraphData(ManagedAccel accel, DriverSettings settings);

        void Clear();

        AccelChartData X { get; }

        AccelChartData Y { get; }
    }
}

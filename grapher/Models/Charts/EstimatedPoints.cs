using grapher.Models.Mouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Charts
{
    public class EstimatedPoints
    {
        public EstimatedPoints()
        {
            Sensitivity = new PointData();
            Velocity = new PointData();
            Gain = new PointData();
        }

        public PointData Sensitivity { get; }

        public PointData Velocity { get; }

        public PointData Gain { get; }
    }
}

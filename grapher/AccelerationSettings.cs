using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher
{
    public class AccelerationSettings
    {
        public double SensitivityX { get; }

        public double SensitivityY { get; }

        public double Rotation { get; }

        public double Offset { get; }

        public double WeightX { get; }

        public double WeightY { get; }

        public double CapX { get; }

        public double CapY { get; }

        public int AccelerationType { get; }

        public double Acceleration { get; }

        public double LimitOrExponent { get; }

        public double Midpoint { get; }
    }
}

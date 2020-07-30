using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher
{
    public class VectorXY
    {
        public VectorXY(double x)
        {
            X = x;
            Y = x;
        }

        public VectorXY(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public void SetBoth(double value)
        {
            X = value;
            Y = value;
        }
    }
}

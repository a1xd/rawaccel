using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class SigmoidLayout : LayoutBase
    {
        public SigmoidLayout()
        {
            Name = "Sigmoid";
            Index = 5;
            Show = new bool[] { true, true, true }; 
            OptionNames = new string[] { Acceleration, Limit, Midpoint }; 
        }
    }
}

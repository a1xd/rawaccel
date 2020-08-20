using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class SigmoidGainLayout : LayoutBase
    {
        public SigmoidGainLayout()
            : base()
        {
            Name = "SigmoidGain";
            Index = 8;
            ShowOptions = new bool[] { true, true, true, true }; 
            OptionNames = new string[] { Offset, Acceleration, Limit, Midpoint }; 
        }
    }
}

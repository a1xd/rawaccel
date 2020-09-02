using grapher.Models.Serialized;
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
            : base()
        {
            Name = "Sigmoid";
            Index = (int)AccelMode.sigmoid;
            ShowOptions = new bool[] { true, true, true, true, false, true }; 
            OptionNames = new string[] { Offset, Acceleration, Limit, Midpoint, string.Empty, Weight  }; 
        }
    }
}

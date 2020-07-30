using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class PowerLayout : LayoutBase
    {
        public PowerLayout()
        {
            Name = "Power";
            Index = 6;
            Show = new bool[] { true, true, false }; 
            OptionNames = new string[] { Scale, Exponent, string.Empty }; 
        }
    }
}

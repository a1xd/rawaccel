using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class NaturalLayout : LayoutBase
    {
        public NaturalLayout()
        {
            Name = "Natural";
            Index = 3;
            Show = new bool[] { true, true, false }; 
            OptionNames = new string[] { Acceleration, Limit, string.Empty }; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class ClassicLayout : LayoutBase
    {
        public ClassicLayout()
        {
            Name = "Classic";
            Index = 2;
            Show = new bool[] { true, true, false }; 
            OptionNames = new string[] { Acceleration, Exponent, string.Empty }; 
        }
    }
}

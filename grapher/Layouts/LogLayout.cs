using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class LogLayout : LayoutBase
    {
        public LogLayout()
        {
            Name = "Logarithmic";
            Index = 4;
            Show = new bool[] { true, false, false }; 
            OptionNames = new string[] { Acceleration, string.Empty, string.Empty }; 
        }
    }
}

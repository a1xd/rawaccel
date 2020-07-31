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
            : base()
        {
            Name = "Logarithmic";
            Index = 4;
            ShowOptions = new bool[] { true, true, false, false }; 
            OptionNames = new string[] { Offset, Acceleration, string.Empty, string.Empty }; 
        }
    }
}

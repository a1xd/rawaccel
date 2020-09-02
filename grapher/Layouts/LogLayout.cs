using grapher.Models.Serialized;
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
            Index = (int)AccelMode.logarithmic;
            ShowOptions = new bool[] { true, true, false, false, true, true }; 
            OptionNames = new string[] { Offset, Acceleration, string.Empty, string.Empty, Cap, Weight }; 
        }
    }
}

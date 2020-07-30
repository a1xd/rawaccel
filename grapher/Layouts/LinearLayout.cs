using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class LinearLayout : LayoutBase
    {
        public LinearLayout()
        {
            Name = "Linear";
            Index = 1;
            Show = new bool[] { true, false, false }; 
            OptionNames = new string[] { Acceleration, string.Empty, string.Empty }; 
        }
    }
}

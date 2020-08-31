using grapher.Models.Serialized;
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
            : base()
        {
            Name = "Linear";
            Index = (int)AccelMode.linear;
            ShowOptions = new bool[] { true, true, false, false }; 
            OptionNames = new string[] { Offset, Acceleration, string.Empty, string.Empty }; 
        }
    }
}

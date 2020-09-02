using grapher.Models.Serialized;
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
            : base()
        {
            Name = "Power";
            Index = (int)AccelMode.power;
            ShowOptions = new bool[] { true, true, true, false, true, true }; 
            OptionNames = new string[] { Offset, Scale, Exponent, string.Empty, Cap, Weight  }; 
        }
    }
}

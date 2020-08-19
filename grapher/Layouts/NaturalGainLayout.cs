using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class NaturalGainLayout : LayoutBase
    {
        public NaturalGainLayout()
            : base()
        {
            Name = "NaturalGain";
            Index = 7;
            ShowOptions = new bool[] { true, true, true, false }; 
            OptionNames = new string[] { Offset, Acceleration, Limit, string.Empty }; 
        }
    }
}

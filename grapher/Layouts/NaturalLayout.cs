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
            : base()
        {
            Name = "Natural";
            Index = 3;
            ShowOptions = new bool[] { true, true, true, false }; 
            OptionNames = new string[] { Offset, Acceleration, Limit, string.Empty }; 
        }
    }
}

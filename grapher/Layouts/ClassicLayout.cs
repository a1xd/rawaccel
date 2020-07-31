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
            : base()
        {
            Name = "Classic";
            Index = 2;
            ShowOptions = new bool[] { true, true, true, false }; 
            OptionNames = new string[] { Offset, Acceleration, Exponent, string.Empty }; 
        }
    }
}

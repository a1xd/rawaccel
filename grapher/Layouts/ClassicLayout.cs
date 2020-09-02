using grapher.Models.Serialized;
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
            Index = (int)AccelMode.classic;
            ShowOptions = new bool[] { true, true, true, false, true, true }; 
            OptionNames = new string[] { Offset, Acceleration, Exponent, string.Empty, Cap, Weight }; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class OffLayout : LayoutBase
    {
        public OffLayout()
            : base()
        {
            Name = "Off";
            Index = 7;
            ShowOptions = new bool[] { false, false, false, false }; 
            OptionNames = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };
            ShowOptionsXY = new bool[] { false, false }; 
            ButtonEnabled = true;
        }
    }
}

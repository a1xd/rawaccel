using grapher.Models.Serialized;
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
            Index = (int)AccelMode.noaccel;
            ShowOptions = new bool[] { false, false, false, false, false, false }; 
            OptionNames = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            ButtonEnabled = true;
        }
    }
}

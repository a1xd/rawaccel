using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace grapher.Layouts
{
    public class DefaultLayout : LayoutBase
    {
        public DefaultLayout()
            : base()
        {
            Name = "Default";
            Index = (int)AccelMode.noaccel;
            ShowOptions = new bool[] { true, true, true, true }; 
            OptionNames = new string[] { Offset, Acceleration, $"{Limit}\\{Exponent}", Midpoint };
            ButtonEnabled = false;
        }
    }
}

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
        {
            Name = "Off";
            Index = 0;
            Show = new bool[] { true, true, true }; 
            OptionNames = new string[] { Acceleration, $"{Limit}\\{Exponent}", Midpoint }; 
        }
    }
}

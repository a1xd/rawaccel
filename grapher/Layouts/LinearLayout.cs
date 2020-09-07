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

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

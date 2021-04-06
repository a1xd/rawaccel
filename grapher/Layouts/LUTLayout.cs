using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class LUTLayout : LayoutBase
    {
        public const string LUTLayoutText = "This mode is for advanced users only. It requires a lut.json file to define the velocity curve. See the guide for specifics.";

        public LUTLayout()
            : base()
        {
            Name = "LookUpTable";
            Index = (int)AccelMode.lut;

            AccelLayout = new OptionLayout(false, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, Cap);
            WeightLayout = new OptionLayout(false, Weight);
            OffsetLayout = new OptionLayout(false, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, Exponent);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LUTTextLayout = new OptionLayout(true, LUTLayoutText);
        }
    }
}

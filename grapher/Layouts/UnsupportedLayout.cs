using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class UnsupportedLayout : LayoutBase
    {
        public const string LUTLayoutText = "This mode is unsupported by this program. See the guide for details.";

        public UnsupportedLayout()
            : base()
        {
            Name = "Unsupported";
            Mode = AccelMode.noaccel + 1;

            GainSwitchOptionLayout = new OptionLayout(false, string.Empty);
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
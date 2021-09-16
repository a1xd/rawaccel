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
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, Exponent);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(true, LUTLayoutText);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}
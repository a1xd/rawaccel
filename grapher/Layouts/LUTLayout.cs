using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class LUTLayout : LayoutBase
    {
        /// <summary>
        /// String small enough to fit in active value label
        /// </summary>
        public const string LUTActiveName = "LUT";

        public LUTLayout()
            : base()
        {
            Name = "LookUpTable";
            Mode = AccelMode.lut;

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
            LutTextLayout = new OptionLayout(true, string.Empty);
            LutPanelLayout = new OptionLayout(true, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(true, string.Empty);
        }

        public override string ActiveName => LUTActiveName;
    }
}

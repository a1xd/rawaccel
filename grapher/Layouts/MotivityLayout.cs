using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class MotivityLayout : LayoutBase
    {
         public MotivityLayout()
            : base()
        {
            Name = "Motivity";
            Mode = AccelMode.motivity;
            LogarithmicCharts = true;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(true, GrowthRate);
            SmoothLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(true, Motivity);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(true, Midpoint);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}

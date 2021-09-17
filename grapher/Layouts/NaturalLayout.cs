using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class NaturalLayout : LayoutBase
    {
        public NaturalLayout()
            : base()
        {
            Name = "Natural";
            Mode = AccelMode.natural;
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(true, DecayRate);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(true, Limit);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            PowerStartsFromLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}

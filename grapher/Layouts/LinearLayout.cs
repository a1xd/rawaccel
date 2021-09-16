using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class LinearLayout : LayoutBase
    {
        public const string LinearName = "Linear";

        public LinearLayout()
            : base()
        {
            Name = LinearName;
            Mode = AccelMode.classic;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(true, CapType);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}

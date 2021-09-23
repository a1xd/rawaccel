using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class JumpLayout : LayoutBase
    {
        public JumpLayout()
            : base()
        {
            Name = "Jump";
            Mode = AccelMode.jump;
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            AccelLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(true, Smooth);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(false, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, Limit);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}

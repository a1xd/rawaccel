using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class DefaultLayout : LayoutBase
    {
        public DefaultLayout()
            : base()
        {
            Name = "Default";
            Mode = AccelMode.noaccel;
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(true, Scale);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(true, Limit);
            PowerClassicLayout = new OptionLayout(true, PowerClassic);
            ExponentLayout = new OptionLayout(true, Exponent);
            MidpointLayout = new OptionLayout(true, Midpoint);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
        }
    }
}

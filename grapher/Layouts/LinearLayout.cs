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
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(false, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LUTTextLayout = new OptionLayout(false, string.Empty);
        }
    }
}

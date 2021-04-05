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
            Index = (int)AccelMode.classic;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

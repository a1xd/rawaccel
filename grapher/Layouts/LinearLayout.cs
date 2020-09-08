using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class LinearLayout : LayoutBase
    {
        public LinearLayout()
            : base()
        {
            Name = "Linear";
            Index = (int)AccelMode.linear;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

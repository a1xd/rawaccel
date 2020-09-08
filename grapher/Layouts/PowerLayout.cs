using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class PowerLayout : LayoutBase
    {
        public PowerLayout()
            : base()
        {
            Name = "Power";
            Index = (int)AccelMode.power;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(true, Limit);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

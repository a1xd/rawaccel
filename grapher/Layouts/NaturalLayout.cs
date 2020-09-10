using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class NaturalLayout : LayoutBase
    {
        public NaturalLayout()
            : base()
        {
            Name = "Natural";
            Index = (int)AccelMode.natural;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(true, Limit);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class ClassicLayout : LayoutBase
    {
        public ClassicLayout()
            : base()
        {
            Name = "Classic";
            Index = (int)AccelMode.classic;

            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(true, Exponent);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LUTTextLayout = new OptionLayout(false, string.Empty);
        }
    }
}

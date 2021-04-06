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
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(true, Limit);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LUTTextLayout = new OptionLayout(false, string.Empty);
        }
    }
}

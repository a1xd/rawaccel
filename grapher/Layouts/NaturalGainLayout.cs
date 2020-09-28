using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class NaturalGainLayout : LayoutBase
    {
        public NaturalGainLayout()
            : base()
        {
            Name = "NaturalGain";
            Index = (int)AccelMode.naturalgain;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(true, Limit);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

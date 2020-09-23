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
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(true, Limit);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

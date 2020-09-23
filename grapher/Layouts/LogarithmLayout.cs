using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class LogarithmLayout : LayoutBase
    {
        public LogarithmLayout ()
            : base()
        {
            Name = "Logarithm";
            Index = (int)AccelMode.logarithm;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Scale);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

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
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(false, string.Empty);
            ScaleLayout = new OptionLayout(true, Scale);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(true, Exponent);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

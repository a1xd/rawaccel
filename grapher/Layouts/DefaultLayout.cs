using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class DefaultLayout : LayoutBase
    {
        public DefaultLayout()
            : base()
        {
            Name = "Default";
            Index = (int)AccelMode.noaccel;
            ButtonEnabled = false;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(true, Cap);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimExpLayout = new OptionLayout(true, $"{Limit}\\{Exponent}");
            MidpointLayout = new OptionLayout(true, Midpoint);
        }
    }
}

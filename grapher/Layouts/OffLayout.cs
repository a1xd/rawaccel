using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class OffLayout : LayoutBase
    {
        public OffLayout()
            : base()
        {
            Name = "Off";
            Index = (int)AccelMode.noaccel;
            ButtonEnabled = true;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimExpLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

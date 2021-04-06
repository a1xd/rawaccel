using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class JumpLayout : LayoutBase
    {
        public JumpLayout()
            : base()
        {
            Name = "Jump";
            Index = (int)AccelMode.jump;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(false, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(true, string.Empty);
            WeightLayout = new OptionLayout(false, Weight);
            OffsetLayout = new OptionLayout(true, Offset);
            LimitLayout = new OptionLayout(false, Limit);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
        }
    }
}

using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class SigmoidGainLayout : LayoutBase
    {
        public SigmoidGainLayout()
            : base()
        {
            Name = "SigmoidGain";
            Index = (int)AccelMode.sigmoidgain;
            LogarithmicCharts = false;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimExpLayout = new OptionLayout(true, Limit);
            MidpointLayout = new OptionLayout(true, Midpoint);
        }
    }
}

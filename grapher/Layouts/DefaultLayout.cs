using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class DefaultLayout : LayoutBase
    {
        public DefaultLayout()
            : base()
        {
            Name = "Default";
            Mode = AccelMode.noaccel;
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(true, DecayRate);
            GammaLayout = new OptionLayout(true, Gamma);
            SmoothLayout = new OptionLayout(true, Smooth);
            InputOffsetLayout = new OptionLayout(true, InputOffset);
            LimitLayout = new OptionLayout(true, Limit);
            PowerClassicLayout = new OptionLayout(true, PowerClassic);
            ExponentLayout = new OptionLayout(true, Exponent);
            OutputOffsetLayout = new OptionLayout(false, string.Empty);
            SyncSpeedLayout = new OptionLayout(true, SyncSpeed);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
            InputJumpLayout = new OptionLayout(false, string.Empty);
            OutputJumpLayout = new OptionLayout(false, string.Empty);
        }
    }
}

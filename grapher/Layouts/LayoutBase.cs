using grapher.Models.Options;

namespace grapher.Layouts
{
    public abstract class LayoutBase
    {
        public const string Acceleration = "Acceleration";
        public const string GrowthRate = "Growth Rate";
        public const string DecayRate = "Decay Rate";
        public const string Scale = "Scale";
        public const string Exponent = "Exponent";
        public const string PowerClassic = "Power";
        public const string Limit = "Limit";
        public const string Midpoint = "Midpoint";
        public const string Motivity = "Motivity";
        public const string Offset = "Offset";
        public const string Cap = "Cap";
        public const string Weight = "Weight";
        public const string Smooth = "Smooth";
        public const string Gain = "Gain";

        public LayoutBase()
        {
            AccelLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(false, string.Empty);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
            GainSwitchOptionLayout = new OptionLayout(false, string.Empty);

            LogarithmicCharts = false;
        }

        public AccelMode Mode { get; protected set; }

        public string Name { get; protected set; }

        public virtual string ActiveName { get => Name; }

        public bool LogarithmicCharts { get; protected set; }

        protected OptionLayout AccelLayout { get; set; }

        protected OptionLayout DecayRateLayout { get; set; }

        protected OptionLayout GrowthRateLayout { get; set; }

        protected OptionLayout SmoothLayout { get; set; }

        protected OptionLayout ScaleLayout { get; set; }

        protected OptionLayout CapLayout { get; set; }

        protected OptionLayout WeightLayout { get; set; }

        protected OptionLayout OffsetLayout { get; set; }

        protected OptionLayout LimitLayout { get; set; }

        protected OptionLayout PowerClassicLayout { get; set; }

        protected OptionLayout ExponentLayout { get; set; }

        protected OptionLayout MidpointLayout { get; set; }

        protected OptionLayout LutTextLayout { get; set; }

        protected OptionLayout LutPanelLayout { get; set; }

        protected OptionLayout LutApplyOptionsLayout { get; set; }

        protected OptionLayout GainSwitchOptionLayout { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void Layout(
            IOption gainSwitchOption,
            IOption accelOption,
            IOption decayRateOption,
            IOption growthRateOption,
            IOption smoothOption,
            IOption scaleOption,
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limitOption,
            IOption powerClassicOption,
            IOption expOption,
            IOption midpointOption,
            IOption lutTextOption,
            IOption lutPanelOption,
            IOption lutApplyOption,
            int top)
        {

            IOption previous = null;

            foreach (var option in new (OptionLayout, IOption)[] {
                (GainSwitchOptionLayout, gainSwitchOption),
                (AccelLayout, accelOption),
                (DecayRateLayout, decayRateOption),
                (GrowthRateLayout, growthRateOption),
                (SmoothLayout, smoothOption),
                (ScaleLayout, scaleOption),
                (CapLayout, capOption),
                (WeightLayout, weightOption),
                (OffsetLayout, offsetOption),
                (LimitLayout, limitOption),
                (PowerClassicLayout, powerClassicOption),
                (ExponentLayout, expOption),
                (MidpointLayout, midpointOption),
                (LutTextLayout, lutTextOption),
                (LutPanelLayout, lutPanelOption),
                (LutApplyOptionsLayout, lutApplyOption)})
            {
                option.Item1.Layout(option.Item2);

                if (option.Item2.Visible)
                {
                    if (previous != null)
                    {
                        option.Item2.SnapTo(previous);
                    }
                    else
                    {
                        option.Item2.Top = top;
                    }

                    previous = option.Item2;
                }
            }
        }

        public void Layout(
            IOption gainSwitchOption,
            IOption accelOption,
            IOption decayRateOption,
            IOption growthRateOption,
            IOption smoothOption,
            IOption scaleOption,
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limitOption,
            IOption powerClassicOption,
            IOption expOption,
            IOption midpointOption,
            IOption lutTextOption,
            IOption lutPanelOption,
            IOption lutApplyOption)
        {
            Layout(gainSwitchOption,
                accelOption,
                decayRateOption,
                growthRateOption,
                smoothOption,
                scaleOption,
                capOption,
                weightOption,
                offsetOption,
                limitOption,
                powerClassicOption,
                expOption,
                midpointOption,
                lutTextOption,
                lutPanelOption,
                lutApplyOption,
                accelOption.Top);
        }
    }
}

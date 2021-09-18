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
        public const string OutputOffset = "Output Offset";
        public const string PowerClassic = "Power";
        public const string Limit = "Limit";
        public const string Midpoint = "Midpoint";
        public const string Motivity = "Motivity";
        public const string InputOffset = "Input Offset";
        public const string CapType = "Cap Type";
        public const string Weight = "Weight";
        public const string Smooth = "Smooth";
        public const string Gain = "Gain";

        public LayoutBase()
        {
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GrowthRateLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            InputOffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(false, string.Empty);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            OutputOffsetLayout = new OptionLayout(false, string.Empty);
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

        protected OptionLayout DecayRateLayout { get; set; }

        protected OptionLayout GrowthRateLayout { get; set; }

        protected OptionLayout SmoothLayout { get; set; }

        protected OptionLayout ClassicCapLayout { get; set; }

        protected OptionLayout PowerCapLayout { get; set; }

        protected OptionLayout InputOffsetLayout { get; set; }

        protected OptionLayout LimitLayout { get; set; }

        protected OptionLayout PowerClassicLayout { get; set; }

        protected OptionLayout ExponentLayout { get; set; }

        protected OptionLayout OutputOffsetLayout { get; set; }

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
            IOption classicCapOption,
            IOption powerCapOption,
            IOption decayRateOption,
            IOption growthRateOption,
            IOption smoothOption,
            IOption inputOffsetOption,
            IOption limitOption,
            IOption powerClassicOption,
            IOption expOption,
            IOption outputOffsetOption,
            IOption midpointOption,
            IOption lutTextOption,
            IOption lutPanelOption,
            IOption lutApplyOption,
            int top)
        {

            IOption previous = null;

            foreach (var option in new (OptionLayout, IOption)[] {
                (GainSwitchOptionLayout, gainSwitchOption),
                (ClassicCapLayout, classicCapOption),
                (PowerCapLayout, powerCapOption),
                (DecayRateLayout, decayRateOption),
                (GrowthRateLayout, growthRateOption),
                (SmoothLayout, smoothOption),
                (InputOffsetLayout, inputOffsetOption),
                (LimitLayout, limitOption),
                (PowerClassicLayout, powerClassicOption),
                (ExponentLayout, expOption),
                (OutputOffsetLayout, outputOffsetOption),
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
            IOption classicCapOption,
            IOption powerCapOption,
            IOption decayRateOption,
            IOption growthRateOption,
            IOption smoothOption,
            IOption inputOffsetOption,
            IOption limitOption,
            IOption powerClassicOption,
            IOption expOption,
            IOption outputOffsetOption,
            IOption midpointOption,
            IOption lutTextOption,
            IOption lutPanelOption,
            IOption lutApplyOption)
        {
            Layout(gainSwitchOption,
                classicCapOption,
                powerCapOption,
                decayRateOption,
                growthRateOption,
                smoothOption,
                inputOffsetOption,
                limitOption,
                powerClassicOption,
                expOption,
                outputOffsetOption,
                midpointOption,
                lutTextOption,
                lutPanelOption,
                lutApplyOption,
                gainSwitchOption.Top);
        }
    }
}

using grapher.Models.Options;
using System.Windows.Forms;

namespace grapher.Layouts
{
    public abstract class LayoutBase
    {
        public const string Acceleration = "Acceleration";
        public const string Scale = "Scale";
        public const string Exponent = "Exponent";
        public const string Limit = "Limit";
        public const string Midpoint = "Midpoint";
        public const string Motivity = "Motivity";
        public const string Offset = "Offset";
        public const string Cap = "Cap";
        public const string Weight = "Weight";

        public LayoutBase()
        {
            AccelLayout = new OptionLayout(false, string.Empty);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);

            LogarithmicCharts = false;
        }

        /// <summary>
        ///  Gets or sets mapping from acceleration type to identifying integer.
        ///  Must match accel_mode defined in rawaccel-settings.h
        /// </summary>
        public int Index { get; protected set; }

        public string Name { get; protected set; }

        public bool LogarithmicCharts { get; protected set; }

        protected OptionLayout AccelLayout { get; set; }

        protected OptionLayout ScaleLayout { get; set; }

        protected OptionLayout CapLayout { get; set; }

        protected OptionLayout WeightLayout { get; set; }

        protected OptionLayout OffsetLayout { get; set; }

        protected OptionLayout LimitLayout { get; set; }

        protected OptionLayout ExponentLayout { get; set; }

        protected OptionLayout MidpointLayout { get; set; }

        public void Layout(
            IOption accelOption,
            IOption scaleOption,
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limitOption,
            IOption expOption,
            IOption midpointOption,
            int top)
        {

            IOption previous = null;

            foreach (var option in new (OptionLayout, IOption)[] {
                (AccelLayout, accelOption),
                (ScaleLayout, scaleOption),
                (CapLayout, capOption),
                (WeightLayout, weightOption),
                (OffsetLayout, offsetOption),
                (LimitLayout, limitOption),
                (ExponentLayout, expOption),
                (MidpointLayout, midpointOption)})
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
            IOption accelOption,
            IOption scaleOption,
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limitOption,
            IOption expOption,
            IOption midpointOption)
        {
            Layout(accelOption,
                scaleOption,
                capOption,
                weightOption,
                offsetOption,
                limitOption,
                expOption,
                midpointOption,
                accelOption.Top);
        }
    }
}

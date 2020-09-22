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
        public const string Offset = "Offset";
        public const string Cap = "Cap";
        public const string Weight = "Weight";

        public LayoutBase()
        {
            AccelLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(false, string.Empty);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimExpLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(false, string.Empty);

            ButtonEnabled = true;
        }

        /// <summary>
        ///  Gets or sets mapping from acceleration type to identifying integer.
        ///  Must match accel_mode defined in rawaccel-settings.h
        /// </summary>
        public int Index { get; internal set; }

        public string Name { get; internal set; }

        protected bool ButtonEnabled { get; set; }

        protected OptionLayout AccelLayout { get; set; }

        protected OptionLayout CapLayout { get; set; }

        protected OptionLayout WeightLayout { get; set; }

        protected OptionLayout OffsetLayout { get; set; }

        protected OptionLayout LimExpLayout { get; set; }

        protected OptionLayout MidpointLayout { get; set; }

        public void Layout(
            IOption accelOption,
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limExpOption,
            IOption midpointOption,
            Button button,
            int top)
        {
            button.Enabled = ButtonEnabled;

            IOption previous = null;

            foreach (var option in new (OptionLayout, IOption)[] {
                (AccelLayout, accelOption),
                (CapLayout, capOption),
                (WeightLayout, weightOption),
                (OffsetLayout, offsetOption),
                (LimExpLayout, limExpOption),
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
            IOption capOption,
            IOption weightOption,
            IOption offsetOption,
            IOption limExpOption,
            IOption midpointOption,
            Button button)
        {
            Layout(accelOption,
                capOption,
                weightOption,
                offsetOption,
                limExpOption,
                midpointOption,
                button,
                accelOption.Top);
        }
    }
}

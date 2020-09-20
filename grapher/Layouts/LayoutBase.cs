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
        public const string Motility = "Motility";
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
            AccelLayout.Layout(accelOption);
            CapLayout.Layout(capOption);
            WeightLayout.Layout(weightOption);
            OffsetLayout.Layout(offsetOption);
            LimExpLayout.Layout(limExpOption);
            MidpointLayout.Layout(midpointOption);

            button.Enabled = ButtonEnabled;

            IOption previous = null;
            foreach (var option in new IOption[] { accelOption, capOption, weightOption, offsetOption, limExpOption, midpointOption})
            {
                if (option.Visible)
                {
                    if (previous != null)
                    {
                        option.SnapTo(previous);
                    }
                    else
                    {
                        option.Top = top;
                    }

                    previous = option;
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

using System.Windows.Forms;

namespace grapher.Models.Options
{
    /// <summary>
    /// This is an option type that is just a checkbox.
    /// </summary>
    public class CheckBoxOption : OptionBase
    {
        public CheckBoxOption(
            CheckBox checkBox,
            ActiveValueLabel activeValueLabel)
        {
            CheckBox = checkBox;
            ActiveValueLabel = activeValueLabel;
            Show(string.Empty);
        }

        public CheckBox CheckBox { get; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public override bool Visible
        {
            get
            {
                return CheckBox.Visible || ShouldShow;
            }
        }

        public override int Left
        {
            get
            {
                return CheckBox.Left;
            }
            set
            {
                CheckBox.Left = value;
            }
        }

        public override int Height
        {
            get
            {
                return CheckBox.Height;
            }
        }

        public override int Top
        {
            get
            {
                return CheckBox.Top;
            }
            set
            {
                CheckBox.Top = value;
                ActiveValueLabel.Top = value;
            }
        }

        public override int Width
        {
            get
            {
                return CheckBox.Width;
            }
            set
            {
                CheckBox.Width = value;
            }
        }

        /// <summary>
        /// For some reason, setting CheckBox.Show() does not result in visible not being true on GUI startup.
        /// This is inconsistent with the other options, which do.
        /// Keep this bool for allowing Visible to still be the signal for option snapping.
        /// </summary>
        private bool ShouldShow { get; set; }

        public override void AlignActiveValues()
        {
            ActiveValueLabel.Align();
        }

        public override void Hide()
        {
            CheckBox.Hide();
            ShouldShow = false;
            CheckBox.Enabled = false;
            ActiveValueLabel.Hide();
        }

        public override void Show(string Name)
        {
            CheckBox.Show();
            ShouldShow = true;
            CheckBox.Enabled = true;
            CheckBox.Name = Name;
            ActiveValueLabel.Show();
        }

        public void SetActiveValue(bool legacy)
        {
            CheckBox.Checked = !legacy;
            var activeValueString = legacy ? "Legacy" : "Gain";
            ActiveValueLabel.SetValue(activeValueString);
        }
    }
}

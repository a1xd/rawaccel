using System.Windows.Forms;

namespace grapher.Models.Options
{
    public abstract class ComboBoxOptionsBase : OptionBase
    {
        #region Constructors

        public ComboBoxOptionsBase(
            Label label,
            ComboBox dropdown,
            ActiveValueLabel activeValueLabel)
        {
            OptionsDropdown = dropdown;
            OptionsDropdown.Items.Clear();

            Label = label;
            Label.AutoSize = false;
            Label.Width = 50;

            ActiveValueLabel = activeValueLabel;
        }

        #endregion Constructors

        #region Properties

        public Label Label { get; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public ComboBox OptionsDropdown { get; }

        public override bool Visible
        {
            get
            {
                return Label.Visible || ShouldShow;
            }
        }

        public override int Left
        {
            get
            {
                return Label.Left;
            }
            set
            {
                Label.Left = value;
                OptionsDropdown.Left = Label.Left + Label.Width + Constants.OptionVerticalSeperation;
            }
        }

        public override int Height
        {
            get
            {
                return OptionsDropdown.Height;
            }
        }

        public override int Top
        {
            get
            {
                return OptionsDropdown.Top;
            }
            set
            {
                OptionsDropdown.Top = value;
                Label.Top = (OptionsDropdown.Height - Label.Height) / 2 + OptionsDropdown.Top;
                ActiveValueLabel.Top = value;
            }
        }

        public override int Width
        {
            get
            {
                return Label.Width;
            }
            set
            {
                OptionsDropdown.Width = value - Label.Width - Constants.OptionLabelBoxSeperation;
            }
        }

        protected bool ShouldShow { get; set; }

        #endregion Properties

        #region Methods

        public override void Hide()
        {
            Label.Hide();
            OptionsDropdown.Hide();
            ActiveValueLabel.Hide();
            ShouldShow = false;
        }

        public override void Show(string labelText)
        {
            Label.Show();
            
            if (!string.IsNullOrWhiteSpace(labelText))
            {
                Label.Text = labelText;
            }

            OptionsDropdown.Show();
            ActiveValueLabel.Show();
            ShouldShow = true;
        }

        public override void AlignActiveValues()
        {
            ActiveValueLabel.Align();
        }

        #endregion Methods
    }
}

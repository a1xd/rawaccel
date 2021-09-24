using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class SwitchOption : OptionBase
    {

        #region Constructors

        public SwitchOption(
            Label label,
            CheckBox firstCheckBox,
            CheckBox secondCheckBox,
            ActiveValueLabel activeValueLabel,
            int left)
        {
            Label = label;
            First = firstCheckBox;
            Second = secondCheckBox;
            ActiveValueLabel = activeValueLabel;
            Left = left;

            label.AutoSize = false;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label.Width = First.Left - left - Constants.OptionLabelBoxSeperation;
            label.Height = First.Height;

            ActiveValueLabel.Height = First.Height;

            First.CheckedChanged += OnFirstCheckedChange;
            Second.CheckedChanged += OnSecondCheckedChange;

            First.Checked = true;
            Second.Left = First.Left + First.Width + Constants.OptionLabelBoxSeperation;
            Show(string.Empty);
        }

        #endregion Constructors

        #region Properties

        public Label Label { get; }

        public CheckBox First { get; }

        public CheckBox Second { get; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public override int Height
        {
            get => Label.Height;
        }

        public override int Left
        {
            get => Label.Left;
            set
            {
                Label.Left = value;
            }
        }

        public override bool Visible
        {
            get => ShouldShow;
        }

        public override int Width
        {
            get => Second.Left + Second.Width - Label.Left;
            set
            {
            }
        }

        public override int Top
        {
            get => Label.Top;
            set
            {
                Label.Top = value;
                First.Top = value;
                Second.Top = value;
                ActiveValueLabel.Top = value;
            }
        }

        private bool ShouldShow { get; set; }

        #endregion Properties

        #region Methods

        public override void AlignActiveValues()
        {
            ActiveValueLabel.Align();
        }

        public override void Hide()
        {
            ShouldShow = false;

            Label.Hide();
            First.Hide();
            Second.Hide();
            ActiveValueLabel.Hide();
        }

        public override void Show(string name)
        {
            ShouldShow = true;

            if (!string.IsNullOrWhiteSpace(name))
            {
                Label.Text = name;
            }

            Label.Show();
            First.Show();
            Second.Show();
            ActiveValueLabel.Show();
        }

        public void SetActiveValue(bool shouldFirstBeChecked)
        {
            if (shouldFirstBeChecked)
            {
                First.Checked = true;
                ActiveValueLabel.SetValue(First.Text);
            }
            else
            {
                Second.Checked = true;
                ActiveValueLabel.SetValue(Second.Text);
            }
        }

        private void OnFirstCheckedChange(object sender, EventArgs e)
        {
            if (First.Checked)
            {
                Second.Checked = false;
            }
        }

        private void OnSecondCheckedChange(object sender, EventArgs e)
        {
            if (Second.Checked)
            {
                First.Checked = false;
            }
        }

        #endregion Methods
    }
}

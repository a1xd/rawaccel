using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class CheckBoxOption : OptionBase
    {
        public CheckBoxOption(CheckBox checkBox)
        {
            CheckBox = checkBox;
        }

        public CheckBox CheckBox { get; }

        public override bool Visible
        {
            get
            {
                return CheckBox.Visible;
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

        public override void AlignActiveValues()
        {
        }

        public override void Hide()
        {
            CheckBox.Hide();
        }

        public override void Show(string Name)
        {
            CheckBox.Show();
            CheckBox.Name = Name;
        }
    }
}

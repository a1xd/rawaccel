using grapher.Models.Options;
using System.Windows.Forms;
using grapher.Models.Theming.Controls;

namespace grapher
{
    public class Option : OptionBase
    {
        #region Constructors

        public Option(
            Field field,
            Label label,
            ActiveValueLabel activeValueLabel,
            int left)
        {
            Field = field;
            Label = label;
            ActiveValueLabel = activeValueLabel;
            Left = left;

            label.AutoSize = false;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label.Width = Field.Left - left - Constants.OptionLabelBoxSeperation;
            label.Height = Field.Height;

            ActiveValueLabel.Left = Field.Left + Field.Width;
            ActiveValueLabel.Height = Field.Height;
        }

        public Option(
            ThemeableTextBox box,
            Form containingForm,
            double defaultData,
            Label label,
            int left,
            ActiveValueLabel activeValueLabel)
            : this(
                  new Field(box, containingForm, defaultData), 
                  label,
                  activeValueLabel,
                  left)
        {
        }

        public Option(
            ThemeableTextBox box,
            Form containingForm,
            double defaultData,
            Label label,
            int left,
            ActiveValueLabel activeValueLabel,
            string startingName)
            : this(
                  box,
                  containingForm,
                  defaultData,
                  label,
                  left,
                  activeValueLabel)
        {
            SetName(startingName);
        }

        #endregion Constructors

        #region Properties

        public Field Field { get; }

        public Label Label { get; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public override int Top
        { 
            get 
            {
                return Field.Top;
            }
            set
            {
                Field.Top = value;
                Label.Top = value;
                ActiveValueLabel.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return Field.Height;
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
            }
        }
        public override int Width
        {
            get
            {
                return Field.Left + Field.Width - Label.Left;
            }
            set
            {
            }
        }

        public override bool Visible
        {
            get
            {
                return Field.Box.Enabled;
            }
        }


        #endregion Properties

        #region Methods

        public void SetName(string name)
        {
            Label.Text = name;
            //Label.Left = Convert.ToInt32((Field.Box.Left / 2.0) - (Label.Width / 2.0));   //Centered
        }

        public void SetActiveValue(double value)
        {
            ActiveValueLabel.SetValue(value);
            Field.SetNewDefault(value);
            Field.SetToDefault();
        }

        public override void Hide()
        {
            Field.Hide();
            Label.Hide();
            ActiveValueLabel.Hide();
        }

        public void Show()
        {
            Field.Show();
            Label.Show();
            ActiveValueLabel.Show();
        }

        public void UpdateActiveValue(double value)
        {
            ActiveValueLabel.SetValue(value);
        }

        public override void Show(string name)
        {
            SetName(name);

            Show();
        }

        public override void AlignActiveValues()
        {
            ActiveValueLabel.Align();
        }

        public void SetToUnavailable()
        {
            Field.SetToUnavailable();
        }

        public void SetToAvailable()
        {
            Field.SetToDefault();
        }

        #endregion Methods
    }
}

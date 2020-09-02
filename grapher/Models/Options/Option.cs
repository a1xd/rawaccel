using grapher.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class Option
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
            label.Width = Field.Left - left - Constants.OptionLabelBoxSeperation;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        }

        public Option(
            TextBox box,
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
            TextBox box,
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

        public int Top
        { 
            get 
            {
                return Field.Top;
            }
            set
            {
                Field.Top = value;
                Label.Top = value;
            }
        }

        public int Height
        {
            get
            {
                return Field.Height;
            }
        }

        public int Left
        { 
            get 
            {
                return Label.Left;
            }
            private set
            {
                Label.Left = value;
            }
        }
        public int Width
        {
            get
            {
                return Field.Left + Field.Width - Label.Left;
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
        }

        public void Hide()
        {
            Field.Box.Hide();
            Label.Hide();
            ActiveValueLabel.Hide();
        }

        public void Show()
        {
            Field.Box.Show();
            Label.Show();
            ActiveValueLabel.Show();
        }

        public void UpdateActiveValue(double value)
        {
            ActiveValueLabel.SetValue(value);
        }

        public void Show(string name)
        {
            SetName(name);

            Show();
        }
        
        public void SnapTo(Option option)
        {
            Top = option.Top + option.Height + Constants.OptionVerticalSeperation;
        }

        public void SnapTo(CapOptions option)
        {
            Top = option.Top + option.Height + Constants.OptionVerticalSeperation;
        }

        #endregion Methods
    }
}

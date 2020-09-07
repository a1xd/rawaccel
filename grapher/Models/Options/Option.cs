using grapher.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        public override bool Visible
        {
            get
            {
                return Field.Box.Visible;
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

        public override void Hide()
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

        public override void Show(string name)
        {
            SetName(name);

            Show();
        }
        
        #endregion Methods
    }
}

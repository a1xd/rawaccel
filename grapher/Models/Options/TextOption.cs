using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class TextOption : OptionBase
    {
        #region Constructors

        public TextOption(Label label)
        {
            Label = label;
            Label.AutoSize = false;
            Label.Width = 150;
            Label.Height = 100;
        }

        #endregion Constructors

        #region Properties

        private Label Label { get; }

        public override bool Visible
        { 
            get 
            { 
                return Label.Visible;
            }
        }

        public override int Top
        {
            get
            {
                return Label.Top;
            }
            set
            {
                Label.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return Label.Height;
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
                Label.Width = value;
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

        public override void Hide()
        {
            Label.Hide();
        }

        public override void Show(string Name)
        {
            Label.Show();
            Label.Text = Name;
        }

        public override void AlignActiveValues()
        {
            // Nothing to do here
        }

        #endregion Properties
    }
}

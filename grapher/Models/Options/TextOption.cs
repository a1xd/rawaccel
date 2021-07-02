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
            Label.AutoSize = true;
        }

        #endregion Constructors

        #region Properties

        private Label Label { get; }

        public override bool Visible
        { 
            get 
            { 
                return Label.Visible || ShouldShow;
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
                Label.MaximumSize = new System.Drawing.Size(value, 0);
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

        private bool ShouldShow
        {
            get; set;
        }

        public override void Hide()
        {
            Label.Hide();
            ShouldShow = false;
        }

        public override void Show(string Name)
        {
            Label.Show();
            Label.Text = Name;
            ShouldShow = true;
        }

        public override void AlignActiveValues()
        {
            // Nothing to do here
        }

        #endregion Properties
    }
}

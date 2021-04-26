using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.LUT
{
    public class LUTPanelOptions : OptionBase
    {
        public const string LUTPanelTitle = "LookupTable Points";
        public const string ApplyAsSensTitle = "Apply as sensitivity";
        public const string ApplyAsVelocityTitle = "Apply as velocity";
        public const int TitlePadding = 5;
        public const int PanelHeight = 40;

        public LUTPanelOptions(Panel panel)
        {
            Panel = panel;
            Panel.Height = PanelHeight;
            Panel.Paint += Panel_Paint;

            Title = new Label();
            Title.Text = LUTPanelTitle;
            ApplyAsSens = new CheckBox();
            ApplyAsSens.Text = ApplyAsSensTitle;
            ApplyAsVelocity = new CheckBox();
            ApplyAsVelocity.Text = ApplyAsVelocityTitle;

            Panel.Controls.Add(Title);
            Title.Left = TitlePadding;
            Title.Top = TitlePadding;
        }

        public Panel Panel
        {
            get;
        }

        public Label Title
        {
            get;
        }

        public CheckBox ApplyAsSens
        {
            get;
        }

        public CheckBox ApplyAsVelocity
        {
            get;
        }

        public override bool Visible
        {
            get
            {
                return Panel.Visible || ShouldShow;
            }
        }

        public override int Top
        {
            get
            {
                return Panel.Top;
            }
            set
            {
                Panel.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return Panel.Height;
            }
        }

        public override int Left
        {
            get
            {
                return Panel.Left;
            }
            set
            {
                Panel.Left = value;
            }
        }

        public override int Width
        {
            get
            {
                return Panel.Width;
            }
            set
            {
                Panel.Width = value;
            }
        }

        private bool ShouldShow { get; set; }

        public override void Hide()
        {
            Panel.Hide();
            ShouldShow = false;
        }

        public override void Show(string name)
        {
            Panel.Show();
            ShouldShow = true;
        }

        public override void AlignActiveValues()
        {
            // Nothing to do here.
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Color col = Color.DarkGray;
            ButtonBorderStyle bbs = ButtonBorderStyle.Dashed;
            int thickness = 2;
            ControlPaint.DrawBorder(e.Graphics, Panel.ClientRectangle, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs);
        }
    }
}

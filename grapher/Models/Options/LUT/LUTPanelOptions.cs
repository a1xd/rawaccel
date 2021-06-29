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
        public const string ApplyAsSensTitle = "Apply as sensitivity";
        public const string ApplyAsVelocityTitle = "Apply as velocity";
        public const int TitlePadding = 5;
        public const int PanelHeight = 100;

        public LUTPanelOptions(Panel activePanel)
        {
            ActivePanel = activePanel;
            ActivePanel.Height = PanelHeight;
            ActivePanel.Paint += Panel_Paint;

            ApplyAsSens = new CheckBox();
            ApplyAsSens.Text = ApplyAsSensTitle;
            ApplyAsVelocity = new CheckBox();
            ApplyAsVelocity.Text = ApplyAsVelocityTitle;
        }

        public Panel ActivePanel
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
                return ActivePanel.Visible || ShouldShow;
            }
        }

        public override int Top
        {
            get
            {
                return ActivePanel.Top;
            }
            set
            {
                ActivePanel.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return ActivePanel.Height;
            }
        }

        public override int Left
        {
            get
            {
                return ActivePanel.Left;
            }
            set
            {
                ActivePanel.Left = value;
            }
        }

        public override int Width
        {
            get
            {
                return ActivePanel.Width;
            }
            set
            {
                ActivePanel.Width = value;
            }
        }

        private bool ShouldShow { get; set; }

        public override void Hide()
        {
            ActivePanel.Hide();
            ShouldShow = false;
        }

        public override void Show(string name)
        {
            ActivePanel.Show();
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
            ControlPaint.DrawBorder(e.Graphics, ActivePanel.ClientRectangle, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs);
        }
    }
}

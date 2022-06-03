using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Theming
{
    public class StyledMenuRenderer : ToolStripProfessionalRenderer
    {
        public StyledMenuRenderer() : base(new StyledMenuColors())
        {
        }
    }

    public class StyledMenuColors : ProfessionalColorTable
    {
        public override Color ButtonCheckedHighlight => Theme.CurrentScheme.ButtonFace;

        public override Color CheckBackground => Theme.CurrentScheme.CheckBoxBackground;

        public override Color CheckPressedBackground => Theme.CurrentScheme.MenuSelected;

        public override Color CheckSelectedBackground => Theme.CurrentScheme.CheckBoxHover;

        public override Color ButtonSelectedBorder => Theme.CurrentScheme.CheckBoxBorder;

        public override Color MenuItemSelectedGradientBegin => Theme.CurrentScheme.MenuSelected;

        public override Color MenuItemSelectedGradientEnd => Theme.CurrentScheme.MenuSelected;

        public override Color MenuItemPressedGradientBegin => Theme.CurrentScheme.Control;

        public override Color MenuItemPressedGradientEnd => Theme.CurrentScheme.Control;

        public override Color MenuItemBorder => Theme.CurrentScheme.MenuSelectedBorder;

        public override Color MenuItemSelected => Theme.CurrentScheme.MenuSelected;

        public override Color ToolStripDropDownBackground => Theme.CurrentScheme.Control;

        public override Color ImageMarginGradientBegin => Theme.CurrentScheme.Control;

        public override Color ImageMarginGradientMiddle => Theme.CurrentScheme.Control;

        public override Color ImageMarginGradientEnd => Theme.CurrentScheme.Control;

        public override Color SeparatorDark => Color.Black;

        public override Color MenuBorder => Theme.CurrentScheme.ControlBorder;
    }
}
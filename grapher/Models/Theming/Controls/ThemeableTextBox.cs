using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Theming.Controls
{
    /// <summary>
    /// A TextBox which has a customizable BorderColor, making it more theme friendly.
    /// Code is copied over from this StackOverflow answer; https://stackoverflow.com/a/39420512/2721113
    /// and was then reworked a bit.
    /// </summary>
    public class ThemeableTextBox : TextBox
    {
        private const int WM_NCPAINT = 0x85;
        private const uint RDW_INVALIDATE = 0x1;
        private const uint RDW_IUPDATENOW = 0x100;
        private const uint RDW_FRAME = 0x400;

        private Color _borderColor = Color.Gray;

        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                Invalidate();
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
            }
        }

        private const int BorderWidth = 1;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg != WM_NCPAINT || BorderColor == Color.Transparent || BorderStyle != BorderStyle.Fixed3D)
            {
                base.WndProc(ref message);
                return;
            }
            
            var hdc = GetWindowDC(Handle);
            using (var graphics = Graphics.FromHdcInternal(hdc))
            {
                using (var pen = new Pen(BorderColor))
                {
                    var border = new Rectangle(0, 0, Width - BorderWidth, Height - BorderWidth);
                    graphics.DrawRectangle(pen, border);
                }
                using (var pen = new Pen(BackColor))
                {
                    var backGround = new Rectangle(1, 1, Width - 3, Height - 3);
                    graphics.DrawRectangle(pen, backGround);
                }
            }
            
            ReleaseDC(Handle, hdc);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);
    }
}
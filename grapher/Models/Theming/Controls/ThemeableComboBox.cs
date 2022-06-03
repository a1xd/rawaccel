using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace grapher.Models.Theming.Controls
{
    /// <summary>
    /// A ComboBox which has properties for the Border and Button color, making it look far better when applying a theme.
    /// Code is largely copied over from this StackOverflow Answer; https://stackoverflow.com/a/65976649/2721113
    /// and was then reworked a bit.
    /// </summary>
    public class ThemeableComboBox : ComboBox
    {
        private const int WM_PAINT = 0xF;

        private readonly int _buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;

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
            }
        }

        private Color _buttonColor = Color.LightGray;

        [DefaultValue(typeof(Color), "LightGray")]
        public Color ButtonColor
        {
            get => _buttonColor;
            set
            {
                if (_buttonColor == value) return;
                _buttonColor = value;
                Invalidate();
            }
        }

        private const int BorderWidth = 1;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg != WM_PAINT || DropDownStyle == ComboBoxStyle.Simple)
            {
                base.WndProc(ref message);
                return;
            }

            var clientRect = ClientRectangle;
            var dropDownButtonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
            var outerBorder = new Rectangle(clientRect.Location,
                new Size(clientRect.Width - BorderWidth, clientRect.Height - BorderWidth)
            );
            var innerBorder = new Rectangle(outerBorder.X + BorderWidth, outerBorder.Y + BorderWidth,
                outerBorder.Width - dropDownButtonWidth - (BorderWidth * 2),
                outerBorder.Height - (BorderWidth * 2)
            );
            var innerInnerBorder = new Rectangle(innerBorder.X + BorderWidth, innerBorder.Y + BorderWidth,
                innerBorder.Width - (BorderWidth * 2), innerBorder.Height - (BorderWidth * 2)
            );
            var dropDownRect = new Rectangle(innerBorder.Right + BorderWidth, innerBorder.Y,
                dropDownButtonWidth, innerBorder.Height + BorderWidth
            );
            if (RightToLeft == RightToLeft.Yes)
            {
                innerBorder.X = clientRect.Width - innerBorder.Right;
                innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
                dropDownRect.X = clientRect.Width - dropDownRect.Right;
                dropDownRect.Width += BorderWidth;
            }

            var innerBorderColor = Enabled ? BackColor : SystemColors.Control;
            var outerBorderColor = Enabled ? BorderColor : SystemColors.ControlDark;
            var buttonColor = Enabled ? ButtonColor : SystemColors.Control;
            var middle = new Point(dropDownRect.Left + dropDownRect.Width / 2,
                dropDownRect.Top + dropDownRect.Height / 2);
            var arrow = new[]
            {
                new Point(middle.X - 3, middle.Y - 2),
                new Point(middle.X + 4, middle.Y - 2),
                new Point(middle.X, middle.Y + 2)
            };
            var ps = new PAINTSTRUCT();
            var shouldEndPaint = false;
            IntPtr dc;
            if (message.WParam == IntPtr.Zero)
            {
                dc = BeginPaint(Handle, ref ps);
                message.WParam = dc;
                shouldEndPaint = true;
            }
            else
            {
                dc = message.WParam;
            }

            var rgn = CreateRectRgn(innerInnerBorder.Left, innerInnerBorder.Top, innerInnerBorder.Right, innerInnerBorder.Bottom);
            SelectClipRgn(dc, rgn);

            DefWndProc(ref message);
            DeleteObject(rgn);
            rgn = CreateRectRgn(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
            SelectClipRgn(dc, rgn);
            using (var g = Graphics.FromHdc(dc))
            {
                using (var b = new SolidBrush(buttonColor))
                {
                    g.FillRectangle(b, dropDownRect);
                }

                using (var b = new SolidBrush(outerBorderColor))
                {
                    g.FillPolygon(b, arrow);
                }

                using (var p = new Pen(innerBorderColor))
                {
                    g.DrawRectangle(p, innerBorder);
                    g.DrawRectangle(p, innerInnerBorder);
                }

                using (var p = new Pen(outerBorderColor))
                {
                    g.DrawRectangle(p, outerBorder);
                }
            }

            if (shouldEndPaint)
            {
                EndPaint(Handle, ref ps);
            }
            
            DeleteObject(rgn);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int L, T, R, B;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd,
            [In, Out] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("user32.dll")]
        public static extern int GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool fErase);

        public enum RegionFlags
        {
            ERROR = 0,
            NULLREGION = 1,
            SIMPLEREGION = 2,
            COMPLEXREGION = 3,
        }

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
    }
}
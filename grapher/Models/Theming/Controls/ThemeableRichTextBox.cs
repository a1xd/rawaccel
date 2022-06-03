using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json.Serialization;

namespace grapher.Models.Theming.Controls
{
    public class ThemeableRichTextBox : RichTextBox
    {
        private const int WM_NCPAINT = 0x85;

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

        private Color _readOnlyBackColor = Color.DarkSlateGray;

        [DefaultValue(typeof(Color), "DarkSlateGray")]
        public Color ReadOnlyBackColor
        {
            get => _readOnlyBackColor;
            set
            {
                if (_readOnlyBackColor == value) return;
                _readOnlyBackColor = value;
                Invalidate();
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
                var borderRect = new Rectangle(0, 0, ClientSize.Width + BorderWidth * 2, ClientSize.Height + BorderWidth * 2);
                var backGroundRect = new Rectangle(1, 1, ClientSize.Width, ClientSize.Height);

                using (var pen = new Pen(BorderColor))
                {
                    graphics.DrawRectangle(pen, borderRect);
                }

                if (!ReadOnly)
                {
                    using (var pen = new Pen(BackColor))
                    {
                        graphics.DrawRectangle(pen, backGroundRect);
                    }
                }
                else
                {
                    BackColor = ReadOnlyBackColor;
                    using (var pen = new Pen(ReadOnlyBackColor))
                    {
                        graphics.DrawRectangle(pen, backGroundRect);
                    }
                }
            }

            ReleaseDC(Handle, hdc);
        }


        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class FieldXY
    {
        public const int DefaultSeparation = 4;

        public const string ShortenedFormatString = "0.###";

        public FieldXY(TextBox xBox, TextBox yBox, CheckBox lockCheckBox, Form containingForm, double defaultData)
        {
            XField = new Field(xBox, containingForm, defaultData);
            YField = new Field(yBox, containingForm, defaultData);
            YField.FormatString = ShortenedFormatString;
            LockCheckBox = lockCheckBox;
            LockCheckBox.CheckedChanged += new System.EventHandler(CheckChanged);

            XField.Box.Width = (YField.Box.Left + YField.Box.Width - XField.Box.Left - DefaultSeparation) / 2;
            YField.Box.Width = XField.Box.Width;

            DefaultWidthX = XField.Box.Width;
            DefaultWidthY = YField.Box.Width;

            YField.Box.Left = XField.Box.Left + XField.Box.Width + DefaultSeparation;

            CombinedWidth = DefaultWidthX + DefaultWidthY + YField.Box.Left - (XField.Box.Left + DefaultWidthX);
            SetCombined();
        }
        public double X
        {
            get => XField.Data;
        }

        public double Y
        {
            get
            {
                if (Combined)
                {
                    return X;
                }
                else
                {
                    return YField.Data;
                }
            }
        }

        public CheckBox LockCheckBox { get; }

        public Field XField { get; }

        public Field YField { get; }

        private bool Combined { get; set; }

        private int DefaultWidthX { get; }

        private int DefaultWidthY { get; }

        private int CombinedWidth { get; }

        private void CheckChanged(object sender, EventArgs e)
        {
            if (LockCheckBox.CheckState == CheckState.Checked)
            {
                SetCombined();
            }
            else
            {
                SetSeparate();
            }
        }

        public void SetCombined()
        {
            Combined = true;
            YField.SetToUnavailable();
            YField.Box.Hide();
            XField.Box.Width = CombinedWidth;
            XField.FormatString = Field.DefaultFormatString;
        }

        public void SetSeparate()
        {
            Combined = false;

            XField.Box.Width = DefaultWidthX;
            YField.Box.Width = DefaultWidthY;

            XField.FormatString = ShortenedFormatString;

            if (XField.State == Field.FieldState.Default)
            {
                YField.SetToDefault();
            }
            else
            {
                YField.SetToEntered(XField.Data);
            }

            if (XField.Box.Visible)
            {
                YField.Box.Show();
            }
        }

        public void Show()
        {
            XField.Box.Show();

            if (!Combined)
            {
                YField.Box.Show();
            }
        }

        public void Hide()
        {
            XField.Box.Hide();
            YField.Box.Hide();
        }
    }
}

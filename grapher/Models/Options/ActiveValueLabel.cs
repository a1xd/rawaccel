using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class ActiveValueLabel
    {
        public const string DefaultFormatString = "0.######";

        public ActiveValueLabel(Label valueLabel, Label centeringLabel)
        {
            ValueLabel = valueLabel;
            ValueLabel.ForeColor = Color.DarkGray;
            ValueLabel.Left = centeringLabel.Left;
            ValueLabel.Width = centeringLabel.Width;
            ValueLabel.AutoSize = false;
            ValueLabel.TextAlign = ContentAlignment.MiddleCenter;
        }

        public Label ValueLabel { get; }

        private int Left { get; }

        private int Width { get; }

        public void Hide()
        {
            ValueLabel.Hide();
        }

        public void Show()
        {
            ValueLabel.Show();
        }

        public void SetValue(double value)
        {
            ValueLabel.Text = value.ToString(DefaultFormatString);
        }
    }
}

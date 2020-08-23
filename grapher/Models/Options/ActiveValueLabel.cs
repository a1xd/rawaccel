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
        public static readonly Color ActiveValueFontColor = Color.FromArgb(255, 65, 65, 65);

        private string _prefix;
        private string _value;

        public ActiveValueLabel(Label valueLabel, Label centeringLabel)
        {
            ValueLabel = valueLabel;
            ValueLabel.ForeColor = ActiveValueFontColor;
            Left = centeringLabel.Left;
            Width = centeringLabel.Width;
            ValueLabel.AutoSize = false;
            ValueLabel.TextAlign = ContentAlignment.MiddleCenter;

            FormatString = DefaultFormatString;
            Prefix = string.Empty;
        }

        public Label ValueLabel { get; }

        public string FormatString { get; set; }

        public string Prefix 
        { 
            get { return _prefix; }
            set 
            {
                _prefix = value;
                RefreshText();
            } 
        }

        private string Value
        { 
            get { return _value; }
            set 
            {
                _value = value;
                RefreshText();
            } 
        }

        public int Left
        {
            get 
            {
                return ValueLabel.Left;
            }

            set
            {
                ValueLabel.Left = value;
            }
        }

        public int Width
        {
            get 
            {
                return ValueLabel.Width;
            }

            set
            {
                ValueLabel.Width = value;
            }
        }

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
            SetValue(value.ToString(FormatString));
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public void RefreshText()
        {
            ValueLabel.Text = string.IsNullOrWhiteSpace(Prefix) ? Value: $"{Prefix}: {Value}";
        }
    }
}

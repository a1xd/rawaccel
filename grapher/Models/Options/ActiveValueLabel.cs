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
        #region Constants


        #endregion Constants

        #region Fields

        private string _prefix;
        private string _value;

        #endregion Fields

        #region Constructors

        public ActiveValueLabel(Label valueLabel, Label centeringLabel)
        {
            ValueLabel = valueLabel;
            ValueLabel.ForeColor = Constants.ActiveValueFontColor;
            Left = centeringLabel.Left;
            Width = centeringLabel.Width;
            ValueLabel.AutoSize = false;
            ValueLabel.TextAlign = ContentAlignment.MiddleCenter;

            FormatString = Constants.DefaultActiveValueFormatString;
            Prefix = string.Empty;
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}

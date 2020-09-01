using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Options
{
    public class ActiveValueLabelXY
    {
        #region Constants

        public const int ActiveLabelXYSeparation = 2;
        public const string ShortenedFormatString = "0.###";

        #endregion Constants

        #region Constructors

        public ActiveValueLabelXY(
            ActiveValueLabel x,
            ActiveValueLabel y)
        {
            X = x;
            Y = y;

            FullWidth = x.Width;
            ShortenedWidth = (FullWidth - ActiveLabelXYSeparation) / 2;

            Y.Left = X.Left + ShortenedWidth + ActiveLabelXYSeparation;
            Y.Width = ShortenedWidth;
            Y.FormatString = ShortenedFormatString;

            Combined = false;
            SetCombined();
        }

        #endregion Constructors

        #region Properties

        public ActiveValueLabel X { get; }

        public ActiveValueLabel Y { get; }

        public bool Combined { get; private set; }

        private int FullWidth { get; }

        private int ShortenedWidth { get; }

        #endregion Properties

        #region Methods

        public void SetValues(double x, double y)
        {
            X.SetValue(x);
            Y.SetValue(y);

            if (x == y)
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
            if (!Combined)
            {
                X.FormatString = ActiveValueLabel.DefaultFormatString;
                X.Width = FullWidth;
                X.Prefix = string.Empty;
                Y.Hide();
            }

            Combined = true;
        }

        public void SetSeparate()
        {
            if (Combined)
            {
                X.FormatString = ShortenedFormatString;
                X.Width = ShortenedWidth;
                X.Prefix = "X";
                Y.Prefix = "Y";
                Y.Show();
            }

            Combined = false;
        }

        #region Methods
    }
}

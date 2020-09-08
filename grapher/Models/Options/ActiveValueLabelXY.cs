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


        #endregion Constants

        #region Constructors

        public ActiveValueLabelXY(
            ActiveValueLabel x,
            ActiveValueLabel y)
        {
            X = x;
            Y = y;

            FullWidth = x.Width;
            ShortenedWidth = (FullWidth - Constants.ActiveLabelXYSeparation) / 2;

            Y.Left = X.Left + ShortenedWidth + Constants.ActiveLabelXYSeparation;
            Y.Width = ShortenedWidth;
            Y.FormatString = Constants.ShortenedFormatString;

            Combined = false;
            SetCombined();
        }

        #endregion Constructors

        #region Properties

        public ActiveValueLabel X { get; }

        public ActiveValueLabel Y { get; }

        public bool Combined { get; private set; }

        private int FullWidth { get; set; }

        private int ShortenedWidth { get; set; }

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
                X.FormatString = Constants.DefaultActiveValueFormatString;
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
                X.FormatString = Constants.ShortenedFormatString;
                X.Width = ShortenedWidth;
                X.Prefix = "X";
                Y.Prefix = "Y";
                Y.Show();
            }

            Combined = false;
        }

        public void AlignActiveValues(int width)
        {
            Align(width);

            if (Combined)
            {
                X.Width = FullWidth;
            }
            else
            {
                X.Width = ShortenedWidth;
            }
        }

        private void Align (int width)
        {
            FullWidth = width;
            ShortenedWidth = (FullWidth - Constants.ActiveLabelXYSeparation) / 2;

            Y.Left = X.Left + ShortenedWidth + Constants.ActiveLabelXYSeparation;
            Y.Width = ShortenedWidth;
        }

        #endregion Methods
    }
}

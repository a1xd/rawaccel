using System.Drawing;

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

            Align(x.Width);
            Y.Width = ShortenedWidth;
            Y.FormatString = Constants.ShortenedFormatString;

            X.ValueLabel.Margin = new System.Windows.Forms.Padding(0);
            Y.ValueLabel.Margin = new System.Windows.Forms.Padding(0);
            X.ValueLabel.UseCompatibleTextRendering = true;
            Y.ValueLabel.UseCompatibleTextRendering = true;
            DefaultFont = X.ValueLabel.Font;
            ShortenedFont = new Font(DefaultFont.FontFamily, (float)7.75);
            Y.ValueLabel.Font = ShortenedFont;

            Combined = false;
            SetCombined();
        }

        #endregion Constructors

        #region Properties

        public ActiveValueLabel X { get; }

        public ActiveValueLabel Y { get; }

        public Font DefaultFont { get; }

        public Font ShortenedFont { get; }

        public bool Combined { get; private set; }

        public int Left
        {
            get
            {
                return X.Left;
            }
            set
            {
                X.Left = value;
                SetYLeft();
            }
        }

        public int Height
        {
            get
            {
                return X.Height;
            }
            set
            {
                X.Height = value;
                Y.Height = value;
            }
        }

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
                X.ValueLabel.Font = DefaultFont;
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
                X.ValueLabel.Font = ShortenedFont;
                Y.Prefix = "Y";
                Y.Show();
            }

            Combined = false;
        }

        public void AlignActiveValues()
        {
            Align(X.CenteringLabel.Width);

            if (Combined)
            {
                X.Width = FullWidth;
            }
            else
            {
                X.Width = ShortenedWidth;
            }
        }

        public void Hide()
        {
            X.Hide();
            Y.Hide();
        }

        public void Show()
        {
            X.Show();
            Y.Show();
        }

        private void Align (int width)
        {
            FullWidth = width;
            // ShortenedWidth = FullWidth / 2;
            ShortenedWidth = FullWidth;

            SetYLeft();
            Y.Width = ShortenedWidth;
        }

        private void SetYLeft()
        {
            Y.Left = X.Left + ShortenedWidth + Constants.ActiveLabelXYSeparation;
        }

        #endregion Methods
    }
}

using System;

namespace grapher.Models.Mouse
{
    public class MouseData
    {
        #region Constructors

        public MouseData()
        {
            X = 0;
            Y = 0;
        }

        #endregion Constructors

        #region Properties

        private int X { get; set; }
        private int Y { get; set; }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion Properties

        #region Methods

        public void Get(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        #endregion Methods
    }
}

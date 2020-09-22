using System;

namespace grapher.Models.Mouse
{
    public class MouseData
    {
        #region Constructors

        public MouseData()
        {
            Lock = new Object();
            X = 0;
            Y = 0;
        }

        #endregion Constructors

        #region Properties

        public Object Lock { get; }

        private int X { get; set; }
        private int Y { get; set; }

        public void Set(int x, int y)
        {
            lock (Lock)
            {
                X = x;
                Y = y;
            }
        }

        #endregion Properties

        #region Methods

        public void Get(out int x, out int y)
        {
            lock (Lock)
            {
                x = X;
                y = Y;
            }
        }

        #endregion Methods
    }
}

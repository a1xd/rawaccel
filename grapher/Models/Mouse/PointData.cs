using System;

namespace grapher.Models.Mouse
{
    public class PointData
    {
        #region Constructors

        public PointData()
        {
            Lock = new Object();
            X = new double[] { 0 };
            Y = new double[] { 0 };
        }

        #endregion Constructors

        #region Properties

        public Object Lock { get; }

        private double[] X { get; set; }
        private double[] Y { get; set; }

        public void Set(double x, double y)
        {
            lock(Lock)
            {
                X[0] = x;
                Y[0] = y;
            }
        }

        #endregion Properties

        #region Methods

        public void Get(out double[] x, out double[] y)
        {
            lock(Lock)
            {
                x = X;
                y = Y;
            }
        }

        #endregion Methods
    }
}

using System;

namespace grapher.Models.Mouse
{
    public class PointData
    {
        #region Constructors

        public PointData()
        {
            X = new double[] { 0.01 };
            Y = new double[] { 0.01 };
        }

        #endregion Constructors

        #region Properties

        private double[] X { get; set; }
        private double[] Y { get; set; }

        public void Set(double x, double y)
        {
            X[0] = x;
            Y[0] = y;
        }

        #endregion Properties

        #region Methods

        public void Get(out double[] x, out double[] y)
        {
                x = X;
                y = Y;
        }

        #endregion Methods
    }
}

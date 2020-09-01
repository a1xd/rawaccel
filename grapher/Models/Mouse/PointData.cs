﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Mouse
{
    public class PointData
    {
        public PointData()
        {
            Lock = new Object();
            X = new double[] { 0 };
            Y = new double[] { 0 };
        }

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

        public void Get(out double[] x, out double[] y)
        {
            lock(Lock)
            {
                x = X;
                y = Y;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var managedAccel = new ManagedAccel(6, 0, 0.025, 1.01, 0);
            var orderedPoints = new SortedDictionary<double, double>();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    var output = managedAccel.Accelerate(i, j, 1, 6);

                    var inMagnitude = Magnitude(i,j);
                    var outMagnitude = Magnitude(output.Item1, output.Item2);
                    var ratio = inMagnitude > 0 ? outMagnitude / inMagnitude : 0;

                    if (!orderedPoints.ContainsKey(inMagnitude))
                    {
                        orderedPoints.Add(inMagnitude, ratio);
                    }
                }
            }

            var series = this.chart1.Series.FirstOrDefault();
            series.Points.Clear();

            foreach (var point in orderedPoints)
            {
                series.Points.AddXY(point.Key, point.Value);
            }
        }

        public static double Magnitude(int x, int y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static double Magnitude(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}

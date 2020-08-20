using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Options
{
    public class ActiveValueLabelXY
    {
        public const int ActiveLabelXYSeparation = 4;

        public ActiveValueLabelXY(
            ActiveValueLabel x,
            ActiveValueLabel y)
        {
            X = x;
            Y = y;
            Combined = false;
            SetCombined();
        }

        public ActiveValueLabel X { get; }

        public ActiveValueLabel Y { get; }

        public bool Combined { get; private set; }

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
                Y.Hide();
            }

            Combined = true;
        }

        public void SetSeparate()
        {
            if (Combined)
            {
                Y.Show();
            }

            Combined = false;
        }
    }
}

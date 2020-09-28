using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class MotivityLayout : LayoutBase
    {
         public MotivityLayout()
            : base()
        {
            Name = "Motivity";
            Index = (int)AccelMode.motivity;
            LogarithmicCharts = true;

            AccelLayout = new OptionLayout(true, Acceleration);
            ScaleLayout = new OptionLayout(false, string.Empty);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(true, Motivity);
            ExponentLayout = new OptionLayout(false, string.Empty);
            MidpointLayout = new OptionLayout(true, Midpoint);
        }
    }
}

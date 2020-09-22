using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class ExperimentOneLayout : LayoutBase
    {
         public ExperimentOneLayout()
            : base()
        {
            Name = "Experiment 1";
            Index = (int)AccelMode.experimentone;

            AccelLayout = new OptionLayout(true, Acceleration);
            CapLayout = new OptionLayout(false, string.Empty);
            WeightLayout = new OptionLayout(true, Weight);
            OffsetLayout = new OptionLayout(false, string.Empty);
            LimExpLayout = new OptionLayout(true, Motility);
            MidpointLayout = new OptionLayout(true, Midpoint);
        }
    }
}

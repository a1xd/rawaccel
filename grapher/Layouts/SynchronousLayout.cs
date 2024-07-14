using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public class SynchronousLayout : LayoutBase
    {
         public SynchronousLayout()
            : base()
        {
            Name = "Synchronous";
            Mode = AccelMode.synchronous;
            LogarithmicCharts = true;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GammaLayout = new OptionLayout(true, Gamma);
            SmoothLayout = new OptionLayout(true, Smooth);
            InputOffsetLayout = new OptionLayout(false, string.Empty);
            LimitLayout = new OptionLayout(true, Motivity);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            OutputOffsetLayout = new OptionLayout(false, string.Empty);
            SyncSpeedLayout = new OptionLayout(true, SyncSpeed);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
            InputJumpLayout = new OptionLayout(false, string.Empty);
            OutputJumpLayout = new OptionLayout(false, string.Empty);
        }
    }
}

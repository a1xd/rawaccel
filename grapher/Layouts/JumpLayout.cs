﻿using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class JumpLayout : LayoutBase
    {
        public JumpLayout()
            : base()
        {
            Name = "Jump";
            Mode = AccelMode.jump;
            LogarithmicCharts = false;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(false, string.Empty);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GammaLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(true, Smooth);
            InputOffsetLayout = new OptionLayout(false, InputOffset);
            LimitLayout = new OptionLayout(false, Limit);
            PowerClassicLayout = new OptionLayout(false, string.Empty);
            ExponentLayout = new OptionLayout(false, string.Empty);
            OutputOffsetLayout = new OptionLayout(false, string.Empty);
            SyncSpeedLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
            InputJumpLayout = new OptionLayout(true, Input);
            OutputJumpLayout = new OptionLayout(true, Output);
        }
    }
}

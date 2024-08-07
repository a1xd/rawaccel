﻿using grapher.Models.Serialized;

namespace grapher.Layouts
{
    public class ClassicLayout : LayoutBase
    {
        public ClassicLayout()
            : base()
        {
            Name = "Classic";
            Mode = AccelMode.classic;

            GainSwitchOptionLayout = new OptionLayout(true, Gain);
            ClassicCapLayout = new OptionLayout(true, CapType);
            PowerCapLayout = new OptionLayout(false, string.Empty);
            DecayRateLayout = new OptionLayout(false, string.Empty);
            GammaLayout = new OptionLayout(false, string.Empty);
            SmoothLayout = new OptionLayout(false, string.Empty);
            InputOffsetLayout = new OptionLayout(true, InputOffset);
            LimitLayout = new OptionLayout(false, string.Empty);
            PowerClassicLayout = new OptionLayout(true, PowerClassic);
            ExponentLayout = new OptionLayout(false, string.Empty);
            OutputOffsetLayout = new OptionLayout(false, string.Empty);
            SyncSpeedLayout = new OptionLayout(false, string.Empty);
            LutTextLayout = new OptionLayout(false, string.Empty);
            LutPanelLayout = new OptionLayout(false, string.Empty);
            LutApplyOptionsLayout = new OptionLayout(false, string.Empty);
            InputJumpLayout = new OptionLayout(false, string.Empty);
            OutputJumpLayout = new OptionLayout(false, string.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Constants
{
    public static class AccelGUIConstants
    {
        #region Constants

        /// <summary> Vertical separation between charts, in pixels. </summary>
        public const int ChartSeparationVertical = 10;

        /// <summary> Needed to show full contents in form. Unsure why. </summary>
        public const int FormHeightPadding = 35;

        /// <summary> DPI by which charts are scaled if none is set by user. </summary>
        public const int DefaultDPI = 1200;

        /// <summary> Poll rate by which charts are scaled if none is set by user. </summary>
        public const int DefaultPollRate = 1000;

        /// <summary> Resolution of chart calulation. </summary>
        public const int Resolution = 100;

        /// <summary> Multiplied by DPI over poll rate to find rough max expected velocity. </summary>
        public const double MaxMultiplier = 85;

        /// <summary> Ratio of max (X, Y) used in "by component" calulations to those used in "whole vector" calculations. </summary>
        public const double XYToCombinedRatio = 1.4;

        #endregion Constants
    }
}

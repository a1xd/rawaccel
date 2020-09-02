using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher
{
    public static class Constants
    {
        #region Constants

        /// <summary> Vertical separation between charts, in pixels. </summary>
        public const int ChartSeparationVertical = 10;

        /// <summary> Default name of settings file. </summary>
        public const string DefaultSettingsFileName = @"settings.json";

        /// <summary> Needed to show full contents in form. Unsure why. </summary>
        public const int FormHeightPadding = 35;

        /// <summary> Format string for gain cap active value label. </summary>
        public const string GainCapFormatString = "0.##";

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

        /// <summary> Separation between X and Y active value labels, in pixels. </summary>
        public const int ActiveLabelXYSeparation = 2;

        /// <summary> Format string for shortened x and y textboxes. </summary>
        public const string ShortenedFormatString = "0.###";

        /// <summary> Format string for default active value labels. </summary>
        public const string DefaultActiveValueFormatString = "0.######";

        /// <summary> Format string for default textboxes. </summary>
        public const string DefaultFieldFormatString = "0.#########";

        /// <summary> Possible options to display in a layout. </summary>
        public const int PossibleOptionsCount = 4;

        /// <summary> Possible x/y options to display in a layout. </summary>
        public const int PossibleOptionsXYCount = 0;

        /// <summary> Horizontal separation between charts, in pixels. </summary>
        public const int ChartSeparationHorizontal = 10;

        /// <summary> Default horizontal separation between x and y fields, in pixels. </summary>
        public const int DefaultFieldSeparation = 4;

        /// <summary> Default horizontal separation between an option's label and box, in pixels. </summary>
        public const int OptionLabelBoxSeperation = 10;

        /// <summary> Default horizontal separation between an option's label and box, in pixels. </summary>
        public const int OptionVerticalSeperation = 4;

        /// <summary> Format string for shortened x and y fields. </summary>
        public const string ShortenedFieldFormatString = "0.###";

        /// <summary> Format string for shortened x and y dropdowns. </summary>
        public const string AccelDropDownDefaultFullText = "Acceleration Type";

        /// <summary> Format string for default dropdowns. </summary>
        public const string AccelDropDownDefaultShortText = "Accel Type";
        #endregion Constants

        #region ReadOnly

        /// <summary> Color of font in active value labels. </summary>
        public static readonly Color ActiveValueFontColor = Color.FromArgb(255, 65, 65, 65);

        #endregion ReadOnly
    }
}

using System.Drawing;
using System.Xml.Serialization;

namespace grapher.Models.Theming
{
    public class ColorScheme
    {
        /// <summary>
        /// Default Chart Background Color.
        /// </summary>
        [XmlAttribute]
        public Color ChartBackground { get; internal set; } = Color.White;

        /// <summary>
        /// Default Chart Foreground Color.
        /// </summary>
        [XmlAttribute]
        public Color ChartForeground { get; internal set; } = Color.Black;

        /// <summary>
        /// Used on things like sliders and primary chart lines.
        /// </summary>
        [XmlAttribute]
        public Color Primary { get; internal set; }

        // Alternative for <see cref="Primary"/>.
        [XmlAttribute] public Color Secondary { get; internal set; }

        /// <summary>
        /// Mouse movement color which is shown on the charts.
        /// </summary>
        [XmlAttribute]
        public Color MouseMovement { get; internal set; } = Color.FromArgb(192, 0, 0);

        [XmlAttribute] public Color Field { get; internal set; }
        [XmlAttribute] public Color OnField { get; internal set; }
        [XmlAttribute] public Color EditedField { get; internal set; }
        [XmlAttribute] public Color OnEditedField { get; internal set; }
        [XmlAttribute] public Color Control { get; internal set; }
        [XmlAttribute] public Color OnControl { get; internal set; }
        [XmlAttribute] public Color DisabledControl { get; internal set; }
        [XmlAttribute] public Color OnDisabledControl { get; internal set; }
        [XmlAttribute] public Color Background { get; internal set; }
        [XmlAttribute] public Color OnBackground { get; internal set; }
        [XmlAttribute] public Color Surface { get; internal set; }
        [XmlAttribute] public Color SurfaceVariant { get; internal set; }


        public static ColorScheme LightTheme = new ColorScheme
        {
            Primary = Color.Blue,
            Secondary = Color.Orange,
            Field = SystemColors.Window,
            OnField = Color.FromArgb(64, 64, 64),
            EditedField = Color.AntiqueWhite,
            OnEditedField = Color.DarkGray,
            Control = SystemColors.Control,
            OnControl = Color.FromArgb(64, 64, 64),
            DisabledControl = Color.LightGray,
            OnDisabledControl = Color.LightGray,
            Background = SystemColors.Window,
            OnBackground = Color.FromArgb(64, 64, 64),
            Surface = SystemColors.Control,
            SurfaceVariant = SystemColors.ControlLight
        };

        public static ColorScheme LightStreamerTheme = new ColorScheme
        {
            Primary = Color.Blue,
            Secondary = Color.Orange,
            ChartBackground = Color.Green,
            ChartForeground = Color.White,
            Field = SystemColors.Window,
            OnField = Color.FromArgb(64, 64, 64),
            EditedField = Color.AntiqueWhite,
            OnEditedField = Color.DarkGray,
            Control = SystemColors.Control,
            OnControl = Color.FromArgb(64, 64, 64),
            DisabledControl = Color.LightGray,
            OnDisabledControl = Color.LightGray,
            Background = SystemColors.Window,
            OnBackground = Color.FromArgb(64, 64, 64),
            Surface = SystemColors.Control,
            SurfaceVariant = SystemColors.ControlLight
        };

        public static ColorScheme DarkTheme = new ColorScheme
        {
            Primary = Color.FromArgb(147, 110, 227),
            Secondary = Color.FromArgb(223, 185, 136),
            MouseMovement = Color.FromArgb(237, 103, 103),
            ChartBackground = Color.FromArgb(48, 48, 48),
            ChartForeground = Color.FromArgb(234, 234, 234),
            Field = Color.FromArgb(64, 64, 64),
            OnField = Color.FromArgb(234, 234, 234),
            EditedField = Color.FromArgb(116, 101, 31),
            OnEditedField = Color.FromArgb(232, 227, 222),
            Control = Color.FromArgb(81, 81, 81),
            OnControl = Color.FromArgb(234, 234, 234),
            DisabledControl = Color.LightGray,
            OnDisabledControl = Color.LightGray,
            Background = Color.FromArgb(8, 8, 8),
            OnBackground = Color.FromArgb(234, 234, 234),
            Surface = Color.FromArgb(48, 48, 48),
            SurfaceVariant = Color.FromArgb(35, 35, 35)
        };

        public static ColorScheme DarkStreamerTheme = new ColorScheme
        {
            Primary = Color.FromArgb(147, 110, 227),
            Secondary = Color.FromArgb(223, 185, 136),
            ChartBackground = Color.Green,
            ChartForeground = Color.White,
            MouseMovement = Color.FromArgb(237, 103, 103),
            Field = Color.FromArgb(64, 64, 64),
            OnField = Color.FromArgb(234, 234, 234),
            EditedField = Color.FromArgb(116, 101, 31),
            OnEditedField = Color.FromArgb(232, 227, 222),
            Control = Color.FromArgb(81, 81, 81),
            OnControl = Color.FromArgb(234, 234, 234),
            DisabledControl = Color.LightGray,
            OnDisabledControl = Color.LightGray,
            Background = Color.FromArgb(8, 8, 8),
            OnBackground = Color.FromArgb(234, 234, 234),
            Surface = Color.FromArgb(48, 48, 48),
            SurfaceVariant = Color.FromArgb(35, 35, 35)
        };

    }
}

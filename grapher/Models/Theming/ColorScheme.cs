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
        public Color ChartBackground { get; internal set; }

        /// <summary>
        /// Default Chart Foreground Color.
        /// </summary>
        [XmlAttribute]
        public Color ChartForeground { get; internal set; }

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
        public Color MouseMovement { get; internal set; }

        [XmlAttribute] public Color Field { get; internal set; }
        [XmlAttribute] public Color OnField { get; internal set; }
        [XmlAttribute] public Color OnFocusedField { get; internal set; }
        [XmlAttribute] public Color EditedField { get; internal set; }
        [XmlAttribute] public Color OnEditedField { get; internal set; }
        [XmlAttribute] public Color ButtonFace { get; internal set; }
        [XmlAttribute] public Color ButtonBorder { get; internal set; }
        [XmlAttribute] public Color Control { get; internal set; }
        [XmlAttribute] public Color ControlBorder { get; internal set; }
        [XmlAttribute] public Color OnControl { get; internal set; }
        [XmlAttribute] public Color DisabledControl { get; internal set; }
        [XmlAttribute] public Color OnDisabledControl { get; internal set; }
        [XmlAttribute] public Color Background { get; internal set; }
        [XmlAttribute] public Color OnBackground { get; internal set; }
        [XmlAttribute] public Color Surface { get; internal set; }
        [XmlAttribute] public Color SurfaceVariant { get; internal set; }
        [XmlAttribute] public Color MenuSelectedBorder { get; internal set; }
        [XmlAttribute] public Color MenuSelected { get; internal set; }
        [XmlAttribute] public Color CheckBoxBackground { get; internal set; }
        [XmlAttribute] public Color CheckBoxBorder { get; internal set; }
        [XmlAttribute] public Color CheckBoxHover { get; internal set; }

        /// <summary>
        /// Will create gradients between <see cref="CheckBoxBackground"/> and <see cref="Secondary"/>,
        /// instead of just the solid colors for Checkbox checked states.
        /// </summary>
        [XmlAttribute] public bool UseAccentGradientsForCheckboxes { get; internal set; }

        /// <summary>
        /// Will create gradients between <see cref="Primary"/> and <see cref="ButtonFace"/>,
        /// instead of just the solid colors for Buttons.
        /// </summary>
        [XmlAttribute] public bool UseAccentGradientsForButtons { get; internal set; }

        public static ColorScheme LightTheme = new ColorScheme
        {
            Primary = Color.Blue,
            Secondary = Color.Orange,
            ChartBackground = Color.White,
            ChartForeground = Color.Black,
            MouseMovement = Color.FromArgb(192, 0, 0),
            Field = SystemColors.Window,
            OnFocusedField = Color.FromArgb(16, 16, 16),
            OnField = Color.FromArgb(64, 64, 64),
            EditedField = Color.AntiqueWhite,
            OnEditedField = Color.DarkGray,
            ButtonFace = Color.DarkGray,
            ButtonBorder = Color.Gray,
            Control = SystemColors.Control,
            ControlBorder = Color.FromArgb(122, 122, 122),
            OnControl = Color.FromArgb(64, 64, 64),
            DisabledControl = Color.FromArgb(238, 238, 238),
            OnDisabledControl = Color.Gray,
            Background = SystemColors.Window,
            OnBackground = Color.FromArgb(64, 64, 64),
            Surface = SystemColors.Control,
            SurfaceVariant = SystemColors.ControlLight,
            MenuSelectedBorder = Color.FromArgb(0, 120, 215),
            MenuSelected = Color.FromArgb(179, 215, 243),
            CheckBoxBackground = Color.FromArgb(179, 215, 243),
            CheckBoxBorder = Color.FromArgb(0, 120, 215),
            CheckBoxHover = Color.FromArgb(113, 166, 207)
        };

        public static ColorScheme LightStreamerTheme = new ColorScheme
        {
            Primary = Color.Blue,
            Secondary = Color.Orange,
            ChartBackground = Color.Green,
            ChartForeground = Color.White,
            MouseMovement = Color.FromArgb(192, 0, 0),
            Field = SystemColors.Window,
            OnFocusedField = Color.FromArgb(16, 16, 16),
            OnField = Color.FromArgb(64, 64, 64),
            EditedField = Color.AntiqueWhite,
            OnEditedField = Color.DarkGray,
            ButtonFace = Color.DarkGray,
            ButtonBorder = Color.Gray,
            Control = SystemColors.Control,
            ControlBorder = Color.FromArgb(122, 122, 122),
            OnControl = Color.FromArgb(64, 64, 64),
            DisabledControl = Color.LightGray,
            OnDisabledControl = Color.Gray,
            Background = SystemColors.Window,
            OnBackground = Color.FromArgb(64, 64, 64),
            Surface = SystemColors.Control,
            SurfaceVariant = SystemColors.ControlLight,
            MenuSelectedBorder = Color.FromArgb(0, 120, 215),
            MenuSelected = Color.FromArgb(179, 215, 243),
            CheckBoxBackground = Color.FromArgb(179, 215, 243),
            CheckBoxBorder = Color.FromArgb(0, 120, 215),
            CheckBoxHover = Color.FromArgb(113, 166, 207),
        };

        public static ColorScheme DarkTheme = new ColorScheme
        {
            Primary = Color.FromArgb(147, 110, 227),
            Secondary = Color.FromArgb(223, 185, 136),
            MouseMovement = Color.FromArgb(237, 103, 103),
            ChartBackground = Color.FromArgb(57, 57, 57),
            ChartForeground = Color.FromArgb(234, 234, 234),
            Field = Color.FromArgb(81, 81, 81),
            OnFocusedField = Color.FromArgb(240, 240, 240),
            OnField = Color.FromArgb(184, 184, 184),
            EditedField = Color.FromArgb(116, 101, 31),
            OnEditedField = Color.FromArgb(232, 227, 222),
            ButtonFace = Color.FromArgb(100, 100, 100),
            ButtonBorder = Color.Gray,
            Control = Color.FromArgb(64, 64, 64),
            ControlBorder = Color.Gray,
            OnControl = Color.FromArgb(234, 234, 234),
            DisabledControl = Color.FromArgb(126, 126, 126),
            OnDisabledControl = Color.LightGray,
            Background = Color.FromArgb(57, 57, 57),
            OnBackground = Color.FromArgb(234, 234, 234),
            Surface = Color.FromArgb(48, 48, 48),
            SurfaceVariant = Color.FromArgb(35, 35, 35),
            MenuSelectedBorder = Color.FromArgb(64, 64, 64),
            MenuSelected = Color.FromArgb(77, 77, 77),
            CheckBoxBackground = Color.FromArgb(87, 87, 87),
            CheckBoxBorder = Color.FromArgb(102, 102, 102),
            CheckBoxHover = Color.FromArgb(135, 135, 135),
            UseAccentGradientsForCheckboxes = true
        };

        public static ColorScheme AccentedDarkTheme = new ColorScheme
        {
            Primary = Color.FromArgb(147, 110, 227),
            Secondary = Color.FromArgb(223, 185, 136),
            MouseMovement = Color.FromArgb(237, 103, 103),
            ChartBackground = Color.FromArgb(57, 57, 57),
            ChartForeground = Color.FromArgb(234, 234, 234),
            Field = Color.FromArgb(81, 81, 81),
            OnFocusedField = Color.FromArgb(240, 240, 240),
            OnField = Color.FromArgb(184, 184, 184),
            EditedField = Color.FromArgb(116, 101, 31),
            OnEditedField = Color.FromArgb(232, 227, 222),
            ButtonFace = Color.FromArgb(147, 110, 227),
            ButtonBorder = Color.Gray,
            Control = Color.FromArgb(64, 64, 64),
            ControlBorder = Color.FromArgb(223, 185, 136),
            OnControl = Color.FromArgb(234, 234, 234),
            DisabledControl = Color.FromArgb(126, 126, 126),
            OnDisabledControl = Color.LightGray,
            Background = Color.FromArgb(57, 57, 57),
            OnBackground = Color.FromArgb(234, 234, 234),
            Surface = Color.FromArgb(48, 48, 48),
            SurfaceVariant = Color.FromArgb(35, 35, 35),
            MenuSelectedBorder = Color.FromArgb(64, 64, 64),
            MenuSelected = Color.FromArgb(77, 77, 77),
            CheckBoxBackground = Color.FromArgb(87, 87, 87),
            CheckBoxBorder = Color.FromArgb(223, 185, 136),
            CheckBoxHover = Color.FromArgb(135, 135, 135),
            UseAccentGradientsForCheckboxes = true,
            UseAccentGradientsForButtons = true
        };

        public static ColorScheme DarkStreamerTheme = new ColorScheme
        {
            Primary = Color.FromArgb(147, 110, 227),
            Secondary = Color.FromArgb(223, 185, 136),
            MouseMovement = Color.FromArgb(237, 103, 103),
            ChartBackground = Color.Green,
            ChartForeground = Color.White,
            Field = Color.FromArgb(81, 81, 81),
            OnField = Color.FromArgb(234, 234, 234),
            EditedField = Color.FromArgb(116, 101, 31),
            OnEditedField = Color.FromArgb(232, 227, 222),
            ButtonFace = Color.FromArgb(100, 100, 100),
            ButtonBorder = Color.Gray,
            Control = Color.FromArgb(64, 64, 64),
            ControlBorder = Color.Gray,
            OnControl = Color.FromArgb(234, 234, 234),
            DisabledControl = Color.FromArgb(126, 126, 126),
            OnDisabledControl = Color.LightGray,
            Background = Color.FromArgb(57, 57, 57),
            OnBackground = Color.FromArgb(234, 234, 234),
            Surface = Color.FromArgb(48, 48, 48),
            SurfaceVariant = Color.FromArgb(35, 35, 35),
            MenuSelectedBorder = Color.FromArgb(64, 64, 64),
            MenuSelected = Color.FromArgb(77, 77, 77),
            CheckBoxBackground = Color.FromArgb(87, 87, 87),
            CheckBoxBorder = Color.FromArgb(102, 102, 102),
            CheckBoxHover = Color.FromArgb(135, 135, 135),
            UseAccentGradientsForCheckboxes = true
        };
    }
}
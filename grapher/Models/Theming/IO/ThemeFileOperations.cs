using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace grapher.Models.Theming.IO
{
    public class ThemeFileOperations
    {
        public string ThemePath { get; set; }

        public IEnumerable<ColorScheme> LoadThemes()
        {
            ThemePath = Path.Combine(Environment.CurrentDirectory, "/themes");

            var pathFound = Directory.Exists(ThemePath);

            if (!pathFound)
            {
                Directory.CreateDirectory(ThemePath);
                CreateDefaultThemes();
            }

            var schemes = new List<ColorScheme>();

            var files = Directory.GetFiles(ThemePath);
            foreach (var filePath in files)
            {
                var xmlDoc = new XmlDocument();

                xmlDoc.Load(filePath);
                var xml = XDocument.Parse(xmlDoc.OuterXml);
                schemes.Add(ColorSchemeManager.FromXml(xml));
            }

            return schemes;
        }

        private void CreateDefaultThemes()
        {
            // Save all the "preset" themes
            var lightTheme = ColorScheme.LightTheme;
            var lightStreamerTheme = ColorScheme.LightStreamerTheme;
            var darkTheme = ColorScheme.DarkTheme;
            var darkStreamerTheme = ColorScheme.DarkStreamerTheme;
            var accentedDarkTheme = ColorScheme.AccentedDarkTheme;

            SaveScheme(lightTheme, "LightTheme");
            SaveScheme(lightStreamerTheme, "LightStreamerTheme");
            SaveScheme(darkTheme, "DarkTheme");
            SaveScheme(darkStreamerTheme, "DarkStreamerTheme");
            SaveScheme(accentedDarkTheme, "AccentedDarkTheme");
        }

        public bool SaveScheme(ColorScheme scheme, string filename)
        {
            var xml = ColorSchemeManager.ToXml(scheme);

            var xmlDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };

            using (var reader = xml.CreateReader())
            {
                xmlDoc.Load(reader);
            }

            var fullPath = $"{ThemePath}/{filename}.xml";

            xmlDoc.Save(fullPath);

            return true;
        }
    }
}
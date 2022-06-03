using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using grapher.Models.Serialized;
using grapher.Models.Theming.IO;

namespace grapher.Models.Theming
{
    public static class ColorSchemeManager
    {
        private static XmlSerializer _xmlSerializer;

        public static XmlSerializer XmlSerializer =>
            _xmlSerializer ?? (_xmlSerializer = new XmlSerializer(typeof(ColorScheme)));

        public static ColorScheme FromXml(XDocument xml)
        {
            ColorScheme deserializedObject;
            using (var reader = xml.CreateReader())
            {
                deserializedObject = (ColorScheme)XmlSerializer.Deserialize(reader);
            }

            return deserializedObject;
        }

        public static XDocument ToXml(ColorScheme scheme)
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                XmlSerializer.Serialize(writer, scheme);
            }

            return doc;
        }

        /// <summary>
        /// Retrieve the selected <see cref="ColorScheme"/> according to the <paramref name="settings"/>.
        /// </summary>
        public static ColorScheme GetSelected(GUISettings settings, IEnumerable<ColorScheme> schemes)
        {
            if (settings == null)
            {
                return ColorScheme.LightTheme;
            }

            if (schemes == null)
            {
                schemes = LoadSchemes();
            }

            var scheme = schemes.FirstOrDefault(s => s.Name == settings.CurrentColorScheme);
            return scheme ?? ColorScheme.LightTheme;
        }

        public static IEnumerable<ColorScheme> LoadSchemes()
        {
            var operations = new ThemeFileOperations();
            return operations.LoadThemes();
        }

        public static ColorScheme FromName(string name)
        {
            var operations = new ThemeFileOperations();
            var schemes = operations.LoadThemes();

            var scheme = schemes.FirstOrDefault(s=> s.Name == name);
            return scheme ?? ColorScheme.LightTheme;
        }
    }
}
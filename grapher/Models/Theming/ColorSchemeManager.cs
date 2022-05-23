using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static ColorScheme GetSelected(GUISettings settings, List<ColorScheme> schemes = null)
        {
            if (settings == null)
            {
                return ColorScheme.LightTheme;
            }

            if (schemes == null)
            {
                var operations = new ThemeFileOperations();
                schemes = operations.LoadThemes();
            }

            foreach (var scheme in schemes.Where(scheme => scheme.Name == settings.CurrentColorScheme))
            {
                return scheme;
            }

            return ColorScheme.LightTheme;
        }

        public static ColorScheme FromName(string name)
        {
            var operations = new ThemeFileOperations();
            var schemes = operations.LoadThemes();

            foreach (var scheme in schemes.Where(scheme => scheme.Name == name))
            {
                return scheme;
            }

            return ColorScheme.LightTheme;
        }
    }
}
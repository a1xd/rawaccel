using System;
using System.Drawing;
using System.Xml.Serialization;

namespace grapher.Models.Theming.IO
{
    public class XmlColor
    {
        private Color _color = Color.Black;

        public XmlColor()
        {
        }

        public XmlColor(Color c)
        {
            _color = c;
        }


        public Color ToColor()
        {
            return _color;
        }

        public void FromColor(Color c)
        {
            _color = c;
        }

        public static implicit operator Color(XmlColor x)
        {
            return x.ToColor();
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        [XmlAttribute]
        public string Web
        {
            get => ColorTranslator.ToHtml(_color);
            set
            {
                try
                {
                    if (Alpha == 0xFF) // preserve named color value if possible
                        _color = ColorTranslator.FromHtml(value);
                    else
                        _color = Color.FromArgb(Alpha, ColorTranslator.FromHtml(value));
                }
                catch (Exception)
                {
                    _color = Color.Black;
                }
            }
        }

        [XmlAttribute]
        public byte Alpha
        {
            get => _color.A;
            set
            {
                if (value != _color.A) // avoid hammering named color if no alpha change
                    _color = Color.FromArgb(value, _color);
            }
        }

        public bool ShouldSerializeAlpha()
        {
            return Alpha < 0xFF;
        }
    }
}
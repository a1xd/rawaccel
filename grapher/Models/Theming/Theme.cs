using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;

namespace grapher.Models.Theming
{
    public class Theme
    {
        public ColorScheme Scheme { get; set; }

        public void Apply(Form form)
        {
            form.BackColor = Scheme.Background;
            form.ForeColor = Scheme.OnBackground;

            if (!form.HasChildren)
            {
                return;
            }

            ApplyTheme(form.Controls);
        }

        public void ApplyTheme(Control.ControlCollection container)
        {
            foreach (Control control in container)
            {

                switch (control)
                {
                    case Chart chart:
                    {
                        chart.BackColor = Scheme.ChartBackground;
                        chart.ForeColor = Scheme.ChartForeground;
                        chart.Series[0].Color = Scheme.Primary;
                        chart.Series[1].Color = Scheme.MouseMovement;
                        chart.Series[2].Color = Scheme.Secondary;
                        break;
                    }
                    case ComboBox comboBox:
                    {
                        control.BackColor = Scheme.Control;
                        control.ForeColor = Scheme.OnControl;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        comboBox.DrawItem += this.ComboBox_DrawItem;

                        break;
                    }
                    case Label _:
                    {
                        control.ForeColor = Scheme.OnBackground;
                        break;
                    }
                    case TextBox _:
                    {
                        control.BackColor = Scheme.Field;
                        control.ForeColor = Scheme.OnField;

                        break;
                    }
                    default:
                    {
                        control.BackColor = Scheme.Control;
                        control.ForeColor = Scheme.OnControl;
                        break;
                    }
                }
                if (control.HasChildren)
                {
                    ApplyTheme(control.Controls);
                }
            }
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs args)
        {
            var index = args.Index >= 0 ? args.Index : 0;

            args.DrawBackground();
            const int padding = 0;
            var backgroundBoundingBox = new Rectangle(
                padding,
                args.Bounds.Top + padding,
                args.Bounds.Height,
                args.Bounds.Height + (padding * 2)
            );
            args.Graphics.FillRectangle(
                new SolidBrush(Scheme.Surface),
                backgroundBoundingBox
            );

            var textBoundingBox = new RectangleF(
                args.Bounds.X + backgroundBoundingBox.Width,
                args.Bounds.Y,
                args.Bounds.Width,
                args.Bounds.Height
            );

            var currentEntry = (sender as ComboBox)?.Items[index];

            args.Graphics.DrawString(currentEntry?.ToString(), args.Font, new SolidBrush(Scheme.OnBackground), textBoundingBox);

            args.DrawFocusRectangle();
        }
    }
}
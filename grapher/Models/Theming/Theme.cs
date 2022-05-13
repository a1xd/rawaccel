using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;
using grapher.Models.Theming.Controls;

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
                    {

                        break;
                    }
                    case Label _:
                    case ThemeableComboBox comboBox:
                    {
                        comboBox.BackColor = CurrentScheme.Field;
                        comboBox.ForeColor = CurrentScheme.OnField;
                        comboBox.BorderColor = CurrentScheme.ControlBorder;
                        comboBox.ButtonColor = CurrentScheme.Control;
                        break;
                    }
                    {
                        control.ForeColor = Scheme.OnBackground;
                    case ComboBox _:
                    {
                        Console.WriteLine(
                            "Please replace all ComboBoxes with the ThemeambleComboBox, so theming can be applied"
                        );
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

        {
        }
    }
}
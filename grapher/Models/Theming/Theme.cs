using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using grapher.Models.Theming.Controls;

namespace grapher.Models.Theming
{
    public class Theme
    {
        public static ColorScheme CurrentScheme { get; set; }

        public static void Apply(Form form)
        {
            form.BackColor = CurrentScheme.Background;
            form.ForeColor = CurrentScheme.OnBackground;

            if (!form.HasChildren)
            {
                return;
            }

            ApplyTheme(form.Controls);
        }

        public static void ApplyTheme(Control.ControlCollection container)
        {
            foreach (Control control in container)
            {
                switch (control)
                {
                    case Chart chart:
                    {
                        SetChartColors(chart);

                        chart.Series[0].Color = CurrentScheme.Primary;
                        chart.Series[1].Color = CurrentScheme.PrimaryMouseMovement;
                        chart.Series[2].Color = CurrentScheme.Secondary;
                        if (chart.Series.Count > 3)
                        {
                            chart.Series[3].Color = CurrentScheme.SecondaryMouseMovement;
                        }

                        break;
                    }
                    case Label _:
                    {
                        control.ForeColor = CurrentScheme.OnBackground;
                        break;
                    }
                        break;
                    }
                    case ThemeableComboBox comboBox:
                    {
                        comboBox.BackColor = CurrentScheme.Field;
                        comboBox.ForeColor = CurrentScheme.OnField;
                        comboBox.BorderColor = CurrentScheme.ControlBorder;
                        comboBox.ButtonColor = CurrentScheme.Control;
                        break;
                    }
                    case ThemeableTextBox textBox:
                    {
                        textBox.BackColor = CurrentScheme.Field;
                        textBox.ForeColor = CurrentScheme.OnField;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.BorderColor = CurrentScheme.ControlBorder;
                        break;
                    }
                    case ComboBox _:
                    {
                        Console.WriteLine(
                            "Please replace all ComboBoxes with the ThemeambleComboBox, so theming can be applied"
                        );
                        break;
                    }
                    case TextBox _:
                    {
                        Console.WriteLine(
                            "Please replace all TextBoxes with the ThemeambleTextBox, so theming can be correctly applied"
                        );

                        break;
                    }
                    default:
                    {
                        control.BackColor = CurrentScheme.Control;
                        control.ForeColor = CurrentScheme.OnControl;
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
        private static void SetChartColors(Chart chart)
        {
            chart.ForeColor = CurrentScheme.ChartForeground;
            chart.BackColor = CurrentScheme.ChartBackground;

            chart.Titles[0].ForeColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].AxisX.LineColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisY.LineColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].AxisX.MinorGrid.LineColor = CurrentScheme.ChartForeground;
            chart.Legends[0].ForeColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].AxisX.TitleForeColor = CurrentScheme.ChartForeground;
            chart.ChartAreas[0].AxisY.TitleForeColor = CurrentScheme.ChartForeground;

            chart.ChartAreas[0].BorderColor = CurrentScheme.ChartForeground;
        }
    }
}
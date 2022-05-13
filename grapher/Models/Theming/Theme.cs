using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
                        chart.Series[1].Color = CurrentScheme.MouseMovement;
                        chart.Series[2].Color = CurrentScheme.Secondary;
                        if (chart.Series.Count > 3)
                        {
                            chart.Series[3].Color = CurrentScheme.MouseMovement;
                        }

                        break;
                    }
                    case Label _:
                    {
                        control.ForeColor = CurrentScheme.OnBackground;
                        break;
                    }
                    case CheckBox checkBox:
                    {
                        checkBox.Appearance = Appearance.Button;
                        checkBox.FlatStyle = FlatStyle.Flat;
                        checkBox.TextAlign = ContentAlignment.MiddleLeft;
                        checkBox.FlatAppearance.BorderSize = 0;
                        checkBox.AutoSize = false;
                        checkBox.BackColor = CurrentScheme.Background;
                        checkBox.ForeColor = CurrentScheme.OnControl;
                        checkBox.Paint += Theme.CheckBox_Paint;
                        break;
                    }
                    case Panel panel:
                    {
                        control.BackColor = CurrentScheme.Background;
                        control.ForeColor = CurrentScheme.OnBackground;
                        break;
                    }
                    case Button button:
                    {
                        if (button.FlatStyle == FlatStyle.Flat)
                        {
                            // Handle this button like a Label instead.
                            control.ForeColor = CurrentScheme.OnBackground;
                            control.BackColor = CurrentScheme.Background;
                            break;
                        }

                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderColor = CurrentScheme.ButtonBorder;
                        button.ForeColor = CurrentScheme.OnControl;
                        button.BackColor = CurrentScheme.ButtonFace;
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
                        textBox.BorderStyle = BorderStyle.Fixed3D;
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

        private static void CheckBox_Paint(object sender, PaintEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            e.Graphics.Clear(CurrentScheme.Background);
            const int checkBoxSize = 13;
            const int textSpacing = 5;

            using (var brush = new SolidBrush(checkBox.ForeColor))
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(checkBox.Text, checkBox.Font, brush, checkBoxSize + textSpacing, 1);
            }

            var pt = new Point(0, 0);
            var rect = new Rectangle(pt, new Size(checkBoxSize, checkBoxSize));

            using (var brush = new SolidBrush(checkBox.BackColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            if (checkBox.Checked)
            {
                const int padding = 2;
                var selectRect = new Rectangle(
                    new Point(padding, padding),
                    new Size(checkBoxSize - Convert.ToInt32(padding * 1.5),
                        checkBoxSize - Convert.ToInt32(padding * 1.5)
                    )
                );

                if (CurrentScheme.UseAccentGradientsForCheckboxes)
                {
                    using (var brush = new LinearGradientBrush(
                               rect, CurrentScheme.Primary, CurrentScheme.Secondary, 315)
                          )
                    {
                        e.Graphics.FillRectangle(brush, selectRect);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(CurrentScheme.Primary))
                    {
                        e.Graphics.FillRectangle(brush, selectRect);
                    }
                }
            }

            using (var pen = new Pen(CurrentScheme.ControlBorder))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
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
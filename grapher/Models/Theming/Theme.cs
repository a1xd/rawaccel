using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using grapher.Models.Theming.Controls;
using Cursor = System.Windows.Forms.Cursor;

namespace grapher.Models.Theming
{
    public class Theme
    {
        private const TextFormatFlags TextFormatFlags = System.Windows.Forms.TextFormatFlags.HorizontalCenter |
                                                        System.Windows.Forms.TextFormatFlags.VerticalCenter |
                                                        System.Windows.Forms.TextFormatFlags.WordBreak;

        public static ColorScheme CurrentScheme
        {
            get => _currentScheme;
            set
            {
                _previousScheme = _currentScheme;
                _currentScheme = value;
            }
        }

        private static ColorScheme _previousScheme = null;
        private static ColorScheme _currentScheme;

        public static void Apply(Form form, MenuStrip menu = null)
        {
            form.BackColor = CurrentScheme.Background;
            form.ForeColor = CurrentScheme.OnBackground;

            if (form.HasChildren)
            {
                if (_previousScheme != null)
                {
                    RemoveEvents(form.Controls);
                }

                ApplyTheme(form.Controls);
            }

            if (menu == null)
            {
                return;
            }

            menu.BackColor = CurrentScheme.MenuBackground;
            menu.ForeColor = CurrentScheme.OnControl;

            var menuRenderer = new StyledMenuRenderer();
            menu.Renderer = menuRenderer;

            foreach (ToolStripMenuItem item in menu.Items)
            {
                ApplyTheme(item);
            }
        }

        private static void ApplyTheme(ToolStripDropDownItem item)
        {
            item.BackColor = CurrentScheme.MenuBackground;
            item.ForeColor = CurrentScheme.OnControl;

            if (!item.HasDropDownItems) return;

            foreach (ToolStripItem child in item.DropDownItems)
            {
                if (child is ToolStripMenuItem toolStripItem)
                {
                    ApplyTheme(toolStripItem);
                }
            }
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
                        checkBox.Paint += CheckBox_Paint;
                        break;
                    }
                    case Panel _:
                    {
                        control.BackColor = CurrentScheme.Surface;
                        control.ForeColor = CurrentScheme.OnBackground;
                        break;
                    }
                    case Button button:
                    {
                        if (button.FlatStyle == FlatStyle.Flat)
                        {
                            // Handle this button like a Label instead.
                            control.ForeColor = CurrentScheme.OnBackground;
                            control.BackColor = CurrentScheme.Surface;
                            break;
                        }

                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderColor = CurrentScheme.ButtonBorder;
                        button.ForeColor = CurrentScheme.OnControl;
                        button.BackColor = CurrentScheme.ButtonFace;
                        button.Paint += Button_Paint;
                        button.EnabledChanged += Button_EnabledChanged;

                        break;
                    }
                    case ThemeableRichTextBox richTextBox:
                    {
                        control.BackColor = CurrentScheme.Field;
                        control.ForeColor = CurrentScheme.OnField;
                        richTextBox.BorderStyle = BorderStyle.Fixed3D;
                        richTextBox.BorderColor = CurrentScheme.ControlBorder;
                        richTextBox.ReadOnlyBackColor = CurrentScheme.DisabledControl;
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
                    case RichTextBox _:
                    {
                        Console.WriteLine(
                            "Please replace all RichTextBoxes with the ThemeambleRichTextBox, so theming can be correctly applied"
                        );

                        break;
                    }
                }

                if (control.HasChildren)
                {
                    ApplyTheme(control.Controls);
                }
            }
        }

        private static void RemoveEvents(Control.ControlCollection container)
        {
            foreach (Control control in container)
            {
                switch (control)
                {
                    case CheckBox checkBox:
                    {
                        checkBox.Paint -= CheckBox_Paint;
                        break;
                    }
                    case Button button:
                    {
                        button.Paint -= Button_Paint;
                        button.EnabledChanged -= Button_EnabledChanged;

                        break;
                    }
                }

                if (control.HasChildren)
                {
                    RemoveEvents(control.Controls);
                }
            }
        }

        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            var button = (Button)sender;

            button.BackColor = !button.Enabled ? CurrentScheme.DisabledControl : CurrentScheme.ButtonFace;
        }

        private static void Button_Paint(object sender, PaintEventArgs e)
        {
            var button = (Button)sender;

            if (!button.Enabled)
            {
                e.Graphics.Clear(button.BackColor);

                TextRenderer.DrawText(e.Graphics, button.Text, button.Font, e.ClipRectangle, button.ForeColor,
                    TextFormatFlags);

                return;
            }

            if (!CurrentScheme.UseAccentGradientsForButtons) return;

            DrawGradientButton(e, button);
        }

        private static void DrawGradientButton(PaintEventArgs e, Button button)
        {
            using (var brush = GetGradientBrush(button.ClientRectangle, CurrentScheme.Secondary,
                       CurrentScheme.ButtonFace))
            {
                e.Graphics.FillRectangle(brush, button.ClientRectangle);
            }

            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, e.ClipRectangle, button.ForeColor,
                TextFormatFlags);


            var mousePos = button.PointToClient(Cursor.Position);
            var isHovering = button.ClientRectangle.Contains(mousePos);

            if (isHovering)
            {
                if ((Control.MouseButtons & MouseButtons.Left) != 0)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(63, 255, 255, 255)))
                    {
                        e.Graphics.FillRectangle(brush, button.ClientRectangle);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(Color.FromArgb(63, 81, 81, 81)))
                    {
                        e.Graphics.FillRectangle(brush, button.ClientRectangle);
                    }
                }
            }

            const int borderWidth = 2;
            using (var pen = new Pen(CurrentScheme.ButtonBorder, borderWidth))
            {
                var borderRect = new Rectangle(borderWidth / 2, borderWidth / 2,
                    button.ClientRectangle.Width - borderWidth, button.ClientRectangle.Height - borderWidth);
                e.Graphics.DrawRectangle(pen, borderRect);
            }
        }

        private static void CheckBox_Paint(object sender, PaintEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            e.Graphics.Clear(CurrentScheme.Surface);
            const int checkBoxSize = 13;
            const int textSpacing = 5;

            const int textOffset = checkBoxSize + textSpacing;
            var textBounds = new Rectangle(textOffset, 0, e.ClipRectangle.Width - textOffset,
                e.ClipRectangle.Height);
            TextRenderer.DrawText(e.Graphics, checkBox.Text, checkBox.Font, textBounds, checkBox.ForeColor,
                TextFormatFlags);

            var pt = new Point(0, 0);
            var rect = new Rectangle(pt, new Size(checkBoxSize, checkBoxSize));

            var mousePos = checkBox.PointToClient(Cursor.Position);
            var isHovering = checkBox.ClientRectangle.Contains(mousePos);

            if (isHovering)
            {
                using (var brush = new SolidBrush(CurrentScheme.CheckBoxHover))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            else
            {
                using (var brush = new SolidBrush(CurrentScheme.CheckBoxBackground))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
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
                    using (var brush = GetGradientBrush(rect,
                               isHovering ? CurrentScheme.CheckBoxHover : CurrentScheme.CheckBoxChecked,
                               CurrentScheme.Primary)
                          )
                    {
                        e.Graphics.FillRectangle(brush, selectRect);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(CurrentScheme.CheckBoxChecked))
                    {
                        e.Graphics.FillRectangle(brush, selectRect);
                    }
                }
            }

            using (var pen = new Pen(CurrentScheme.CheckBoxBorder))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private static LinearGradientBrush GetGradientBrush(
            Rectangle rect, Color primary, Color secondary, int angle = 315
        )
        {
            return new LinearGradientBrush(rect,
                primary,
                secondary,
                angle
            );
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
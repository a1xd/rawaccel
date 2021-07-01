using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.LUT
{
    public class LUTPanelOptions : OptionBase
    {
        public const int PanelPadding = 5;
        public const int PanelHeight = 100;

        public LUTPanelOptions(Panel activePanel, RichTextBox pointsTextBox)
        {
            PointsTextBox = pointsTextBox;

            ActivePanel = activePanel;
            ActivePanel.Height = PanelHeight;
            ActivePanel.Paint += Panel_Paint;

            ActiveValuesTextBox = new RichTextBox();
            ActiveValuesTextBox.ReadOnly = true;
            ActivePanel.Controls.Add(ActiveValuesTextBox);
        }

        public RichTextBox PointsTextBox
        {
            get;
        }

        public RichTextBox ActiveValuesTextBox
        {
            get;
        }

        public Panel ActivePanel
        {
            get;
        }

        public override bool Visible
        {
            get
            {
                return PointsTextBox.Visible || ShouldShow;
            }
        }

        public override int Top
        {
            get
            {
                return PointsTextBox.Top;
            }
            set
            {
                PointsTextBox.Top = value;
                ActivePanel.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return PointsTextBox.Height;
            }
        }

        public override int Left
        {
            get
            {
                return PointsTextBox.Left;
            }
            set
            {
                PointsTextBox.Left = value;
                AlignActivePanelToPointsTextBox();
            }
        }

        public override int Width
        {
            get
            {
                return PointsTextBox.Width;
            }
            set
            {
                var panelWidth = value / 2 - PanelPadding;
                PointsTextBox.Width = panelWidth;
                ActivePanel.Width = panelWidth;
                AlignActivePanelToPointsTextBox();
            }
        }

        private bool ShouldShow { get; set; }

        public override void Hide()
        {
            PointsTextBox.Hide();
            ActivePanel.Hide();
            ShouldShow = false;
        }

        public override void Show(string name)
        {
            PointsTextBox.Show();
            ActivePanel.Show();
            ShouldShow = true;
        }

        public override void AlignActiveValues()
        {
            // Nothing to do here.
        }

        public void SetActiveValues(IEnumerable<Vec2<float>> activePoints)
        {
            if (activePoints.Any() && activePoints.First().x != 0)
            {
                ActiveValuesTextBox.Text = PointsToActiveValuesText(activePoints);
            }
            else
            {
                ActiveValuesTextBox.Text = string.Empty;
            }
        }

        public Vec2<float>[] GetPoints()
        {
            return UserTextToPoints(PointsTextBox.Text);
        }

        private static Vec2<float>[] UserTextToPoints(string userText)
        {
            if (string.IsNullOrWhiteSpace(userText))
            {
                throw new Exception("Text must be entered in text box to fill Look Up Table.");
            }

            Vec2<float>[] points = new Vec2<float>[256];

            var userTextSplit = userText.Split(';');
            int index = 0;
            float lastX = 0;

            foreach(var pointEntry in userTextSplit)
            {
                var pointSplit = pointEntry.Trim().Split(',');

                if (pointSplit.Length != 2)
                {
                    throw new Exception($"Point at index {index} is malformed. Expected format: x,y; Given: {pointEntry.Trim()}");
                }

                float x;
                float y;

                try
                {
                    x = float.Parse(pointSplit[0]);
                }
                catch (Exception ex)
                {
                    throw new Exception($"X-value for point at index {index} is malformed. Expected: float. Given: {pointSplit[0]}", ex);
                }

                if (x <= 0)
                {
                    throw new Exception($"X-value for point at index {index} is less than or equal to 0. Point (0,0) is implied and should not be specified in points text.");
                }

                if (x <= lastX)
                {
                    throw new Exception($"X-value for point at index {index} is less than or equal to previous x-value. Value: {x} Previous: {lastX}");
                }

                lastX = x;

                try
                {
                    y = float.Parse(pointSplit[1]);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Y-value for point at index {index} is malformed. Expected: float. Given: {pointSplit[1]}", ex);
                }

                if (y <= 0)
                {
                    throw new Exception($"Y-value for point at index {index} is less than or equal to 0. Value: {y}");
                }

                points[index] = new Vec2<float> { x = x, y = y };

                index++;
            }

            return points;
        }

        private void AlignActivePanelToPointsTextBox()
        {
            ActivePanel.Left = PointsTextBox.Right + PanelPadding;
        }

        private string PointsToActiveValuesText(IEnumerable<Vec2<float>> points)
        {
            StringBuilder builder = new StringBuilder();

            foreach(var point in points)
            {
                builder.AppendLine($"{point.x},{point.y};");
            }

            return builder.ToString();
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Color col = Color.DarkGray;
            ButtonBorderStyle bbs = ButtonBorderStyle.Dashed;
            int thickness = 2;
            ControlPaint.DrawBorder(e.Graphics, ActivePanel.ClientRectangle, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs);
        }
    }
}

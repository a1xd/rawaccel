using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using grapher.Models.Theming;

namespace grapher.Models.Options.Directionality
{
    public class DirectionalityOptions
    {
        public DirectionalityOptions(
            Panel containingPanel,
            Button directionalityLabel,
            Label directionalityX,
            Label directionalityY,
            Label directionalityActiveValueTitle,
            Option lpNorm,
            OptionXY domain,
            OptionXY range,
            CheckBox wholeCheckBox,
            CheckBox byComponentCheckBox,
            int top)
        {
            ContainingPanel = containingPanel;

            DirectionalityLabel = directionalityLabel;
            DirectionalityLabel.FlatStyle = FlatStyle.Flat;
            DirectionalityLabel.FlatAppearance.BorderSize = 0;
            DirectionalityLabel.FlatAppearance.MouseDownBackColor = Theme.CurrentScheme.Control;
            DirectionalityLabel.FlatAppearance.CheckedBackColor = Theme.CurrentScheme.Control;
            DirectionalityLabel.FlatAppearance.MouseOverBackColor = Theme.CurrentScheme.Control;

            DirectionalityX = directionalityX;
            DirectionalityY = directionalityY;
            DirectionalityActiveValueTitle = directionalityActiveValueTitle;
            LpNorm = lpNorm;
            Domain = domain;
            Range = range;
            WholeCheckBox = wholeCheckBox;
            ByComponentCheckBox = byComponentCheckBox;

            ContainingPanel.Paint += Panel_Paint;
            DirectionalityLabel.Click += Title_click;
            ContainingPanel.Top = top;
            DirectionalityLabel.Left = Constants.DirectionalityTitlePad;
            DirectionalityLabel.Top = Constants.DirectionalityTitlePad;
            IsHidden = false;
            ToWhole();
            Hide();
        }

        public Panel ContainingPanel { get; }

        public Button DirectionalityLabel { get; }

        public Label DirectionalityX { get; }

        public Label DirectionalityY { get; }

        public Label DirectionalityActiveValueTitle { get; }

        public Option LpNorm { get; }

        public OptionXY Domain { get; }

        public OptionXY Range { get; }

        public CheckBox WholeCheckBox { get; }

        public CheckBox ByComponentCheckBox { get; }

        public int OpenHeight { get => WholeCheckBox.Bottom - DirectionalityLabel.Top + 2 * Constants.DirectionalityTitlePad; }

        public int ClosedHeight { get => DirectionalityLabel.Height + 2 * Constants.DirectionalityTitlePad; }

        private bool IsHidden { get; set; }

        public Tuple<Vec2<double>, double> GetDomainArgs()
        {
            var weights = new Vec2<double>
            {
                x = Domain.Fields.X,
                y = Domain.Fields.Y
            };
            double p = ByComponentCheckBox.Checked ? 2 : LpNorm.Field.Data;

            return new Tuple<Vec2<double>, double>(weights, p);
        }

        public Vec2<double> GetRangeXY()
        {
            return new Vec2<double>
            {
                x = Range.Fields.X,
                y = Range.Fields.Y,
            };
        }

        public void SetActiveValues(Profile settings)
        {
            Domain.SetActiveValues(settings.domainXY.x, settings.domainXY.y);
            Range.SetActiveValues(settings.rangeXY.x, settings.rangeXY.y);

            if (settings.combineMagnitudes)
            {
                LpNorm.SetActiveValue(settings.lpNorm);
            }
            else
            {
                LpNorm.SetToUnavailable();
            }
        }

        public void Hide()
        {
            if (!IsHidden)
            {
                DirectionalityX.Hide();
                DirectionalityY.Hide();
                DirectionalityActiveValueTitle.Hide();
                LpNorm.Hide();
                Domain.Hide();
                Range.Hide();
                WholeCheckBox.Hide();
                ByComponentCheckBox.Hide();
                DirectionalityLabel.Text = Constants.DirectionalityTitleClosed;
                DrawHidden();
                IsHidden = true;
            }
        }

        public void Show()
        {
            if (IsHidden)
            {
                DirectionalityX.Show();
                DirectionalityY.Show();
                DirectionalityActiveValueTitle.Show();
                LpNorm.Show();
                Domain.Show();
                Range.Show();
                Range.Fields.LockCheckBox.Hide();
                WholeCheckBox.Show();
                ByComponentCheckBox.Show();
                DirectionalityLabel.Text = Constants.DirectionalityTitleOpen;
                DrawShown();
                IsHidden = false;
            }
        }

        public void ToByComponent()
        {
            LpNorm.SetToUnavailable();
        }

        public void ToWhole()
        {
            LpNorm.SetToAvailable();
        }

        private void DrawHidden()
        {
            ContainingPanel.Height = ClosedHeight;
            ContainingPanel.Invalidate();
        }

        private void DrawShown()
        {
            ContainingPanel.Height = OpenHeight;
            ContainingPanel.Invalidate();
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Color col = Theme.CurrentScheme.ControlBorder;
            ButtonBorderStyle bbs = ButtonBorderStyle.Dashed;
            int thickness = 1;
            ControlPaint.DrawBorder(e.Graphics, this.ContainingPanel.ClientRectangle, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs);
        }

        private void Title_click(object sender, EventArgs e)
        {
            if (IsHidden)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}

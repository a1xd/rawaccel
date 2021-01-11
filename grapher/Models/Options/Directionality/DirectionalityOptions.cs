using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.Directionality
{
    public class DirectionalityOptions
    {
        public DirectionalityOptions(
            Panel containingPanel,
            Label directionalityLabel,
            Label directionalityX,
            Label directionalityY,
            Option lpNorm,
            OptionXY domain,
            OptionXY range,
            CheckBox wholeCheckBox,
            CheckBox byComponentCheckBox)
        {
            ContainingPanel = containingPanel;
            DirectionalityLabel = directionalityLabel;
            DirectionalityX = directionalityX;
            DirectionalityY = directionalityY;
            LpNorm = lpNorm;
            Domain = domain;
            Range = range;
            WholeCheckBox = wholeCheckBox;
            ByComponentCheckBox = byComponentCheckBox;

            ContainingPanel.Paint += panel_Paint;
            DirectionalityLabel.Left = ContainingPanel.Left + Constants.DirectionalityTitlePad;
            DirectionalityLabel.Top = ContainingPanel.Top + Constants.DirectionalityTitlePad;
            ToWhole();
            Hide();
        }

        public Panel ContainingPanel { get; }

        public Label DirectionalityLabel { get; }

        public Label DirectionalityX { get; }

        public Label DirectionalityY { get; }

        public Option LpNorm { get; }

        public OptionXY Domain { get; }

        public OptionXY Range { get; }

        public CheckBox WholeCheckBox { get; }

        public CheckBox ByComponentCheckBox { get; }

        public DomainArgs GetDomainArgs()
        {
            if (!ByComponentCheckBox.Checked)
            {
                return new DomainArgs
                {
                    domainXY = new Vec2<double>
                    {
                        x = Domain.Fields.X,
                        y = Domain.Fields.Y,
                    },
                    lpNorm = LpNorm.Field.Data,
                };
            }
            else
            {
                return new DomainArgs
                {
                    domainXY = new Vec2<double>
                    {
                        x = 1,
                        y = 1,
                    },
                    lpNorm = 2,
                };

            }
        }

        public Vec2<double> GetRangeXY()
        {
            if (!ByComponentCheckBox.Checked)
            {
                return new Vec2<double>
                {
                    x = Range.Fields.X,
                    y = Range.Fields.Y,
                };
            }
            else
            {
                return new Vec2<double>
                {
                    x = 1,
                    y = 1,
                };
            }

        }

        public void SetActiveValues(DriverSettings settings)
        {
            Domain.SetActiveValues(settings.domainArgs.domainXY.x, settings.domainArgs.domainXY.y);
            LpNorm.SetActiveValue(settings.domainArgs.lpNorm);
            Range.SetActiveValues(settings.rangeXY.x, settings.rangeXY.y);
        }

        public void Hide()
        {
            DirectionalityX.Hide();
            DirectionalityY.Hide();
            LpNorm.Hide();
            Domain.Hide();
            Range.Hide();
            WholeCheckBox.Hide();
            ByComponentCheckBox.Hide();
            DrawHidden();
        }

        public void ToByComponent()
        {
            LpNorm.SetToUnavailable();
            Domain.SetToUnavailable();
            Range.SetToUnavailable();
        }

        public void ToWhole()
        {
            LpNorm.SetToAvailable();
            Domain.SetToAvailable();
            Range.SetToAvailable();
        }

        public void Show()
        {
            DirectionalityX.Hide();
            DirectionalityY.Hide();
            LpNorm.Hide();
            Domain.Hide();
            Range.Hide();
            WholeCheckBox.Hide();
            ByComponentCheckBox.Hide();
            DrawShown();
        }

        private void DrawHidden()
        {
            ContainingPanel.Height = DirectionalityLabel.Height + 2 * Constants.DirectionalityTitlePad;
        }

        private void DrawShown()
        {
            ContainingPanel.Height = WholeCheckBox.Bottom - DirectionalityLabel.Top + 2 * Constants.DirectionalityTitlePad;
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Color col = Color.DarkGray;
            ButtonBorderStyle bbs = ButtonBorderStyle.Dashed;
            int thickness = 2;
            ControlPaint.DrawBorder(e.Graphics, this.ContainingPanel.ClientRectangle, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs, col, thickness, bbs);
        }
    }
}

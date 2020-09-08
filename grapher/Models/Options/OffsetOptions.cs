using System;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class OffsetOptions : OptionBase
    {
        public OffsetOptions(
            ToolStripMenuItem velocityGainOffsetCheck,
            ToolStripMenuItem legacyOffsetCheck,
            Option offsetOption)
        {
            VelocityGainOffsetCheck = velocityGainOffsetCheck;
            LegacyOffsetCheck = legacyOffsetCheck;
            OffsetOption = offsetOption;

            VelocityGainOffsetCheck.Click += new System.EventHandler(OnVelocityGainOffsetClick);
            LegacyOffsetCheck.Click += new System.EventHandler(OnLegacyOffsetClick);

            VelocityGainOffsetCheck.CheckedChanged += new System.EventHandler(OnVelocityGainOffsetCheckedChange);
            LegacyOffsetCheck.CheckedChanged += new System.EventHandler(OnLegacyOffsetCheckedChange);

            VelocityGainOffsetCheck.Checked = true;
        }

        public ToolStripMenuItem VelocityGainOffsetCheck { get; }

        public ToolStripMenuItem LegacyOffsetCheck { get; }

        public Option OffsetOption { get; }

        public bool IsLegacy { get; private set; }

        public double LegacyOffset 
        { 
            get
            {
                if (IsLegacy)
                {
                    return OffsetOption.Field.Data;
                }
                else
                {
                    return 0;
                }
            } 
        }

        public double Offset
        {
            get
            {
                if (IsLegacy)
                {
                    return 0;
                }
                else
                {
                    return OffsetOption.Field.Data;
                }
            }
        }

        public override int Top
        {
            get
            {
                return OffsetOption.Top;
            }
            set
            {
                OffsetOption.Top = value;
            }
        }

        public override int Height
        {
            get
            {
                return OffsetOption.Height;
            }
        }

        public override int Left
        {
            get
            {
                return OffsetOption.Left;
            }
            set
            {
                OffsetOption.Left = value;
            }
        }

        public override int Width
        {
            get
            {
                return OffsetOption.Width;
            }
            set
            {
                OffsetOption.Width = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return OffsetOption.Visible;
            }
        }

        public override void Hide()
        {
            OffsetOption.Hide();
        }

        public void Show()
        {
            OffsetOption.Show();
        }

        public override void Show(string name)
        {
            OffsetOption.Show(name);
        }

        public void SetActiveValue(double offset, double legacyOffset)
        {
            if (offset > 0)
            {
                OffsetOption.SetActiveValue(offset);
            }
            else
            {
                OffsetOption.SetActiveValue(legacyOffset);
            }
        }

        public override void AlignActiveValues(int width)
        {
            OffsetOption.AlignActiveValues(width);
        }

        public void OnVelocityGainOffsetClick(object sender, EventArgs e)
        {
            if (!VelocityGainOffsetCheck.Checked)
            {
                VelocityGainOffsetCheck.Checked = true;
                LegacyOffsetCheck.Checked = false;
            }
        }

        public void OnLegacyOffsetClick(object sender, EventArgs e)
        {
            if (!LegacyOffsetCheck.Checked)
            {
                LegacyOffsetCheck.Checked = true;
                VelocityGainOffsetCheck.Checked = false;
            }
        }

        public void OnVelocityGainOffsetCheckedChange(object sender, EventArgs e)
        {
            if (VelocityGainOffsetCheck.Checked)
            {
                EnableVelocityGainOffset();
            }
        }

        public void OnLegacyOffsetCheckedChange(object sender, EventArgs e)
        {
            if (LegacyOffsetCheck.Checked)
            {
                EnableLegacyOffset();
            }
        }

        public void EnableVelocityGainOffset()
        {
            IsLegacy = false;
        }

        public void EnableLegacyOffset()
        {
            IsLegacy = true;
        }
    }
}

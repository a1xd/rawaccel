using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class OffsetOptions
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class CapOptions
    {
        public CapOptions(
            ToolStripMenuItem sensitivityCapCheck,
            ToolStripMenuItem velocityGainCapCheck,
            OptionXY capOptionFields)
        {

            SensitivityCapCheck = sensitivityCapCheck;
            VelocityGainCapCheck = velocityGainCapCheck;
            CapOptionsFields = capOptionFields;

            SensitivityCapCheck.Click += new System.EventHandler(OnSensitivityCapCheckClick);
            VelocityGainCapCheck.Click += new System.EventHandler(OnVelocityGainCapCheckClick);

            SensitivityCapCheck.CheckedChanged += new System.EventHandler(OnSensitivityCapCheckedChange);
            VelocityGainCapCheck.CheckedChanged += new System.EventHandler(OnVelocityGainCapCheckedChange);

            EnableSensitivityCap();
        }

        ToolStripMenuItem SensitivityCapCheck { get; }

        ToolStripMenuItem VelocityGainCapCheck { get; }

        OptionXY CapOptionsFields { get; }

        public double SensitivityCapX { 
            get
            {
                if (IsSensitivityGain)
                {
                    return CapOptionsFields.Fields.X;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SensitivityCapY { 
            get
            {
                if (IsSensitivityGain)
                {
                    return CapOptionsFields.Fields.Y;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double VelocityGainCap { 
            get
            {
                if (IsSensitivityGain)
                {
                    return 0;
                }
                else
                {
                    return CapOptionsFields.Fields.X;
                }
            }
        }

        public bool IsSensitivityGain { get; private set; }

        void OnSensitivityCapCheckClick(object sender, EventArgs e)
        {
            if (!SensitivityCapCheck.Checked)
            {
                VelocityGainCapCheck.Checked = false;
                SensitivityCapCheck.Checked = true;
            }
        }

        void OnVelocityGainCapCheckClick(object sender, EventArgs e)
        {
            if (!VelocityGainCapCheck.Checked)
            {
                VelocityGainCapCheck.Checked = true;
                SensitivityCapCheck.Checked = false;
            }
        }

        void OnSensitivityCapCheckedChange(object sender, EventArgs e)
        {
            if (SensitivityCapCheck.Checked == true)
            {
                EnableSensitivityCap();
            }
        }

        void OnVelocityGainCapCheckedChange(object sender, EventArgs e)
        {
            if (SensitivityCapCheck.Checked == true)
            {
                EnableVelocityGainCap();
            }
        }

        void EnableSensitivityCap()
        {
            IsSensitivityGain = true;
            CapOptionsFields.Fields.LockCheckBox.Enabled = true;
            CapOptionsFields.SetName("Sensitivity Cap");
        }

        void EnableVelocityGainCap()
        {
            IsSensitivityGain = false;
            CapOptionsFields.Fields.LockCheckBox.Checked = true;
            CapOptionsFields.Fields.LockCheckBox.Enabled = false;
            CapOptionsFields.SetName("Velocity Gain Cap");
        }
    }
}

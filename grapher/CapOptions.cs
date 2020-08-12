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
            OptionXY capOption,
            OptionXY weightOption)
        {

            SensitivityCapCheck = sensitivityCapCheck;
            VelocityGainCapCheck = velocityGainCapCheck;
            CapOption = capOption;
            WeightOption = weightOption;

            SensitivityCapCheck.Click += new System.EventHandler(OnSensitivityCapCheckClick);
            VelocityGainCapCheck.Click += new System.EventHandler(OnVelocityGainCapCheckClick);

            SensitivityCapCheck.CheckedChanged += new System.EventHandler(OnSensitivityCapCheckedChange);
            VelocityGainCapCheck.CheckedChanged += new System.EventHandler(OnVelocityGainCapCheckedChange);

            EnableSensitivityCap();
        }

        ToolStripMenuItem SensitivityCapCheck { get; }

        ToolStripMenuItem VelocityGainCapCheck { get; }

        OptionXY CapOption { get; }

        OptionXY WeightOption { get; }

        public double SensitivityCapX { 
            get
            {
                if (IsSensitivityGain)
                {
                    return CapOption.Fields.X;
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
                    return CapOption.Fields.Y;
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
                    return CapOption.Fields.X;
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
            CapOption.Fields.LockCheckBox.Enabled = true;
            WeightOption.Fields.LockCheckBox.Enabled = true;
            CapOption.SetName("Sensitivity Cap");
        }

        void EnableVelocityGainCap()
        {
            IsSensitivityGain = false;
            CapOption.Fields.LockCheckBox.Checked = true;
            CapOption.Fields.LockCheckBox.Enabled = false;
            WeightOption.Fields.LockCheckBox.Checked = true;
            WeightOption.Fields.LockCheckBox.Enabled = false;
            CapOption.SetName("Velocity Gain Cap");
        }
    }
}

using grapher.Models.Options;
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

        public const string GainCapFormatString = "0.##";

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

        public ToolStripMenuItem SensitivityCapCheck { get; }

        public ToolStripMenuItem VelocityGainCapCheck { get; }

        public OptionXY CapOption { get; }

        public OptionXY WeightOption { get; }

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

        public void SetActiveValues(double gainCap, double sensCapX, double sensCapY, bool capGainEnabled)
        {
            if (capGainEnabled)
            {
                CapOption.ActiveValueLabels.X.FormatString = GainCapFormatString;
                CapOption.ActiveValueLabels.X.Prefix = "Gain";
                CapOption.SetActiveValues(gainCap, gainCap);
                SensitivityCapCheck.Checked = true;
                VelocityGainCapCheck.Checked = false;
            }
            else
            {
                CapOption.ActiveValueLabels.X.FormatString = ActiveValueLabel.DefaultFormatString;
                CapOption.ActiveValueLabels.X.Prefix = string.Empty;
                CapOption.SetActiveValues(sensCapX, sensCapY);
                SensitivityCapCheck.Checked = false;
                VelocityGainCapCheck.Checked = true;
            }
        }

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
            if (SensitivityCapCheck.Checked)
            {
                EnableSensitivityCap();
            }
        }

        void OnVelocityGainCapCheckedChange(object sender, EventArgs e)
        {
            if (SensitivityCapCheck.Checked)
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

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
        #region Constants


        #endregion Constants

        #region Constructors

        public CapOptions(
            ToolStripMenuItem sensitivityCapCheck,
            ToolStripMenuItem velocityGainCapCheck,
            Option capOption)
        {

            SensitivityCapCheck = sensitivityCapCheck;
            VelocityGainCapCheck = velocityGainCapCheck;
            CapOption = capOption;

            SensitivityCapCheck.Click += new System.EventHandler(OnSensitivityCapCheckClick);
            VelocityGainCapCheck.Click += new System.EventHandler(OnVelocityGainCapCheckClick);

            SensitivityCapCheck.CheckedChanged += new System.EventHandler(OnSensitivityCapCheckedChange);
            VelocityGainCapCheck.CheckedChanged += new System.EventHandler(OnVelocityGainCapCheckedChange);

            EnableSensitivityCap();
        }

        #endregion Constructors

        #region Properties

        public ToolStripMenuItem SensitivityCapCheck { get; }

        public ToolStripMenuItem VelocityGainCapCheck { get; }

        public Option CapOption { get; }

        public bool IsSensitivityGain { get; private set; }

        public double SensitivityCap { 
            get
            {
                if (IsSensitivityGain)
                {
                    return CapOption.Field.Data;
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
                    return CapOption.Field.Data;
                }
            }
        }

        public int Top
        { 
            get 
            {
                return CapOption.Top;
            }
            set
            {
                CapOption.Top = value;
            }
        }

        public int Height
        {
            get
            {
                return CapOption.Height;
            }
        }


        #endregion Properties

        #region Methods

        public void Hide()
        {
            CapOption.Hide();
        }

        public void Show()
        {
            CapOption.Show();
        }

        public void SnapTo(Option option)
        {
            Top = option.Top + option.Height + Constants.OptionVerticalSeperation;
        }


        public void SetActiveValues(double gainCap, double sensCap, bool capGainEnabled)
        {
            if (capGainEnabled)
            {
                CapOption.ActiveValueLabel.FormatString = Constants.GainCapFormatString;
                CapOption.ActiveValueLabel.Prefix = "Gain";
                CapOption.SetActiveValue(gainCap);
                SensitivityCapCheck.Checked = true;
                VelocityGainCapCheck.Checked = false;
            }
            else
            {
                CapOption.ActiveValueLabel.FormatString = Constants.DefaultActiveValueFormatString;
                CapOption.ActiveValueLabel.Prefix = string.Empty;
                CapOption.SetActiveValue(sensCap);
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
            CapOption.SetName("Sensitivity Cap");
        }

        void EnableVelocityGainCap()
        {
            IsSensitivityGain = false;
            CapOption.SetName("Velocity Gain Cap");
        }

        #endregion Methods
    }
}

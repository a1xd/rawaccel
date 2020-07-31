using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class FieldXY
    {
        public FieldXY(TextBox xBox, TextBox yBox, CheckBox lockCheckBox, Form containingForm, double defaultData)
        {
            XField = new Field(xBox, containingForm, defaultData);
            YField = new Field(yBox, containingForm, defaultData);
            LockCheckBox = lockCheckBox;
            LockCheckBox.CheckedChanged += new System.EventHandler(CheckChanged);
            SetLocked();
        }
        public double X
        {
            get => XField.Data;
        }

        public double Y
        {
            get
            {
                if (Locked)
                {
                    return X;
                }
                else
                {
                    return YField.Data;
                }
            }
        }

        public CheckBox LockCheckBox { get; }

        public Field XField { get; }

        public Field YField { get; }

        private bool Locked { get; set; }

        private void CheckChanged(object sender, EventArgs e)
        {
            if (LockCheckBox.CheckState == CheckState.Checked)
            {
                SetLocked();
            }
            else
            {
                SetUnlocked();
            }
        }

        private void SetLocked()
        {
            Locked = true;
            YField.SetToUnavailable();
        }

        private void SetUnlocked()
        {
            Locked = false;
            if (XField.State == Field.FieldState.Default)
            {
                YField.SetToDefault();
            }
            else
            {
                YField.SetToEntered(XField.Data);
            }
        }
    }
}

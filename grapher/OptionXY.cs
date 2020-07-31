using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class OptionXY
    {
        public OptionXY(FieldXY fields, Label label)
        {
            Fields = fields;
            Label = label;
        }

        public OptionXY(
            TextBox xBox,
            TextBox yBox,
            CheckBox lockCheckBox,
            Form containingForm,
            double defaultData,
            Label label)
            : this(new FieldXY(xBox, yBox, lockCheckBox, containingForm, defaultData), label)
        {
        }

        public OptionXY(
            TextBox xBox,
            TextBox yBox,
            CheckBox lockCheckBox,
            Form containingForm,
            double defaultData,
            Label label,
            string startingName):
            this(
                xBox,
                yBox,
                lockCheckBox,
                containingForm,
                defaultData,
                label)
        {
            SetName(startingName);
        }

        public FieldXY Fields { get; }

        public Label Label { get; }

        public void SetName(string name)
        {
            Label.Text = name;
            Label.Left = Convert.ToInt32((Fields.XField.Box.Left / 2.0) - (Label.Width / 2.0));
        }

        public void Hide()
        {
            Fields.XField.Box.Hide();
            Fields.YField.Box.Hide();
            Label.Hide();
        }

        public void Show()
        {
            Fields.XField.Box.Show();
            Fields.YField.Box.Show();
            Label.Show();
        }

        public void Show(string name)
        {
            SetName(name);

            Fields.XField.Box.Show();
            Fields.YField.Box.Show();
            Label.Show();
        }

    }
}

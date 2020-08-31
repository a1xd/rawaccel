using grapher.Models.Options;
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
        public OptionXY(FieldXY fields, Label label, ActiveValueLabelXY activeValueLabels)
        {
            Fields = fields;
            Label = label;
            ActiveValueLabels = activeValueLabels;
        }

        public OptionXY(
            TextBox xBox,
            TextBox yBox,
            CheckBox lockCheckBox,
            Form containingForm,
            double defaultData,
            Label label,
            AccelCharts accelCharts,
            ActiveValueLabelXY activeValueLabels)
            : this(new FieldXY(xBox, yBox, lockCheckBox, containingForm, defaultData, accelCharts), label, activeValueLabels)
        {
        }

        public OptionXY(
            TextBox xBox,
            TextBox yBox,
            CheckBox lockCheckBox,
            Form containingForm,
            double defaultData,
            Label label,
            ActiveValueLabelXY activeValueLabels,
            string startingName,
            AccelCharts accelCharts):
            this(
                xBox,
                yBox,
                lockCheckBox,
                containingForm,
                defaultData,
                label,
                accelCharts,
                activeValueLabels)
        {
            SetName(startingName);
        }

        public FieldXY Fields { get; }

        public Label Label { get; }

        public ActiveValueLabelXY ActiveValueLabels { get; }

        public void SetName(string name)
        {
            Label.Text = name;
            //Label.Left = Convert.ToInt32((Fields.XField.Box.Left / 2.0) - (Label.Width / 2.0));   //Centered
            Label.Left = Convert.ToInt32(Fields.XField.Box.Left - Label.Width - 10);    //Right-aligned
        }

        public void SetActiveValues(double x, double y)
        {
            ActiveValueLabels.SetValues(x, y);
        }

        public void Hide()
        {
            Fields.Hide();
            Fields.LockCheckBox.Hide();
            Label.Hide();
        }

        public void Show()
        {
            Fields.Show();
            Fields.LockCheckBox.Show();
            Label.Show();
        }

        public void Show(string name)
        {
            SetName(name);

            Show();
        }

    }
}

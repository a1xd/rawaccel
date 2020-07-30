using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class Option
    {
        public Option(TextBox box, Label label)
        {
            Box = box;
            Label = label;
        }

        public TextBox Box { get; }

        public Label Label { get; }

        public void SetName(string name)
        {
            Label.Text = name;
            Label.Left = Convert.ToInt32((Box.Left / 2.0) - (Label.Width / 2.0));
        }

        public void Hide()
        {
            Box.Hide();
            Label.Hide();
        }

        public void Show()
        {
            Box.Show();
            Label.Show();
        }

        public void Show(string name)
        {
            SetName(name);

            Box.Show();
            Label.Show();
        }
    }
}

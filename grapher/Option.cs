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
    }
}

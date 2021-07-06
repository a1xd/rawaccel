using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.LUT
{
    public class LUTPointOption
    {
        public LUTPointOption(Form form)
        {
            FieldX = new Field(new System.Windows.Forms.TextBox(), form, 0);
            FieldY = new Field(new System.Windows.Forms.TextBox(), form, 0);
        }

        public double X { get => FieldX.Data; }

        public double Y { get => FieldY.Data; }

        public int Top
        {
            get
            {
                return FieldX.Top;
            }
            set
            {
                FieldX.Top = value;
                FieldY.Top = value;
            }
        }

        private Field FieldX { get; }

        private Field FieldY { get; }
    }
}

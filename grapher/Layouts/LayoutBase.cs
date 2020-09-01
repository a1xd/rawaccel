using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Layouts
{
    public abstract class LayoutBase
    {
        public const string Acceleration = "Acceleration";
        public const string Scale = "Scale";
        public const string Exponent = "Exponent";
        public const string Limit = "Limit";
        public const string Midpoint = "Midpoint";
        public const string Offset = "Offset";

        public LayoutBase()
        {
            ShowOptions = new bool[] { false, false, false, false };
            ShowOptionsXY = new bool[] { true, true };
            OptionNames = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };
            ButtonEnabled = true;
        }

        /// <summary>
        ///  Gets or sets mapping from acceleration type to identifying integer.
        ///  Must match accel_mode defined in rawaccel-settings.h
        /// </summary>
        public int Index { get; internal set; }

        public string Name { get; internal set; }

        internal bool[] ShowOptions { get; set; }

        internal bool[] ShowOptionsXY { get; set; }

        internal string[] OptionNames { get; set; }

        internal bool ButtonEnabled { get; set; }

        public void Layout(Option[] options, OptionXY[] optionsXY, Button button)
        {
            // Relies on AccelOptions to keep lengths correct.
            for (int i = 0; i< options.Length; i++)
            {
                if (ShowOptions[i])
                {
                    options[i].Show(OptionNames[i]);
                }
                else
                {
                    options[i].Hide();
                }
            }

            // Relies on AccelOptions to keep lengths correct.
            for (int i = 0; i< optionsXY.Length; i++)
            {
                if (ShowOptionsXY[i])
                {
                    optionsXY[i].Show();
                }
                else
                {
                    optionsXY[i].Hide();
                }
            }

            button.Enabled = ButtonEnabled;
        }
    }
}

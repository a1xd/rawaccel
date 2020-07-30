using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Layouts
{
    public abstract class LayoutBase
    {
        public const string Acceleration = "Acceleration";
        public const string Scale = "Scale";
        public const string Exponent = "Exponent";
        public const string Limit = "Limit";
        public const string Midpoint = "Midpoint";

        public LayoutBase()
        {
            Show = new bool[] { false, false, false };
            OptionNames = new string[] { string.Empty, string.Empty, string.Empty };
        }

        /// <summary>
        ///  Gets or sets mapping from acceleration type to identifying integer.
        ///  Must match order in tagged_union in rawaccel.hpp (which is 1-indexed, meaning 0 is off.)
        /// </summary>
        public int Index { get; internal set; }

        public string Name { get; internal set; }

        internal bool[] Show { get; set; }

        internal string[] OptionNames { get; set; }

        public void Layout(Option[] options)
        {
            // Relies on AccelOptions to keep lengths correct.
            for (int i = 0; i< options.Length; i++)
            {
                if (Show[i])
                {
                    options[i].Show(OptionNames[i]);
                }
                else
                {
                    options[i].Hide();
                }
            }

        }
    }
}

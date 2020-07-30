using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class AccelOptions
    {
        public const string Off = "Off";
        public const string Linear = "Linear";
        public const string Classic = "Classic";
        public const string Natural = "Natural";
        public const string Logarithmic = "Logarithmic";
        public const string Sigmoid = "Sigmoid";
        public const string Power = "Power";

        /// <summary>
        ///  Holds mapping from acceleration type to identifying integer.
        ///  Must match order in tagged_union in rawaccel.hpp (which is 1-indexed, meaning 0 is off.)
        /// </summary>
        public static readonly Dictionary<string, int> TypeToIndex = new Dictionary<string, int>
        {
            { Off, 0 },
            { Linear, 1 },
            { Classic, 2 },
            { Natural, 3 },
            { Logarithmic, 4 },
            { Sigmoid, 5 },
            { Power, 6 },
        };

        public AccelOptions(
            ComboBox accelDropdown,
            Option constOptionOne,
            Option constOptionTwo,
            Option constOptionThree
            )
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(TypeToIndex.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            ConstOptionOne = constOptionOne;
            ConstOptionTwo = constOptionTwo;
            ConstOptionThree = constOptionThree;
        }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex { get; private set; }

        public Option ConstOptionOne { get; }

        public Option ConstOptionTwo { get; }

        public Option ConstOptionThree { get; }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            var AccelerationType = AccelDropdown.SelectedItem.ToString(); 
            AccelerationIndex = TypeToIndex[AccelerationType];

            switch (AccelerationType)
            {
                case Linear:
                    LayoutLinear();
                    break;
                case Classic:
                    LayoutClassic();
                    break;
                case Natural:
                    LayoutNatural();
                    break;
                case Logarithmic:
                    LayoutLogarithmic();
                    break;
                case Sigmoid:
                    LayoutSigmoid();
                    break;
                case Power:
                    LayoutPower();
                    break;
                default:
                    LayoutDefault();
                    break;
            }
        }

        private void LayoutDefault()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Show();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Acceleration");
            ConstOptionTwo.SetName("Limit\\Exponent");
        }

        private void LayoutLinear()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Hide();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Acceleration");
        }

        private void LayoutClassic()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Show();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Acceleration");
            ConstOptionTwo.SetName("Exponent");
        }

        private void LayoutNatural()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Show();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Acceleration");
            ConstOptionOne.SetName("Limit");
        }

        private void LayoutLogarithmic()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Hide();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Acceleration");
        }


        private void LayoutSigmoid()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Show();
            ConstOptionThree.Show();

            ConstOptionOne.SetName("Acceleration");
            ConstOptionTwo.SetName("Limit");
            ConstOptionThree.SetName("Midpoint");
        }

        private void LayoutPower()
        {
            ConstOptionOne.Show();
            ConstOptionTwo.Show();
            ConstOptionThree.Hide();

            ConstOptionOne.SetName("Scale");
            ConstOptionTwo.SetName("Exponent");
        }
    }
}

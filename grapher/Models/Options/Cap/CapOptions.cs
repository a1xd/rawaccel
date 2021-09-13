using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.Cap
{
    public class CapOptions : OptionBase
    {
        public enum CapType
        {
            In,
            Out,
            Both,
        }

        public CapOptions(
            ComboBox capTypeDropDown,
            Option capIn,
            Option capOut,
            Option slope)
        {
            CapTypeDropdown = capTypeDropDown;
            In = capIn;
            Out = capOut;
            Slope = slope;

            SetupCapTypeDropdown(CapTypeDropdown);
            CapTypeDropdown.SelectedItem = CapType.In;
        }

        public ComboBox CapTypeDropdown { get; }

        public Option In { get; }

        public Option Out { get; }

        public Option Slope { get; }

        public CapType SelectedCapType { get; private set; }

        public override int Left
        {
            get => In.Left;

            set
            {
                In.Left = value;
                Out.Left = value;
                Slope.Left = value;
            }
        }

        public override int Top
        {
            get => CapTypeDropdown.Top;
            set
            {
                CapTypeDropdown.Top = value;
                Layout();
            }
        }

        public override int Height
        {
            get => BottomElement.Top + BottomElement.Height - CapTypeDropdown.Top;
        }

        public override int Width
        {
            get => CapTypeDropdown.Width;

            set
            {
                CapTypeDropdown.Width = value;
                In.Width = value;
                Out.Width = value;
                Slope.Width = value;
            }
        }

        public override bool Visible
        {
            get => CapTypeDropdown.Visible;
        }

        private Option BottomElement { get; set; }

        public void Layout()
        {
            Layout(CapTypeDropdown.Top + CapTypeDropdown.Height + Constants.OptionVerticalSeperation);
        }

        private void Layout(int top)
        {
            switch (SelectedCapType)
            {
                case CapType.In:
                    Slope.Show();
                    In.Show();
                    Out.Hide();

                    Slope.Top = top;
                    In.SnapTo(Slope);
                    BottomElement = In;
                    break;
                case CapType.Out:
                    Slope.Show();
                    In.Hide();
                    Out.Show();

                    Slope.Top = top;
                    In.SnapTo(Slope);
                    BottomElement = In;
                    break;
                case CapType.Both:
                    Slope.Hide();
                    In.Show();
                    Out.Show();

                    In.Top = top;
                    Out.SnapTo(In);
                    BottomElement = Out;
                    break;
            }
        }

        private void FindSelectedTypeFromDropdown()
        {
            SelectedCapType = (CapType)CapTypeDropdown.SelectedItem;
        }

        private void OnCapTypeDropdownSelectedItemChanged(object sender, EventArgs e)
        {
            FindSelectedTypeFromDropdown();
            Layout();
        }

        private void SetupCapTypeDropdown(ComboBox capTypeDropDown)
        {
            capTypeDropDown.Items.Clear();
            capTypeDropDown.DataSource = Enum.GetValues(typeof(CapType));
        }
    }
}

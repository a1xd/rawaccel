using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static grapher.Models.Options.Cap.CapTypeOptions;

namespace grapher.Models.Options.Cap
{
    public class CapOptions : OptionBase
    {

        public CapOptions(
            CapTypeOptions capTypeOptions,
            Option capIn,
            Option capOut,
            Option slope)
        {
            CapTypeOptions = capTypeOptions;
            In = capIn;
            Out = capOut;
            Slope = slope;
        }

        public CapTypeOptions CapTypeOptions { get; }

        public Option In { get; }

        public Option Out { get; }

        public Option Slope { get; }

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
            get => CapTypeOptions.Top;
            set
            {
                CapTypeOptions.Top = value;
                Layout();
            }
        }

        public override int Height
        {
            get => BottomElement.Top + BottomElement.Height - CapTypeOptions.Top;
        }

        public override int Width
        {
            get => CapTypeOptions.Width;

            set
            {
                CapTypeOptions.Width = value;
                In.Width = value;
                Out.Width = value;
                Slope.Width = value;
            }
        }

        public override bool Visible
        {
            get => CapTypeOptions.Visible;
        }

        private Option BottomElement { get; set; }

        public void Layout()
        {
            Layout(CapTypeOptions.Top + CapTypeOptions.Height + Constants.OptionVerticalSeperation);
        }

        private void Layout(int top)
        {
            switch (CapTypeOptions.SelectedCapType)
            {
                case CapType.Input:
                    Slope.Show();
                    In.Show();
                    Out.Hide();

                    Slope.Top = top;
                    In.SnapTo(Slope);
                    BottomElement = In;
                    break;
                case CapType.Output:
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

        private void OnCapTypeDropdownSelectedItemChanged(object sender, EventArgs e)
        {
            Layout();
        }

        private void SetupCapTypeDropdown(ComboBox capTypeDropDown)
        {
            capTypeDropDown.Items.Clear();
            capTypeDropDown.DataSource = Enum.GetValues(typeof(CapType));
        }
    }
}

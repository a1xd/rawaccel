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

            ShouldShow = true;
            TopElement = Slope;
            BottomElement = In;
            CapTypeOptions.OptionsDropdown.SelectedIndexChanged += OnCapTypeDropdownSelectedItemChanged;
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
            get => TopElement.Top;
            set
            {
                Layout(value);
            }
        }

        public override int Height
        {
            get => BottomElement.Top + BottomElement.Height - TopElement.Top;
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
            get => ShouldShow;
        }

        private bool ShouldShow { get; set; }

        private IOption BottomElement { get; set; }

        private IOption TopElement { get; set; }

        public override void AlignActiveValues()
        {
            Slope.AlignActiveValues();
            CapTypeOptions.AlignActiveValues();
            In.AlignActiveValues();
            Out.AlignActiveValues();
        }

        public override void Show(string name)
        {
            ShouldShow = true;
            Layout(Top, name);
        }

        public override void Hide()
        {
            ShouldShow = false;
            CapTypeOptions.Hide();
            Slope.Hide();
            In.Hide();
            Out.Hide();
        }

        public void SetActiveValues(
            double scale,
            double inCap,
            double outCap,
            ClassicCapMode capMode)
        {
            Slope.SetActiveValue(scale);
            In.SetActiveValue(inCap);
            Out.SetActiveValue(outCap);
            CapTypeOptions.SetActiveValue(capMode);
        }

        private void Layout(int top, string name = null)
        {
            switch (CapTypeOptions.SelectedCapType)
            {
                case CapType.Input:
                    if (ShouldShow)
                    {
                        Slope.Show();
                        CapTypeOptions.Show(name);
                        In.Show();
                        Out.Hide();
                    }

                    Slope.Top = top;
                    CapTypeOptions.SnapTo(Slope);
                    In.SnapTo(CapTypeOptions);

                    TopElement = CapTypeOptions;
                    BottomElement = In;
                    break;
                case CapType.Output:
                    if (ShouldShow)
                    {
                        Slope.Show();
                        CapTypeOptions.Show(name);
                        In.Hide();
                        Out.Show();
                    }

                    Slope.Top = top;
                    CapTypeOptions.SnapTo(Slope);
                    Out.SnapTo(CapTypeOptions);

                    TopElement = CapTypeOptions;
                    BottomElement = Out;
                    break;
                case CapType.Both:
                    if (ShouldShow)
                    {
                        CapTypeOptions.Show(name);
                        Slope.Hide();
                        In.Show();
                        Out.Show();
                    }

                    CapTypeOptions.Top = top;
                    In.SnapTo(CapTypeOptions);
                    Out.SnapTo(In);

                    TopElement = In;
                    BottomElement = Out;
                    break;
            }
        }

        private void OnCapTypeDropdownSelectedItemChanged(object sender, EventArgs e)
        {
            Layout(Top);
        }

        private void SetupCapTypeDropdown(ComboBox capTypeDropDown)
        {
            capTypeDropDown.Items.Clear();
            capTypeDropDown.DataSource = Enum.GetValues(typeof(CapType));
        }
    }
}

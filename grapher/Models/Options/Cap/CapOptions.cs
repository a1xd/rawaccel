using System;
using System.Windows.Forms;
using static grapher.Models.Options.Cap.CapTypeOptions;

namespace grapher.Models.Options.Cap
{
    public class CapOptions : OptionBase
    {
        #region Constants

        public const string InCapLabel = "Cap: Input";
        public const string OutCapLabel = "Cap: Output";

        #endregion Constants

        #region Fields

        private int _top;

        #endregion Fields

        #region Constructors

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
            _top = Slope.Top;
            BottomElement = In;
            CapTypeOptions.OptionsDropdown.SelectedIndexChanged += OnCapTypeDropdownSelectedItemChanged;
            CapTypeOptions.SelectedCapOption = InCap;
        }

        #endregion Constructors

        #region Properties

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
            get => _top;
            set
            {
                _top = value;
                Layout(value);
            }
        }

        public override int Height
        {
            get => BottomElement.Top + BottomElement.Height - Top;
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

        #endregion Properties

        #region Methods

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
            CapMode capMode)
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
                        ShowInCap();
                        Out.Hide();
                    }

                    Slope.Top = top;
                    CapTypeOptions.SnapTo(Slope);
                    In.SnapTo(CapTypeOptions);

                    BottomElement = In;
                    break;
                case CapType.Output:
                    if (ShouldShow)
                    {
                        Slope.Show();
                        CapTypeOptions.Show(name);
                        In.Hide();
                        ShowOutCap();
                    }

                    Slope.Top = top;
                    CapTypeOptions.SnapTo(Slope);
                    Out.SnapTo(CapTypeOptions);

                    BottomElement = Out;
                    break;
                case CapType.Both:
                    if (ShouldShow)
                    {
                        CapTypeOptions.Show(name);
                        Slope.Hide();
                        ShowInCap();
                        ShowOutCap();
                    }

                    CapTypeOptions.Top = top;
                    In.SnapTo(CapTypeOptions);
                    Out.SnapTo(In);

                    BottomElement = Out;
                    break;
            }
        }

        private void ShowInCap()
        {
            In.Show(InCapLabel);
        }

        private void ShowOutCap()
        {
            Out.Show(OutCapLabel);
        }

        private void OnCapTypeDropdownSelectedItemChanged(object sender, EventArgs e)
        {
            Layout(Top);
            CapTypeOptions.CheckIfDefault();
        }

        #endregion Methods
    }
}

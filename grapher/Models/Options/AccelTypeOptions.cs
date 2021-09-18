using grapher.Layouts;
using grapher.Models.Options;
using grapher.Models.Options.Cap;
using grapher.Models.Options.LUT;
using System;
using System.Windows.Forms;

namespace grapher
{
    public class AccelTypeOptions : OptionBase
    {
        #region Fields

        public static readonly LayoutBase Linear = new LinearLayout();
        public static readonly LayoutBase Classic = new ClassicLayout();
        public static readonly LayoutBase Jump = new JumpLayout();
        public static readonly LayoutBase Natural = new NaturalLayout();
        public static readonly LayoutBase Motivity = new MotivityLayout();
        public static readonly LayoutBase Power = new PowerLayout();
        public static readonly LayoutBase LUT = new LUTLayout();
        public static readonly LayoutBase Off = new OffLayout();

        #endregion Fields

        #region Constructors

        public AccelTypeOptions(
            ComboBox accelDropdown,
            CheckBoxOption gainSwitch,
            CapOptions classicCap,
            CapOptions powerCap,
            Option outputOffset,
            Option decayRate,
            Option growthRate,
            Option smooth,
            Option inputOffset,
            Option limit,
            Option powerClassic,
            Option exponent,
            Option midpoint,
            TextOption lutText,
            LUTPanelOptions lutPanelOptions,
            LutApplyOptions lutApplyOptions,
            Button writeButton,
            ActiveValueLabel accelTypeActiveValue)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(
                new LayoutBase[]
                {
                    Linear,
                    Classic,
                    Jump,
                    Natural,
                    Motivity,
                    Power,
                    LUT,
                    Off
                });

            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            GainSwitch = gainSwitch;
            DecayRate = decayRate;
            GrowthRate = growthRate;
            Smooth = smooth;
            ClassicCap = classicCap;
            PowerCap = powerCap;
            InputOffset = inputOffset;
            Limit = limit;
            PowerClassic = powerClassic;
            Exponent = exponent;
            OutputOffset = outputOffset;
            Midpoint = midpoint;
            WriteButton = writeButton;
            AccelTypeActiveValue = accelTypeActiveValue;
            LutText = lutText;
            LutPanel = lutPanelOptions;
            LutApply = lutApplyOptions;

            AccelTypeActiveValue.Left = AccelDropdown.Left + AccelDropdown.Width;
            AccelTypeActiveValue.Height = AccelDropdown.Height;
            GainSwitch.Left = DecayRate.Field.Left;

            LutPanel.Left = AccelDropdown.Left;
            LutPanel.Width = AccelDropdown.Width + AccelTypeActiveValue.Width;

            LutText.SetText(TextOption.LUTLayoutExpandedText, TextOption.LUTLayoutShortenedText);

            AccelerationType = Off;
            Layout();
            ShowingDefault = true;
        }

        #endregion Constructors

        #region Properties
        public AccelCharts AccelCharts { get; }

        public Button WriteButton { get; }

        public ComboBox AccelDropdown { get; }

        public ActiveValueLabel AccelTypeActiveValue { get; }

        public Option DecayRate { get; }

        public Option GrowthRate { get; }

        public Option Smooth { get; }

        public CapOptions ClassicCap { get; }

        public CapOptions PowerCap { get; }

        public Option InputOffset { get; }

        public Option OutputOffset { get; }

        public Option Limit { get; }

        /// <summary>
        /// This is the option for the power parameter for Classic style,
        /// and has nothing to do with the Power style.
        /// </summary>
        public Option PowerClassic { get; }

        public Option Exponent { get; }

        public Option Midpoint { get; }

        public TextOption LutText { get; }

        public CheckBoxOption GainSwitch { get; }

        public LUTPanelOptions LutPanel { get; }

        public LutApplyOptions LutApply { get; }

        public LayoutBase AccelerationType
        {
            get
            {
                return AccelDropdown.SelectedItem as LayoutBase;
            }
            private set
            {
                AccelDropdown.SelectedItem = value;
            }
        }

        public override int Top 
        {
            get
            {
                return AccelDropdown.Top;
            } 
            set
            {
                AccelDropdown.Top = value;
                AccelTypeActiveValue.Top = value;
                Layout(value + AccelDropdown.Height + Constants.OptionVerticalSeperation);
            }
        }

        public override int Height
        {
            get
            {
                return AccelDropdown.Height;
            } 
        }

        public override int Left
        {
            get
            {
                return AccelDropdown.Left;
            } 
            set
            {
                AccelDropdown.Left = value;
                LutText.Left = value;
                LutPanel.Left = value;
                LutApply.Left = value;
            }
        }

        public override int Width
        {
            get
            {
                return AccelDropdown.Width;
            }
            set
            {
                AccelDropdown.Width = value;
                LutText.Width = value;
                LutPanel.Width = AccelTypeActiveValue.CenteringLabel.Right - AccelDropdown.Left;
                LutApply.Width = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return AccelDropdown.Visible;
            }
        }

        private bool ShowingDefault { get; set; }

        #endregion Properties

        #region Methods

        public override void Hide()
        {
            AccelDropdown.Hide();
            AccelTypeActiveValue.Hide();

            GainSwitch.Hide();
            DecayRate.Hide();
            GrowthRate.Hide();
            Smooth.Hide();
            ClassicCap.Hide();
            PowerCap.Hide();
            OutputOffset.Hide();
            InputOffset.Hide();
            Limit.Hide();
            PowerClassic.Hide();
            Exponent.Hide();
            Midpoint.Hide();
            LutText.Hide();
            LutPanel.Hide();
            LutApply.Hide();
        }

        public void Show()
        {
            AccelDropdown.Show();
            AccelTypeActiveValue.Show();
            Layout(AccelDropdown.Bottom + Constants.OptionVerticalSeperation);
        }

        public override void Show(string name)
        {
            Show();
        }

        public void SetActiveValues(ref AccelArgs args)
        {
            AccelerationType = AccelTypeFromSettings(ref args);
            AccelTypeActiveValue.SetValue(AccelerationType.ActiveName);
            GainSwitch.SetActiveValue(args.gain);
            ClassicCap.SetActiveValues(
                args.acceleration,
                args.cap.x,
                args.cap.y,
                args.capMode);
            PowerCap.SetActiveValues(
                args.scale,
                args.cap.x,
                args.cap.y,
                args.capMode);
            OutputOffset.SetActiveValue(args.outputOffset);
            InputOffset.SetActiveValue(args.inputOffset);
            DecayRate.SetActiveValue(args.decayRate);
            GrowthRate.SetActiveValue(args.growthRate);
            Smooth.SetActiveValue(args.smooth);
            Limit.SetActiveValue(args.limit);
            PowerClassic.SetActiveValue(args.exponentClassic);
            Exponent.SetActiveValue(args.exponentPower);
            Midpoint.SetActiveValue(args.midpoint);
            LutPanel.SetActiveValues(args.data, args.length, args.mode);
            // TODO - use GainSwitch only?
            LutApply.SetActiveValue(args.gain);
        }

        public void ShowFull()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultFullText;
            }

            Left = DecayRate.Left + Constants.DropDownLeftSeparation;
            Width = DecayRate.Width - Constants.DropDownLeftSeparation;

            LutText.Expand();
            HandleLUTOptionsOnResize();
        }

        public void ShowShortened()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultShortText;
            }

            Left = DecayRate.Field.Left;
            Width = DecayRate.Field.Width;

            LutText.Shorten();
        }

        public void SetArgs(ref AccelArgs args)
        {
            args.mode = AccelerationType.Mode;
            args.gain = GainSwitch.CheckBox.Checked;

            if (DecayRate.Visible) args.decayRate = DecayRate.Field.Data;
            if (GrowthRate.Visible) args.growthRate = GrowthRate.Field.Data;
            if (Smooth.Visible) args.smooth = Smooth.Field.Data;
            if (ClassicCap.Visible)
            {
                args.acceleration = ClassicCap.Slope.Field.Data;
                args.cap.x = ClassicCap.In.Field.Data;
                args.cap.y = ClassicCap.Out.Field.Data;
                args.capMode = ClassicCap.CapTypeOptions.GetSelectedCapMode();
            }
            if (PowerCap.Visible)
            {
                args.scale = PowerCap.Slope.Field.Data;
                args.cap.x = PowerCap.In.Field.Data;
                args.cap.y = PowerCap.Out.Field.Data;
                args.capMode = PowerCap.CapTypeOptions.GetSelectedCapMode();
            }
            if (Limit.Visible) args.limit = Limit.Field.Data;
            if (PowerClassic.Visible) args.exponentClassic = PowerClassic.Field.Data;
            if (Exponent.Visible) args.exponentPower = Exponent.Field.Data;
            if (InputOffset.Visible) args.inputOffset = InputOffset.Field.Data;
            if (OutputOffset.Visible) args.outputOffset = OutputOffset.Field.Data;
            if (Midpoint.Visible) args.midpoint = Midpoint.Field.Data;
            if (LutPanel.Visible)
            {
                (var points, var length) = LutPanel.GetPoints();
                args.length = length * 2;

                for (int i = 0; i < length; i++)
                {
                    ref var p = ref points[i];
                    var data_idx = i * 2;
                    args.data[data_idx] = p.x;
                    args.data[data_idx + 1] = p.y;
                }
            }

        }

        public override void AlignActiveValues()
        {
            AccelTypeActiveValue.Align();
            GainSwitch.AlignActiveValues();
            DecayRate.AlignActiveValues();
            GrowthRate.AlignActiveValues();
            Smooth.AlignActiveValues();
            ClassicCap.AlignActiveValues();
            PowerCap.AlignActiveValues();
            OutputOffset.AlignActiveValues();
            InputOffset.AlignActiveValues();
            Limit.AlignActiveValues();
            PowerClassic.AlignActiveValues();
            Exponent.AlignActiveValues();
            Midpoint.AlignActiveValues();
            LutApply.AlignActiveValues();
        }

        public void HandleLUTOptionsOnResize()
        {
            LutText.Left = AccelDropdown.Left;
            LutPanel.Left = GainSwitch.Left - 100;
            LutPanel.Width = DecayRate.ActiveValueLabel.CenteringLabel.Right - LutPanel.Left;
            LutApply.Left = LutPanel.Left;
            LutApply.Width = AccelDropdown.Right - LutPanel.Left;
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            Layout(Beneath);
            ShowingDefault = false;
        }

        private void Layout(int top = -1)
        {
            if (top < 0)
            {
                top = GainSwitch.Top;
            }

            AccelerationType.Layout(
                GainSwitch,
                ClassicCap,
                PowerCap,
                DecayRate,
                GrowthRate,
                Smooth,
                InputOffset,
                Limit,
                PowerClassic,
                Exponent,
                OutputOffset,
                Midpoint,
                LutText,
                LutPanel,
                LutApply,
                top);
        }

        private LayoutBase AccelTypeFromSettings(ref AccelArgs args)
        { 
            switch (args.mode)
            {
                case AccelMode.classic:  return (args.exponentClassic == 2) ? Linear : Classic;
                case AccelMode.jump:     return Jump;
                case AccelMode.natural:  return Natural;
                case AccelMode.motivity: return Motivity;
                case AccelMode.power:    return Power;
                case AccelMode.lut:      return LUT;
                default:                 return Off;
            }
        }

        #endregion Methods
    }
}

using grapher.Layouts;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace grapher
{
    public class AccelTypeOptions : OptionBase
    {
        #region Fields

        public static readonly Dictionary<string, LayoutBase> AccelerationTypes = new List<LayoutBase>
        {
            new LinearLayout(),
            new ClassicLayout(),
            new NaturalLayout(),
            new JumpLayout(),
            new PowerLayout(),
            new MotivityLayout(),
            new LUTLayout(),
            new OffLayout(),
        }.ToDictionary(k => k.Name);

        public static readonly AccelArgs DefaultArgs = new DriverSettings().args.x;

        #endregion Fields

        #region Constructors

        public AccelTypeOptions(
            ComboBox accelDropdown,
            CheckBoxOption gainSwitch,
            Option acceleration,
            Option scale,
            CapOptions cap,
            Option weight,
            OffsetOptions offset,
            Option limit,
            Option exponent,
            Option midpoint,
            TextOption lutText,
            Button writeButton,
            ActiveValueLabel accelTypeActiveValue)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            GainSwitch = gainSwitch;
            Acceleration = acceleration;
            Scale = scale;
            Cap = cap;
            Weight = weight;
            Offset = offset;
            Limit = limit;
            Exponent = exponent;
            Midpoint = midpoint;
            WriteButton = writeButton;
            AccelTypeActiveValue = accelTypeActiveValue;
            LutText = lutText;

            AccelTypeActiveValue.Left = AccelDropdown.Left + AccelDropdown.Width;
            AccelTypeActiveValue.Height = AccelDropdown.Height;
            GainSwitch.Left = Acceleration.Field.Left;

            Layout("Off");
            ShowingDefault = true;
        }

        #endregion Constructors

        #region Properties
        public AccelCharts AccelCharts { get; }

        public Button WriteButton { get; }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex
        {
            get
            {
                return AccelerationType.Index;
            }
        }

        public LayoutBase AccelerationType { get; private set; }

        public ActiveValueLabel AccelTypeActiveValue { get; }

        public Option Acceleration { get; }

        public Option Scale { get; }

        public CapOptions Cap { get; }

        public Option Weight { get; }

        public OffsetOptions Offset { get; }

        public Option Limit { get; }

        public Option Exponent { get; }

        public Option Midpoint { get; }

        public TextOption LutText { get; }

        public CheckBoxOption GainSwitch { get; }

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
            Acceleration.Hide();
            Scale.Hide();
            Cap.Hide();
            Weight.Hide();
            Offset.Hide();
            Limit.Hide();
            Exponent.Hide();
            Midpoint.Hide();
            LutText.Hide();
        }

        public void Show()
        {
            AccelDropdown.Show();
            AccelTypeActiveValue.Show();
            Layout();
        }

        public override void Show(string name)
        {
            Show();
        }

        public void SetActiveValues(AccelArgs args)
        {
            AccelerationType = AccelTypeFromSettings(args);
            AccelTypeActiveValue.SetValue(AccelerationType.Name);
            AccelDropdown.SelectedIndex = AccelerationType.Index;

            Weight.SetActiveValue(args.weight);
            Cap.SetActiveValues(args.cap, args.legacy);
            Offset.SetActiveValue(args.offset);
            Acceleration.SetActiveValue(AccelerationParameterFromArgs(args));
            Scale.SetActiveValue(args.scale);
            Limit.SetActiveValue(args.limit);
            Exponent.SetActiveValue(args.exponent);
            Midpoint.SetActiveValue(args.midpoint);
        }

        public void ShowFull()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultFullText;
            }

            Left = Acceleration.Left + Constants.DropDownLeftSeparation;
            Width = Acceleration.Width - Constants.DropDownLeftSeparation;
        }

        public void ShowShortened()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultShortText;
            }

            Left = Acceleration.Field.Left;
            Width = Acceleration.Field.Width;
        }

        public void SetArgs(ref AccelArgs args)
        {
            args.mode = (AccelMode)AccelerationType.Index;
            if (Acceleration.Visible)
            {
                if (args.mode == AccelMode.natural)
                {
                    args.accelNatural = Acceleration.Field.Data;
                }
                else if (args.mode == AccelMode.motivity)
                {
                    args.accelMotivity = Acceleration.Field.Data;
                }
                else
                {
                    args.accelClassic = Acceleration.Field.Data;
                }

                args.smooth = Acceleration.Field.Data;
            }

            args.legacy = !GainSwitch.CheckBox.Checked;

            if (Scale.Visible) args.scale = Scale.Field.Data;
            if (Cap.Visible) args.cap = Cap.SensitivityCap;
            if (Limit.Visible) args.limit = Limit.Field.Data;
            if (Exponent.Visible) args.exponent = Exponent.Field.Data;
            if (Offset.Visible) args.offset = Offset.Offset;
            if (Midpoint.Visible) args.midpoint = Midpoint.Field.Data;
            if (Weight.Visible) args.weight = Weight.Field.Data;
        }

        public AccelArgs GenerateArgs()
        {
            AccelArgs args = new DriverSettings().args.x;
            SetArgs(ref args);
            return args;
        }

        public override void AlignActiveValues()
        {
            AccelTypeActiveValue.Align();
            Acceleration.AlignActiveValues();
            Scale.AlignActiveValues();
            Cap.AlignActiveValues();
            Offset.AlignActiveValues();
            Weight.AlignActiveValues();
            Limit.AlignActiveValues();
            Exponent.AlignActiveValues();
            Midpoint.AlignActiveValues();
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            var accelerationTypeString = AccelDropdown.SelectedItem.ToString();
            Layout(accelerationTypeString, Beneath);
            ShowingDefault = false;
        }

        private void Layout(string type, int top = -1)
        {
            AccelerationType = AccelerationTypes[type];
            Layout(top);
        }

        private void Layout(int top = -1)
        {
            if (top < 0)
            {
                top = GainSwitch.Top;
            }

            AccelerationType.Layout(
                GainSwitch,
                Acceleration,
                Scale,
                Cap,
                Weight,
                Offset,
                Limit,
                Exponent,
                Midpoint,
                LutText,
                top);
        }

        private LayoutBase AccelTypeFromSettings(AccelArgs args)
        {
            LayoutBase type;
            if (args.mode == AccelMode.classic && args.exponent == 2)
            {
                type = AccelerationTypes.Values.Where(t => string.Equals(t.Name, LinearLayout.LinearName)).FirstOrDefault();
            }
            else
            {
                int index = (int)args.mode;
                type = AccelerationTypes.Where(t => t.Value.Index == index).FirstOrDefault().Value;
            }

            return type;
        }

        private double AccelerationParameterFromArgs(AccelArgs args)
        {
            if (args.mode == AccelMode.motivity)
            {
                return args.accelMotivity;
            }
            else if (args.mode == AccelMode.natural)
            {
                return args.accelNatural;
            }
            else
            {
                return args.accelClassic;
            }
        }

        #endregion Methods
    }
}

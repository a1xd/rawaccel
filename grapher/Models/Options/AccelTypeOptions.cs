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
            new PowerLayout(),
            new NaturalGainLayout(),
            new SigmoidGainLayout(),
            new OffLayout()
        }.ToDictionary(k => k.Name);

        #endregion Fields

        #region Constructors

        public AccelTypeOptions(
            ComboBox accelDropdown,
            Option acceleration,
            CapOptions cap,
            Option weight,
            OffsetOptions offset,
            Option limitOrExponent,
            Option midpoint,
            Button writeButton,
            ActiveValueLabel accelTypeActiveValue)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            Acceleration = acceleration;
            Cap = cap;
            Weight = weight;
            Offset = offset;
            LimitOrExponent = limitOrExponent;
            Midpoint = midpoint;
            WriteButton = writeButton;
            AccelTypeActiveValue = accelTypeActiveValue;

            AccelTypeActiveValue.Left = AccelDropdown.Left + AccelDropdown.Width;
            AccelTypeActiveValue.Height = AccelDropdown.Height;

            Layout("Off");
            ShowingDefault = true;
        }

        #endregion Constructors

        #region Properties

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

        public CapOptions Cap { get; }

        public Option Weight { get; }

        public OffsetOptions Offset { get; }

        public Option LimitOrExponent { get; }

        public Option Midpoint { get; }

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

            Acceleration.Hide();
            Cap.Hide();
            Weight.Hide();
            Offset.Hide();
            LimitOrExponent.Hide();
            Midpoint.Hide();
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

        public void SetActiveValues(int index, AccelArgs args)
        {
            var name = AccelerationTypes.Where(t => t.Value.Index == index).FirstOrDefault().Value.Name;
            AccelTypeActiveValue.SetValue(name);

            Weight.SetActiveValue(args.weight);
            Cap.SetActiveValues(args.gainCap, args.scaleCap, args.gainCap > 0);
            Offset.SetActiveValue(args.offset, args.legacy_offset);
            Acceleration.SetActiveValue(args.accel);
            LimitOrExponent.SetActiveValue(args.exponent);
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
            args.accel = Acceleration.Field.Data;
            args.rate = Acceleration.Field.Data;
            args.powerScale = Acceleration.Field.Data;
            args.gainCap = Cap.VelocityGainCap;
            args.scaleCap = Cap.SensitivityCap;
            args.limit = LimitOrExponent.Field.Data;
            args.exponent = LimitOrExponent.Field.Data;
            args.powerExponent = LimitOrExponent.Field.Data;
            args.offset = Offset.Offset;
            args.legacy_offset = Offset.LegacyOffset;
            args.midpoint = Midpoint.Field.Data;
            args.weight = Weight.Field.Data;
        }

        public AccelArgs GenerateArgs()
        {
            AccelArgs args = new AccelArgs();
            SetArgs(ref args);
            return args;
        }

        public override void AlignActiveValues()
        {
            AccelTypeActiveValue.Align();
            Acceleration.AlignActiveValues();
            Cap.AlignActiveValues();
            Offset.AlignActiveValues();
            Weight.AlignActiveValues();
            LimitOrExponent.AlignActiveValues();
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
                top = Acceleration.Top;
            }

            AccelerationType.Layout(
                Acceleration,
                Cap,
                Weight,
                Offset,
                LimitOrExponent,
                Midpoint,
                WriteButton,
                top);
        }

        #endregion Methods
    }
}

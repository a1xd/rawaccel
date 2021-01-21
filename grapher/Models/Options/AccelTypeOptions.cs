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
            new NaturalGainLayout(),
            new PowerLayout(),
            new MotivityLayout(),
            new OffLayout()
        }.ToDictionary(k => k.Name);

        #endregion Fields

        #region Constructors

        public AccelTypeOptions(
            ComboBox accelDropdown,
            Option acceleration,
            Option scale,
            CapOptions cap,
            Option weight,
            OffsetOptions offset,
            Option limit,
            Option exponent,
            Option midpoint,
            Button writeButton,
            ActiveValueLabel accelTypeActiveValue)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

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

            AccelTypeActiveValue.Left = AccelDropdown.Left + AccelDropdown.Width;
            AccelTypeActiveValue.Height = AccelDropdown.Height;

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
            Scale.Hide();
            Cap.Hide();
            Weight.Hide();
            Offset.Hide();
            Limit.Hide();
            Exponent.Hide();
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
            AccelerationType = AccelerationTypes.Where(t => t.Value.Index == index).FirstOrDefault().Value;
            AccelTypeActiveValue.SetValue(AccelerationType.Name);
            AccelDropdown.SelectedIndex = AccelerationType.Index;

            Weight.SetActiveValue(args.weight);
            Cap.SetActiveValues(args.gainCap, args.scaleCap, args.gainCap > 0 || args.scaleCap <= 0);
            Offset.SetActiveValue(args.offset, args.legacyOffset);
            Acceleration.SetActiveValue(args.acceleration);
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
            AccelArgs defaults = DriverInterop.DefaultSettings.args.x;
            args.acceleration = Acceleration.Visible ? Acceleration.Field.Data : defaults.acceleration;
            args.scale = Scale.Visible ? Scale.Field.Data : defaults.scale;
            args.gainCap = Cap.Visible ? Cap.VelocityGainCap : defaults.gainCap;
            args.scaleCap = Cap.Visible ? Cap.SensitivityCap : defaults.scaleCap;
            args.limit = Limit.Visible ? Limit.Field.Data : defaults.limit;
            args.exponent = Exponent.Visible ? Exponent.Field.Data : defaults.exponent;
            args.offset = Offset.Visible ? Offset.Offset : defaults.offset;
            args.legacyOffset = Offset.IsLegacy;
            args.midpoint = Midpoint.Visible ? Midpoint.Field.Data : defaults.midpoint;
            args.weight = Weight.Visible ? Weight.Field.Data : defaults.weight;
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
                top = Acceleration.Top;
            }

            AccelerationType.Layout(
                Acceleration,
                Scale,
                Cap,
                Weight,
                Offset,
                Limit,
                Exponent,
                Midpoint,
                top);
        }

        #endregion Methods
    }
}

using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class AccelOptionSet
    {
        public AccelOptionSet(
            Label titleLabel,
            int topAnchor,
            AccelTypeOptions accelTypeOptions,
            Option acceleration,
            CapOptions cap,
            Option weight,
            Option offset,
            Option limitOrExp,
            Option midpoint)
        {
            TitleLabel = titleLabel;
            TopAnchor = topAnchor;
            AccelTypeOptions = accelTypeOptions;
            Acceleration = acceleration;
            Cap = cap;
            Weight = weight;
            Offset = offset;
            LimitOrExponent = limitOrExp;
            Midpoint = midpoint;

            AccelTypeOptions.ShowFullText();

            TitleLabel.Top = TopAnchor;
        }

        public int TopAnchor { get; }

        public Label TitleLabel { get; }

        public AccelTypeOptions AccelTypeOptions { get; }

        public Option Acceleration { get; }

        public CapOptions Cap { get; }

        public Option Weight { get; }

        public Option Offset { get; }

        public Option LimitOrExponent { get; }

        public Option Midpoint { get; }

        public bool IsTitleMode { get; private set; }

        public void SetRegularMode()
        {
            if (IsTitleMode)
            {
                IsTitleMode = false;

                HideTitle();
                AccelTypeOptions.Left = Acceleration.Left;
                AccelTypeOptions.Width = Acceleration.Width;
                AccelTypeOptions.ShowFullText();
            }
        }

        public void SetTitleMode()
        {
            if (!IsTitleMode)
            {
                IsTitleMode = true;

                AccelTypeOptions.Left = Acceleration.Field.Left;
                AccelTypeOptions.Width = Acceleration.Field.Width;
                AccelTypeOptions.ShowFullText();
                DisplayTitle();
            }
        }

        public void Hide()
        {
            TitleLabel.Hide();
            AccelTypeOptions.Hide();
            Acceleration.Hide();
            Cap.Hide();
            Weight.Hide();
            Offset.Hide();
            LimitOrExponent.Hide();
            Midpoint.Hide();
        }

        public void Show()
        {
            if (IsTitleMode)
            {
                TitleLabel.Show();
            }

            AccelTypeOptions.Show();
            Acceleration.Show();
            Cap.Show();
            Weight.Show();
            Offset.Show();
            LimitOrExponent.Show();
            Midpoint.Show();
        }

        public void DisplayTitle()
        {
            TitleLabel.Show();

            SetOptionsTop(TitleLabel.Top + TitleLabel.Height + Constants.OptionVerticalSeperation);
        }

        public void HideTitle()
        {
            TitleLabel.Hide();

            SetOptionsTop(TopAnchor);
        }

        public void SetArgs(AccelArgs args)
        {
            args.accel = Acceleration.Field.Data;
            args.rate = Acceleration.Field.Data;
            args.powerScale = Acceleration.Field.Data;
            args.gainCap = Cap.VelocityGainCap;
            args.scaleCap = Cap.SensitivityCap;
            args.limit = LimitOrExponent.Field.Data;
            args.exponent = LimitOrExponent.Field.Data;
            args.powerExponent = LimitOrExponent.Field.Data;
            args.offset = Offset.Field.Data;
            args.midpoint = Midpoint.Field.Data;
            args.weight = Weight.Field.Data;
        }

        public AccelArgs GenerateArgs()
        {
            AccelArgs args = new AccelArgs();
            SetArgs(args);
            return args;
        }

        public void SetActiveValues(int mode, AccelArgs args)
        {
            AccelTypeOptions.SetActiveValue(mode);
            Weight.SetActiveValue(args.weight);
            Cap.SetActiveValues(args.gainCap, args.scaleCap, args.gainCap > 0);
            Offset.SetActiveValue(args.offset);
            Acceleration.SetActiveValue(args.accel);
            LimitOrExponent.SetActiveValue(args.exponent);
            Midpoint.SetActiveValue(args.midpoint);
        }

        private void SetOptionsTop(int top)
        {
            AccelTypeOptions.Top = top;
            Acceleration.Top = AccelTypeOptions.Top + AccelTypeOptions.Height + Constants.OptionVerticalSeperation;
            Cap.SnapTo(Acceleration);
            Weight.SnapTo(Cap);
            Offset.SnapTo(Weight);
            LimitOrExponent.SnapTo(Offset);
            Midpoint.SnapTo(LimitOrExponent);
        }
    }
}

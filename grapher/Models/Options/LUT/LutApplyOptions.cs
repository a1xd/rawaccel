using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.LUT
{
    public class LutApplyOptions : OptionBase
    {
        public const string LUTApplyOptionsLabelText = "Apply as:";

        public enum LutApplyType
        {
            Sensitivity,
            Velocity
        }

        public class LutApplyOption
        {
            public LutApplyType Type { get; set; }

            public string Name => Type.ToString();
        }

        public static readonly LutApplyOption Sensitivity = new LutApplyOption
        {
            Type = LutApplyType.Sensitivity,
        };

        public static readonly LutApplyOption Velocity = new LutApplyOption
        {
            Type = LutApplyType.Velocity,
        };

        public LutApplyOptions(
            Label label,
            ComboBox applyOptionsDropdown,
            ActiveValueLabel lutApplyActiveValue)
        {
            ApplyOptions = applyOptionsDropdown;
            ApplyOptions.Items.Clear();
            ApplyOptions.Items.AddRange(
                new LutApplyOption[]
                {
                    Sensitivity,
                    Velocity,
                });

            Label = label;
            Label.Text = LUTApplyOptionsLabelText;

            ActiveValueLabel = lutApplyActiveValue;
        }

        public LutApplyType ApplyType { get => ApplyOption.Type; }

        public LutApplyOption ApplyOption {
            get
            {
                return ApplyOptions.SelectedItem as LutApplyOption;
            }
            set
            {
                ApplyOptions.SelectedItem = value;
            }
        }

        public Label Label { get; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public ComboBox ApplyOptions { get; }

        public override bool Visible
        {
            get
            {
                return Label.Visible || ShouldShow;
            }
        }

        public override int Left
        {
            get
            {
                return Label.Left;
            }
            set
            {
                Label.Left = value;
                ApplyOptions.Left = Label.Left + Label.Width + Constants.OptionLabelBoxSeperation;
            }
        }

        public override int Height
        {
            get
            {
                return Label.Height;
            }
        }

        public override int Top
        {
            get
            {
                return Label.Top;
            }
            set
            {
                Label.Top = value;
                ApplyOptions.Top = value;
                ActiveValueLabel.Top = value;
            }
        }

        public override int Width
        {
            get
            {
                return Label.Width;
            }
            set
            {
                ApplyOptions.Width = value - Label.Width - Constants.OptionLabelBoxSeperation;
            }
        }

        private bool ShouldShow { get; set; }

        public override void Hide()
        {
            Label.Hide();
            ApplyOptions.Hide();
            ShouldShow = false;
        }

        public override void Show(string labelText)
        {
            Label.Show();
            
            if (!string.IsNullOrWhiteSpace(labelText))
            {
                Label.Text = labelText;
            }

            ApplyOptions.Show();
            ShouldShow = true;
        }

        public override void AlignActiveValues()
        {
            ActiveValueLabel.Align();
        }

        public void SetActiveValue(bool applyAsVelocity)
        {
            ApplyOption = ApplyOptionFromSettings(applyAsVelocity);
            ActiveValueLabel.SetValue(ApplyOption.Name);
        }

        public LutApplyOption ApplyOptionFromSettings(bool applyAsVelocity)
        {
            if (applyAsVelocity)
            {
                return Velocity;
            }
            else
            {
                return Sensitivity;
            }
        }
    }
}

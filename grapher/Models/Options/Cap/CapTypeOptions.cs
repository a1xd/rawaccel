using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.Cap
{
    public class CapTypeOptions : ComboBoxOptionsBase
    {
        #region Enum

        public enum CapType
        {
            Input,
            Output,
            Both,
        }

        #endregion Enum

        #region Classes

        public class CapTypeOption
        {
            public CapType Type { get; set; }

            public string Name => Type.ToString();

            public override string ToString() => Name;
        }

        #endregion Classes

        #region Static

        public static readonly CapTypeOption InCap = new CapTypeOption
        {
            Type = CapType.Input,
        };

        public static readonly CapTypeOption OutCap = new CapTypeOption
        {
            Type = CapType.Output,
        };

        public static readonly CapTypeOption BothCap = new CapTypeOption
        {
            Type = CapType.Both,
        };

        public static readonly CapTypeOption[] AllCapTypeOptions = new CapTypeOption[]
        {
            InCap,
            OutCap,
            BothCap
        };

        #endregion Static

        #region Constructors

        public CapTypeOptions(
            Label label,
            ComboBox dropdown,
            ActiveValueLabel activeValueLabel,
            int left)
            : base(
                  label,
                  dropdown,
                  activeValueLabel)
        {
            OptionsDropdown.Items.AddRange(
                new CapTypeOption[]
                {
                    InCap,
                    OutCap,
                    BothCap
                });

            Default = OutCap;

            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label.Left = left;
            label.Width = OptionsDropdown.Left - left - Constants.OptionLabelBoxSeperation;
            label.Height = OptionsDropdown.Height;
        }

        #endregion Constructors

        #region Properties

        public CapType SelectedCapType => SelectedCapOption.Type;

        public CapTypeOption SelectedCapOption
        {
            get
            {
                return OptionsDropdown.SelectedItem as CapTypeOption;
            }
            set
            {
                OptionsDropdown.SelectedItem = value;
            }
        }

        private CapTypeOption Default { get; set; }

        public CapMode GetSelectedCapMode()
        {
            switch(SelectedCapType)
            {
                case CapType.Output: return CapMode.output;
                case CapType.Both: return CapMode.in_out;
                case CapType.Input:
                default: return CapMode.input;
            }
        }

        #endregion Properties

        #region Methods

        public static CapTypeOption CapTypeOptionFromSettings(CapMode capMode)
        {
            switch (capMode)
            {
                case CapMode.output:
                    return OutCap;
                case CapMode.in_out:
                    return BothCap;
                case CapMode.input:
                default:
                    return InCap;
            }
        }

        public void SetActiveValue(CapMode capMode)
        {
            Default = CapTypeOptionFromSettings(capMode);
            SelectedCapOption = Default;
            ActiveValueLabel.SetValue(SelectedCapOption.Name);
        }

        public void CheckIfDefault()
        {
            if (SelectedCapOption.Equals(Default))
            {
                OptionsDropdown.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                OptionsDropdown.ForeColor = System.Drawing.Color.Black;
            }
        }

        #endregion Methods
    }
}

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
            ActiveValueLabel activeValueLabel)
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

        public ClassicCapMode GetSelectedCapMode()
        {
            switch(SelectedCapType)
            {
                case CapType.Output: return ClassicCapMode.output;
                case CapType.Both: return ClassicCapMode.in_out;
                case CapType.Input:
                default: return ClassicCapMode.input;
            }
        }

        #endregion Properties

        #region Methods

        public static CapTypeOption CapTypeOptionFromSettings(ClassicCapMode capMode)
        {
            switch (capMode)
            {
                case ClassicCapMode.output:
                    return OutCap;
                case ClassicCapMode.in_out:
                    return BothCap;
                case ClassicCapMode.input:
                default:
                    return InCap;
            }
        }

        public void SetActiveValue(ClassicCapMode capMode)
        {
            SelectedCapOption = CapTypeOptionFromSettings(capMode);
            ActiveValueLabel.SetValue(SelectedCapOption.Name);
        }

        #endregion Methods
    }
}

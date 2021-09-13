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
            In,
            Out,
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
            Type = CapType.In,
        };

        public static readonly CapTypeOption OutCap = new CapTypeOption
        {
            Type = CapType.Out,
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
        }

        #endregion Constructors

        #region Properties

        CapTypeOption CapOption
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

        #endregion Properties
    }
}

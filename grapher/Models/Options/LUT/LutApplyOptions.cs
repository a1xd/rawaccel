using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options.LUT
{
    public class LutApplyOptions : ComboBoxOptionsBase
    {
        #region Enum

        public enum LutApplyType
        {
            Sensitivity,
            Velocity
        }

        #endregion Enum

        #region Classes

        public class LutApplyOption
        {
            public LutApplyType Type { get; set; }

            public string Name => Type.ToString();

            public override string ToString() => Name;
        }

        #endregion Classes

        #region Static

        public static readonly LutApplyOption Sensitivity = new LutApplyOption
        {
            Type = LutApplyType.Sensitivity,
        };

        public static readonly LutApplyOption Velocity = new LutApplyOption
        {
            Type = LutApplyType.Velocity,
        };

        #endregion Static

        #region Constructors

        public LutApplyOptions(
            Label label,
            ComboBox applyOptionsDropdown,
            ActiveValueLabel lutApplyActiveValue)
            : base(
                  label,
                  applyOptionsDropdown,
                  lutApplyActiveValue)
        {
            OptionsDropdown.Items.AddRange(
                new LutApplyOption[]
                {
                    Sensitivity,
                    Velocity,
                });
        }

        #endregion Constructors

        #region Properties

        public LutApplyType ApplyType { get => ApplyOption.Type; }

        public LutApplyOption ApplyOption {
            get
            {
                return OptionsDropdown.SelectedItem as LutApplyOption;
            }
            set
            {
                OptionsDropdown.SelectedItem = value;
            }
        }

        #endregion Properties

        #region Methods

        public static LutApplyOption ApplyOptionFromSettings(bool applyAsVelocity)
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

        public void SetActiveValue(bool applyAsVelocity)
        {
            ApplyOption = ApplyOptionFromSettings(applyAsVelocity);
            ActiveValueLabel.SetValue(ApplyOption.Name);
        }

        #endregion Methods
    }
}

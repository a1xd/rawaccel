using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    /// <summary>
    /// This is an option type that is a regular option with a checkbox that disables it.
    /// </summary>
    public class LockableOption : OptionBase
    {
        public LockableOption(
            Option option,
            CheckBox checkBox,
            int lockedvalue)
        {
            Option = option;
            LockBox = checkBox;
            LockedValue = lockedvalue;
        }

        public Option Option { get; }

        public CheckBox LockBox { get; }

        public int LockedValue { get; }

        public override int Left
        {
            get => Option.Left;

            set
            {
                Option.Left = value;
            }
        }

        public override int Top
        {
            get => Option.Top;

            set
            {
                Option.Top = value;
                LockBox.Top = value;
            }
        }

        public override int Width
        {
            get => Option.Width;

            set
            {
                Option.Width = value;
            }
        }

        public override int Height
        {
            get => Option.Height;
        }

        public override bool Visible
        {
            get => Option.Visible;
        }

        public override void AlignActiveValues()
        {
            Option.AlignActiveValues();
        }

        public override void Hide()
        {
            Option.Hide();
            LockBox.Hide();
        }

        public override void Show(string Name)
        {
            Option.Show(Name);
            LockBox.Show();
        }
    }
}

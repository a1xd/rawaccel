using grapher.Common;

namespace grapher.Models.Options
{
    public abstract class OptionBase : IOption
    {
        public abstract int Top { get; set; }

        public abstract int Height { get; }
        
        public abstract int Left { get; set; }

        public abstract int Width { get; set; }

        public int Beneath { 
            get
            {
                return Top + Height + Constants.OptionVerticalSeperation;
            }
        }

        public abstract bool Visible { get; }

        public abstract void Show(string Name);

        public abstract void Hide();

        public abstract void AlignActiveValues();

        public virtual void SnapTo(IOption option)
        {
            Top = option.Beneath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Options
{
    public interface IOption
    {
        int Top { get; set; }

        int Height { get; }

        int Left { get; }

        int Width { get; }
        
        bool Visible { get; }

        void Show(string name);

        void Hide();

        void SnapTo(IOption option);
    }
}

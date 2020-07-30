using grapher.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class AccelOptions
    {
        public const int PossibleOptionsCount = 3;

        public static readonly Dictionary<string, LayoutBase> AccelerationTypes = new List<LayoutBase>
        {
            new DefaultLayout(),
            new LinearLayout(),
            new ClassicLayout(),
            new NaturalLayout(),
            new LogLayout(),
            new SigmoidLayout(),
            new PowerLayout(),
        }.ToDictionary(k => k.Name);

        public AccelOptions(
            ComboBox accelDropdown,
            Option[] options)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            if (options.Length > PossibleOptionsCount)
            {
                throw new Exception("Layout given too many options.");
            }

            Options = options;
        }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex { get; private set; }

        public Option[] Options { get; }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            var AccelerationTypeString = AccelDropdown.SelectedItem.ToString(); 
            var AccelerationType = AccelerationTypes[AccelerationTypeString];
            AccelerationIndex = AccelerationType.Index;
            AccelerationType.Layout(Options);
        }
    }
}

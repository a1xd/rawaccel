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
        public const int PossibleOptionsCount = 4;
        public const int PossibleOptionsXYCount = 2;

        public static readonly Dictionary<string, LayoutBase> AccelerationTypes = new List<LayoutBase>
        {
            new DefaultLayout(),
            new LinearLayout(),
            new ClassicLayout(),
            new NaturalLayout(),
            new LogLayout(),
            new SigmoidLayout(),
            new PowerLayout(),
            new NaturalGainLayout(),
            new OffLayout()
        }.ToDictionary(k => k.Name);

        public AccelOptions(
            ComboBox accelDropdown,
            Option[] options,
            OptionXY[] optionsXY,
            Button writeButton)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.Skip(1).ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            if (options.Length > PossibleOptionsCount)
            {
                throw new Exception("Layout given too many options.");
            }

            if (optionsXY.Length > PossibleOptionsXYCount)
            {
                throw new Exception("Layout given too many options.");
            }

            Options = options;
            OptionsXY = optionsXY;
            WriteButton = writeButton;

            Layout("Default");
        }

        public Button WriteButton { get; }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex { get; private set; }

        public Option[] Options { get; }

        public OptionXY[] OptionsXY { get; }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            var accelerationTypeString = AccelDropdown.SelectedItem.ToString();
            Layout(accelerationTypeString);
        }

        private void Layout(string type)
        {
            var accelerationType = AccelerationTypes[type];
            AccelerationIndex = accelerationType.Index;
            accelerationType.Layout(Options, OptionsXY, WriteButton);
        }
    }
}

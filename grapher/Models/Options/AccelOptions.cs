using grapher.Layouts;
using grapher.Models.Options;
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
        #region Constants

        public const int PossibleOptionsCount = 4;
        public const int PossibleOptionsXYCount = 2;

        #endregion Constants

        #region Fields

        public static readonly Dictionary<string, LayoutBase> AccelerationTypes = new List<LayoutBase>
        {
            new LinearLayout(),
            new ClassicLayout(),
            new NaturalLayout(),
            new LogLayout(),
            new SigmoidLayout(),
            new PowerLayout(),
            new NaturalGainLayout(),
            new SigmoidGainLayout(),
            new OffLayout()
        }.ToDictionary(k => k.Name);

        #endregion Fields

        #region Constructors

        public AccelOptions(
            ComboBox accelDropdown,
            Option[] options,
            OptionXY[] optionsXY,
            Button writeButton,
            ActiveValueLabel activeValueLabel)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
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
            ActiveValueLabel = activeValueLabel;

            Layout("Off");
        }

        #endregion Constructors

        #region Properties

        public Button WriteButton { get; }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex { get; private set; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public Option[] Options { get; }

        public OptionXY[] OptionsXY { get; }

        #endregion Properties

        #region Methods

        public void SetActiveValue(int index)
        {
            var name = AccelerationTypes.Where(t => t.Value.Index == index).FirstOrDefault().Value.Name;
            ActiveValueLabel.SetValue(name);
        }

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

        #endregion Methods
    }
}

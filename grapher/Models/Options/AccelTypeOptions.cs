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
    public class AccelTypeOptions
    {
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

        public AccelTypeOptions(
            ComboBox accelDropdown,
            Option[] options,
            Button writeButton,
            ActiveValueLabel activeValueLabel)
        {
            AccelDropdown = accelDropdown;
            AccelDropdown.Items.Clear();
            AccelDropdown.Items.AddRange(AccelerationTypes.Keys.ToArray());
            AccelDropdown.SelectedIndexChanged += new System.EventHandler(OnIndexChanged);

            if (options.Length > Constants.PossibleOptionsCount)
            {
                throw new Exception("Layout given too many options.");
            }

            Options = options;
            WriteButton = writeButton;
            ActiveValueLabel = activeValueLabel;

            Layout("Off");
            ShowingDefault = true;
        }

        #endregion Constructors

        #region Properties

        public Button WriteButton { get; }

        public ComboBox AccelDropdown { get; }

        public int AccelerationIndex
        {
            get
            {
                return AccelerationType.Index;
            }
        }

        public LayoutBase AccelerationType { get; private set; }

        public ActiveValueLabel ActiveValueLabel { get; }

        public Option[] Options { get; }

        public int Top 
        {
            get
            {
                return AccelDropdown.Top;
            } 
            set
            {
                AccelDropdown.Top = value;
            }
        }

        public int Height
        {
            get
            {
                return AccelDropdown.Height;
            } 
            set
            {
                AccelDropdown.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return AccelDropdown.Left;
            } 
            set
            {
                AccelDropdown.Left = value;
            }
        }

        public int Width
        {
            get
            {
                return AccelDropdown.Width;
            }
            set
            {
                AccelDropdown.Width = value;
            }
        }

        private bool ShowingDefault { get; set; }

        #endregion Properties

        #region Methods

        public void Hide()
        {
            AccelDropdown.Hide();
            
            foreach(var option in Options)
            {
                option.Hide();
            }
        }

        public void Show()
        {
            AccelDropdown.Show();
            Layout();
        }

        public void SetActiveValue(int index)
        {
            var name = AccelerationTypes.Where(t => t.Value.Index == index).FirstOrDefault().Value.Name;
            ActiveValueLabel.SetValue(name);
        }

        public void ShowFullText()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultFullText;
            }
        }

        public void ShowShortenedText()
        {
            if (ShowingDefault)
            {
                AccelDropdown.Text = Constants.AccelDropDownDefaultShortText;
            }
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            var accelerationTypeString = AccelDropdown.SelectedItem.ToString();
            Layout(accelerationTypeString);
            ShowingDefault = false;
        }

        private void Layout(string type)
        {
            AccelerationType = AccelerationTypes[type];
            Layout();
        }

        private void Layout()
        {
            AccelerationType.Layout(Options, WriteButton);
        }

        #endregion Methods
    }
}

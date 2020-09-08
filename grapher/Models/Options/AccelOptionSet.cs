using grapher.Models.Serialized;
using System.Drawing;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class AccelOptionSet
    {
        public AccelOptionSet(
            Label title,
            Label activeValuesTitle,
            int topAnchor,
            AccelTypeOptions accelTypeOptions)
        {
            OptionsTitle = title;
            ActiveValuesTitle = activeValuesTitle;
            TopAnchor = topAnchor;
            Options = accelTypeOptions;

            ActiveValuesTitle.AutoSize = false;
            ActiveValuesTitle.TextAlign = ContentAlignment.MiddleCenter;

            Options.ShowFull();

            OptionsTitle.Top = TopAnchor;
            IsTitleMode = true;
            SetRegularMode();
        }

        public int TopAnchor { get; }

        public Label OptionsTitle { get; }

        public Label ActiveValuesTitle { get; }

        public AccelTypeOptions Options { get; }


        public bool IsTitleMode { get; private set; }

        public void SetRegularMode()
        {
            if (IsTitleMode)
            {
                IsTitleMode = false;

                HideTitle();
                Options.ShowFull();
            }
        }

        public void SetTitleMode(string title)
        {
            OptionsTitle.Text = title;

            if (!IsTitleMode)
            {
                IsTitleMode = true;

                Options.ShowShortened();
                DisplayTitle();
            }
        }

        public void Hide()
        {
            OptionsTitle.Hide();
            ActiveValuesTitle.Hide();
            Options.Hide();
        }

        public void Show()
        {
            if (IsTitleMode)
            {
                OptionsTitle.Show();
            }

            ActiveValuesTitle.Show();
            Options.Show();
        }

        public void DisplayTitle()
        {
            OptionsTitle.Show();

            Options.Top = OptionsTitle.Top + OptionsTitle.Height + Constants.OptionVerticalSeperation;
        }

        public void HideTitle()
        {
            OptionsTitle.Hide();

            Options.Top = TopAnchor;
        }

        public void SetArgs(ref AccelArgs args)
        {
            Options.SetArgs(ref args);
        }

        public AccelArgs GenerateArgs()
        {
            return Options.GenerateArgs();
        }

        public void SetActiveValues(int mode, AccelArgs args)
        {
            Options.SetActiveValues(mode, args);
        }

        public void AlignActiveValuesByTitle()
        {
            Options.AlignActiveValues(ActiveValuesTitle.Width);
        }
    }
}

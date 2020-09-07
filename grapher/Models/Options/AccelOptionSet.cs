using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class AccelOptionSet
    {
        public AccelOptionSet(
            Label titleLabel,
            int topAnchor,
            AccelTypeOptions accelTypeOptions)
        {
            TitleLabel = titleLabel;
            TopAnchor = topAnchor;
            Options = accelTypeOptions;

            Options.ShowFull();

            TitleLabel.Top = TopAnchor;
            IsTitleMode = true;
            SetRegularMode();
        }

        public int TopAnchor { get; }

        public Label TitleLabel { get; }

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
            TitleLabel.Text = title;

            if (!IsTitleMode)
            {
                IsTitleMode = true;

                Options.ShowShortened();
                DisplayTitle();
            }
        }

        public void Hide()
        {
            TitleLabel.Hide();
            Options.Hide();
        }

        public void Show()
        {
            if (IsTitleMode)
            {
                TitleLabel.Show();
            }

            Options.Show();
        }

        public void DisplayTitle()
        {
            TitleLabel.Show();

            Options.Top = TitleLabel.Top + TitleLabel.Height + Constants.OptionVerticalSeperation;
        }

        public void HideTitle()
        {
            TitleLabel.Hide();

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
    }
}

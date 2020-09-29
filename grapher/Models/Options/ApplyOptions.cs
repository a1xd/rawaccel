using grapher.Models.Serialized;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace grapher.Models.Options
{
    public class ApplyOptions
    {
        #region Constructors

        public ApplyOptions(
            ToolStripMenuItem wholeVectorMenuItem,
            ToolStripMenuItem byComponentMenuItem,
            CheckBox byComponentVectorXYLock,
            AccelOptionSet optionSetX,
            AccelOptionSet optionSetY,
            OptionXY sensitivity,
            Option rotation,
            Label lockXYLabel,
            AccelCharts accelCharts)
        {
            WholeVectorMenuItem = wholeVectorMenuItem;
            ByComponentVectorMenuItem = byComponentMenuItem;

            WholeVectorMenuItem.Click += new System.EventHandler(OnWholeClicked);
            ByComponentVectorMenuItem.Click += new System.EventHandler(OnByComponentClicked);

            WholeVectorMenuItem.CheckedChanged += new System.EventHandler(OnWholeCheckedChange);
            ByComponentVectorMenuItem.CheckedChanged += new System.EventHandler(OnByComponentCheckedChange);

            ByComponentVectorXYLock = byComponentVectorXYLock;
            OptionSetX = optionSetX;
            OptionSetY = optionSetY;
            Sensitivity = sensitivity;
            Rotation = rotation;
            LockXYLabel = lockXYLabel;
            AccelCharts = accelCharts;

            LockXYLabel.AutoSize = false;
            LockXYLabel.TextAlign = ContentAlignment.MiddleCenter;

            ByComponentVectorXYLock.CheckedChanged += new System.EventHandler(OnByComponentXYLockChecked);
            ByComponentVectorXYLock.Checked = true;

            Rotation.SnapTo(Sensitivity);

            EnableWholeApplication();
        }

        #endregion Constructors

        #region Properties

        public ToolStripMenuItem WholeVectorMenuItem { get; }

        public ToolStripMenuItem ByComponentVectorMenuItem { get; }

        public CheckBox ByComponentVectorXYLock { get; }

        public AccelOptionSet OptionSetX { get; }

        public AccelOptionSet OptionSetY { get; }

        public OptionXY Sensitivity { get; }

        public Option Rotation { get; }

        public AccelCharts AccelCharts { get; }

        public Label ActiveValueTitleY { get; }

        public Label LockXYLabel { get; }

        public bool IsWhole { get; private set; }

        #endregion Properties

        #region Methods

        public Vec2<AccelMode> GetModes()
        {
            var xMode = (AccelMode)OptionSetX.Options.AccelerationIndex;

            return new Vec2<AccelMode>
            {
                x = xMode,
                y = ByComponentVectorXYLock.Checked ? xMode : (AccelMode)OptionSetY.Options.AccelerationIndex
            };
        }

        public Vec2<AccelArgs> GetArgs()
        {
            var xArgs = OptionSetX.GenerateArgs();
            
            return new Vec2<AccelArgs>
            {
                x = xArgs,
                y = ByComponentVectorXYLock.Checked ? xArgs : OptionSetY.GenerateArgs()
            };

        }

        public void SetActiveValues(
            double xSens,
            double ySens,
            double rotation,
            int xMode,
            int yMode,
            AccelArgs xArgs,
            AccelArgs yArgs,
            bool isWhole)
        {
            Sensitivity.SetActiveValues(xSens, ySens);
            Rotation.SetActiveValue(rotation);
            OptionSetX.SetActiveValues(xMode, xArgs);
            OptionSetY.SetActiveValues(yMode, yArgs);
            WholeVectorMenuItem.Checked = isWhole;
            ByComponentVectorMenuItem.Checked = !isWhole;
        }

        public void SetActiveValues(DriverSettings settings)
        {
            SetActiveValues(
                settings.sensitivity.x,
                settings.sensitivity.y,
                settings.rotation,
                (int)settings.modes.x,
                (int)settings.modes.y,
                settings.args.x,
                settings.args.y,
                settings.combineMagnitudes);

            AccelCharts.SetLogarithmic(
                OptionSetX.Options.AccelerationType.LogarithmicCharts,
                OptionSetY.Options.AccelerationType.LogarithmicCharts);
        }

        public void OnWholeClicked(object sender, EventArgs e)
        {
            if (!WholeVectorMenuItem.Checked)
            {
                WholeVectorMenuItem.Checked = true;
                ByComponentVectorMenuItem.Checked = false;
            }
        }

        public void OnByComponentClicked(object sender, EventArgs e)
        {
            if (!ByComponentVectorMenuItem.Checked)
            {
                WholeVectorMenuItem.Checked = false;
                ByComponentVectorMenuItem.Checked = true;
            }
        }

        public void OnWholeCheckedChange(object sender, EventArgs e)
        {
            if (WholeVectorMenuItem.Checked)
            {
                EnableWholeApplication();
            }
        }

        public void OnByComponentCheckedChange(object sender, EventArgs e)
        {
            if (ByComponentVectorMenuItem.Checked)
            {
                EnableByComponentApplication();
            }
        }

        public void ShowWholeSet()
        {
            OptionSetX.SetRegularMode();
            OptionSetY.Hide();
            AccelCharts.SetWidened();
            SetActiveTitlesWhole();
        }

        public void ShowByComponentAsOneSet()
        {
            OptionSetX.SetTitleMode("X = Y");
            OptionSetY.Hide();
            AccelCharts.SetWidened();
            SetActiveTitlesByComponents();
        }

        public void ShowByComponentAsTwoSets()
        {
            OptionSetX.SetTitleMode("X");
            OptionSetY.SetTitleMode("Y");
            OptionSetY.Show();
            AccelCharts.SetNarrowed();
        }

        public void ShowByComponentSet()
        {
            if (ByComponentVectorXYLock.Checked)
            {
                ShowByComponentAsOneSet();
            }
            else
            {
                ShowByComponentAsTwoSets();
            }
        }

        private void OnByComponentXYLockChecked(object sender, EventArgs e)
        {
            if (!IsWhole)
            {
                ShowByComponentSet();
            }
        }

        public void EnableWholeApplication()
        {
            IsWhole = true;
            ByComponentVectorXYLock.Hide();
            ShowWholeSet();
        }

        public void EnableByComponentApplication()
        {
            IsWhole = false;
            ByComponentVectorXYLock.Show();
            ShowByComponentSet();
        }

        private void SetActiveTitlesWhole()
        {
            OptionSetX.ActiveValuesTitle.Left = OptionSetX.Options.Left + OptionSetX.Options.Width;
            LockXYLabel.Width = (AccelCharts.Left - OptionSetX.ActiveValuesTitle.Left) / 2;
            OptionSetX.ActiveValuesTitle.Width = LockXYLabel.Width;
            LockXYLabel.Left = OptionSetX.ActiveValuesTitle.Left + OptionSetX.ActiveValuesTitle.Width;
            Sensitivity.Fields.LockCheckBox.Left = LockXYLabel.Left + LockXYLabel.Width / 2 - Sensitivity.Fields.LockCheckBox.Width / 2;
            ByComponentVectorXYLock.Left = Sensitivity.Fields.LockCheckBox.Left;
            AlignActiveValues();
        }

        private void SetActiveTitlesByComponents()
        {
            OptionSetY.ActiveValuesTitle.Left = OptionSetY.Options.Left + OptionSetY.Options.Width;
            OptionSetY.ActiveValuesTitle.Width = Constants.NarrowChartLeft - OptionSetY.ActiveValuesTitle.Left;
            AlignActiveValues();
        }

        private void AlignActiveValues()
        {
            OptionSetX.AlignActiveValues();
            OptionSetY.AlignActiveValues();
            Sensitivity.AlignActiveValues();
            Rotation.AlignActiveValues();
        }

        #endregion Methods
    }
}

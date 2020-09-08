using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
            AccelCharts = accelCharts;

            ByComponentVectorXYLock.CheckedChanged += new System.EventHandler(OnByComponentXYLockChecked);
            ByComponentVectorXYLock.Checked = true;

            EnableWholeApplication();
        }

        #endregion Constructors

        #region Properties

        public ToolStripMenuItem WholeVectorMenuItem { get; }

        public ToolStripMenuItem ByComponentVectorMenuItem { get; }

        public CheckBox ByComponentVectorXYLock { get; }

        public AccelOptionSet OptionSetX { get; }

        public AccelOptionSet OptionSetY { get; }

        public AccelCharts AccelCharts { get; }

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

        public void SetActiveValues(int xMode, int yMode, AccelArgs xArgs, AccelArgs yArgs, bool isWhole)
        {
            OptionSetX.SetActiveValues(xMode, xArgs);
            OptionSetY.SetActiveValues(yMode, yArgs);
            WholeVectorMenuItem.Checked = isWhole;
            ByComponentVectorMenuItem.Checked = !isWhole;
        }

        public void SetActiveValues(DriverSettings settings)
        {
            SetActiveValues((int)settings.modes.x, (int)settings.modes.y, settings.args.x, settings.args.y, settings.combineMagnitudes);
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
            EnableByComponentApplication();
        }

        public void ShowWholeSet()
        {
            OptionSetX.SetRegularMode();
            OptionSetY.Hide();
            AccelCharts.SetWidened();
        }

        public void ShowByComponentAsOneSet()
        {
            OptionSetX.SetTitleMode("X = Y");
            OptionSetY.Hide();
            AccelCharts.SetWidened();
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

        #endregion Methods
    }
}

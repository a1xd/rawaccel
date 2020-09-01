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
            ToolStripMenuItem byComponentMenuItem)
        {
            WholeVectorMenuItem = wholeVectorMenuItem;
            ByComponentVectorMenuItem = byComponentMenuItem;

            WholeVectorMenuItem.Click += new System.EventHandler(OnWholeClicked);
            ByComponentVectorMenuItem.Click += new System.EventHandler(OnByComponentClicked);

            WholeVectorMenuItem.CheckedChanged += new System.EventHandler(OnWholeCheckedChange);
            ByComponentVectorMenuItem.CheckedChanged += new System.EventHandler(OnByComponentCheckedChange);

            IsWhole = false;
        }

        #endregion Constructors

        #region Properties

        public ToolStripMenuItem WholeVectorMenuItem { get; }

        public ToolStripMenuItem ByComponentVectorMenuItem { get; }

        public bool IsWhole { get; private set; }

        #endregion Properties

        #region Methods

        public void SetActive(bool isWhole)
        {
            WholeVectorMenuItem.Checked = isWhole;
            ByComponentVectorMenuItem.Checked = !isWhole;
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

        public void EnableWholeApplication()
        {
            IsWhole = true;
        }
        public void EnableByComponentApplication()
        {
            IsWhole = false;
        }

        #endregion Methods
    }
}

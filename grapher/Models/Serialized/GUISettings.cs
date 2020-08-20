using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Serialized
{
    [Serializable]
    public class GUISettings
    {
        public GUISettings(
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWriteMenuItem)
        {
            BindToGUI(dpiField, pollRateField, autoWriteMenuItem);
        }

        public bool AutoWriteToDriverOnStartup { get; set; }

        public int DPI { get; set; }

        public int PollRate { get; set; }

        [field: NonSerialized]
        private Field DpiField { get; set; }

        [field: NonSerialized]
        private Field PollRateField { get; set; }

        [field: NonSerialized]
        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        public void UpdateSettings()
        {
            DPI = (int)DpiField.Data;
            PollRate = (int)PollRateField.Data;
            AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked;
        }

        public void BindToGUI(Field dpiField, Field pollRateField, ToolStripMenuItem autoWriteMenuItem)
        {
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWriteMenuItem;
        }
    }
}

using Newtonsoft.Json;
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
        public GUISettings() {}

        public GUISettings(bool autoWrite, int dpi, int pollRate)
        {
            AutoWriteToDriverOnStartup = autoWrite;
            DPI = dpi;
            PollRate = pollRate;
        }

        [JsonProperty(Order = 1)]
        public bool AutoWriteToDriverOnStartup { get; set; }

        [JsonProperty(Order = 2)]
        public int DPI { get; set; }

        [JsonProperty(Order = 3)]
        public int PollRate { get; set; }
    }
}

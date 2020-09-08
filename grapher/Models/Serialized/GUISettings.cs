using Newtonsoft.Json;
using System;

namespace grapher.Models.Serialized
{
    [Serializable]
    public class GUISettings
    {
        #region Constructors

        public GUISettings() {}

        public GUISettings(bool autoWrite, int dpi, int pollRate)
        {
            AutoWriteToDriverOnStartup = autoWrite;
            DPI = dpi;
            PollRate = pollRate;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(Order = 1)]
        public bool AutoWriteToDriverOnStartup { get; set; }

        [JsonProperty(Order = 2)]
        public int DPI { get; set; }

        [JsonProperty(Order = 3)]
        public int PollRate { get; set; }

        [JsonProperty(Order = 4)]
        public bool ShowLastMouseMove { get; set; }

        #endregion Properties
    }
}

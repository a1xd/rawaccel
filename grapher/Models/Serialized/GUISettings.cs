using Newtonsoft.Json;
using System;

namespace grapher.Models.Serialized
{
    [Serializable]
    public class GUISettings
    {
        #region Constructors

        public GUISettings() {}

        #endregion Constructors

        #region Properties


        [JsonProperty(Order = 1)]
        public int DPI { get; set; }

        [JsonProperty(Order = 2)]
        public int PollRate { get; set; }

        [JsonProperty(Order = 3)]
        public bool ShowLastMouseMove { get; set; }

        [JsonProperty(Order = 4)]
        public bool ShowVelocityAndGain { get; set; }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            var other = obj as GUISettings;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(GUISettings other)
        {
            return DPI == other.DPI &&
                PollRate == other.PollRate &&
                ShowLastMouseMove == other.ShowLastMouseMove &&
                ShowVelocityAndGain == other.ShowVelocityAndGain;
        }

        public override int GetHashCode()
        {
            return DPI.GetHashCode() ^
                PollRate.GetHashCode() ^
                ShowLastMouseMove.GetHashCode() ^
                ShowVelocityAndGain.GetHashCode();
        }

        #endregion Methods
    }
}

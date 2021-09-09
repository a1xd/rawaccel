using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace grapher.Models.Serialized
{
    [Serializable]
    public class GUISettings
    {
        #region Constructors

        public GUISettings() {}

        #endregion Constructors

        #region Properties


        public int DPI { get; set; }

        public int PollRate { get; set; }

        public bool ShowLastMouseMove { get; set; }

        public bool ShowVelocityAndGain { get; set; }

        public bool AutoWriteToDriverOnStartup { get; set; }

        public bool StreamingMode { get; set; }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is not GUISettings other)
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
                ShowVelocityAndGain == other.ShowVelocityAndGain &&
                AutoWriteToDriverOnStartup == other.AutoWriteToDriverOnStartup &&
                StreamingMode == other.StreamingMode;
        }

        public override int GetHashCode()
        {
            return DPI.GetHashCode() ^
                PollRate.GetHashCode() ^
                ShowLastMouseMove.GetHashCode() ^
                ShowVelocityAndGain.GetHashCode() ^
                AutoWriteToDriverOnStartup.GetHashCode() ^
                StreamingMode.GetHashCode();
        }

        public void Save()
        {
            File.WriteAllText(Constants.GuiConfigFileName, JsonSerializer.Serialize(this));
        }

        public static GUISettings MaybeLoad()
        {
            GUISettings settings = null;

            try
            {
                settings = JsonSerializer.Deserialize<GUISettings>(
                    File.ReadAllText(Constants.GuiConfigFileName));
            }
            catch (Exception ex)
            {
                if (!(ex is JsonException || ex is FileNotFoundException))
                {
                    throw;
                }
            }

            return settings;
        }

        #endregion Methods

    }
}

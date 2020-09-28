using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace grapher.Models.Serialized
{
    [Serializable]
    public class RawAccelSettings
    {
        #region Fields

        public static readonly string ExecutingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DefaultSettingsFile = Path.Combine(ExecutingDirectory, Constants.DefaultSettingsFileName);
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };
        #endregion Fields

        #region Constructors

        public RawAccelSettings() { }

        public RawAccelSettings(
            DriverSettings accelSettings,
            GUISettings guiSettings)
        {
            AccelerationSettings = accelSettings;
            GUISettings = guiSettings;
        }

        #endregion Constructors

        #region Properties
        [JsonProperty(Required = Required.Always)]
        public GUISettings GUISettings { get; set; }

        [JsonProperty(DriverSettings.Key, Required = Required.Always)]
        public DriverSettings AccelerationSettings { get; set; }

        #endregion Properties

        #region Methods

        public static RawAccelSettings Load()
        {
            return Load(DefaultSettingsFile);
        }

        public static RawAccelSettings Load(string file)
        {   
            try
            {
                var settings = JsonConvert.DeserializeObject<RawAccelSettings>(File.ReadAllText(file), SerializerSettings);
                if (settings is null) throw new JsonException($"{file} contains invalid JSON");
                return settings;
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException($"Settings file does not exist at {file}", e);
            }
            catch (JsonException e)
            {
                throw new JsonException($"Settings file at {file} does not contain valid Raw Accel Settings.", e);
            }
        }

        public static bool Exists()
        {
            return Exists(DefaultSettingsFile);
        }

        public static bool Exists(string file)
        {
            return File.Exists(file);
        }

        public void Save()
        {
            Save(DefaultSettingsFile);
        }

        public void Save(string file)
        {
            JObject thisJO = JObject.FromObject(this);
            AddComments(thisJO);
            File.WriteAllText(file, thisJO.ToString(Formatting.Indented));
        }

        private void AddComments(JObject thisJO)
        {
            string modes = string.Join(" | ", Enum.GetNames(typeof(AccelMode)));
            ((JObject)thisJO[DriverSettings.Key])
                .AddFirst(new JProperty("### Mode Types ###", modes));
        }

        #endregion Methods
    }
}

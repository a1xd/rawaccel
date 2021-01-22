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

        public static RawAccelSettings Load(Func<GUISettings> DefaultGUISettingsSupplier)
        {
            return Load(DefaultSettingsFile, DefaultGUISettingsSupplier);
        }

        public static RawAccelSettings Load(string file, Func<GUISettings> DefaultGUISettingsSupplier)
        {   
            try
            {
                RawAccelSettings settings = null;

                JObject settingsJObject = JObject.Parse(File.ReadAllText(file));
                if (settingsJObject.ContainsKey(DriverSettings.Key))
                {
                    settings = settingsJObject.ToObject<RawAccelSettings>(JsonSerializer.Create(SerializerSettings));
                }
                else
                {
                    settings = new RawAccelSettings
                    {
                        AccelerationSettings = settingsJObject.ToObject<DriverSettings>(),
                        GUISettings = DefaultGUISettingsSupplier()
                    };
                }

                if (settings is null || settings.AccelerationSettings is null)
                {
                    throw new JsonException($"{file} contains invalid JSON");
                }

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

        public bool IsDefaultEquivalent()
        {
            return IsDefaultEquivalent(AccelerationSettings);
        }

        public static bool IsDefaultEquivalent(DriverSettings accelSettings)
        {
            bool wholeOrNoY = accelSettings.combineMagnitudes ||
                accelSettings.modes.y == AccelMode.noaccel;

            return string.IsNullOrEmpty(accelSettings.deviceID) &&
                accelSettings.sensitivity.x == 1 &&
                accelSettings.sensitivity.y == 1 &&
                accelSettings.directionalMultipliers.x <= 0 &&
                accelSettings.directionalMultipliers.y <= 0 &&
                accelSettings.rotation == 0 &&
                accelSettings.snap == 0 &&
                accelSettings.modes.x == AccelMode.noaccel &&
                wholeOrNoY;
        }

        #endregion Methods
    }
}

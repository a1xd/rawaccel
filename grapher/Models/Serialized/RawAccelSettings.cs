using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            MissingMemberHandling = MissingMemberHandling.Error,
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

        public GUISettings GUISettings { get; set; }

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
               return JsonConvert.DeserializeObject<RawAccelSettings>(File.ReadAllText(file), SerializerSettings);
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException($"Settings file does not exist at {file}", e);
            }
            catch (JsonSerializationException e)
            {
                throw new JsonSerializationException($"Settings file at {file} does not contain valid Raw Accel Settings.", e);
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
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        #endregion Methods
    }
}

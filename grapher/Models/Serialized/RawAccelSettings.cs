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
        public const string DefaultSettingsFileName = @"settings.json";
        public static readonly string ExecutingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DefaultSettingsFile = Path.Combine(ExecutingDirectory, DefaultSettingsFileName);
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
        };

        public RawAccelSettings() { }

        public RawAccelSettings(
            ManagedAccel managedAccel,
            GUISettings guiSettings)
        {
            AccelerationSettings = new modifier_args(managedAccel);
            GUISettings = guiSettings;
        }


        public GUISettings GUISettings { get; set; }

        public modifier_args AccelerationSettings { get; set; }

        public static RawAccelSettings Load()
        {
            return Load(DefaultSettingsFile);
        }

        public static RawAccelSettings Load(string file)
        {
            if (!Exists(file))
            {
                throw new Exception($"Settings file does not exist at {file}");
            }

            RawAccelSettings deserializedSettings;
            try
            {
               deserializedSettings = JsonConvert.DeserializeObject<RawAccelSettings>(File.ReadAllText(file), SerializerSettings);
            }
            catch(Exception e)
            {
                throw new Exception($"Settings file at {file} does not contain valid Raw Accel Settings.", e);
            }

            return deserializedSettings;
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
    }
}

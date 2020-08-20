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
    public class RawAccelSettings
    {
        public const string DefaultSettingsFile = @".\settings.json";

        public RawAccelSettings(
            ManagedAccel managedAccel,
            GUISettings guiSettings)
        {
            ManagedAccel = managedAccel;
            GUISettings = guiSettings;
        }

        public ManagedAccel ManagedAccel { get; set; }

        public GUISettings GUISettings { get; set; }

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

            object deserializedObject;
            try
            {
               deserializedObject = JsonConvert.DeserializeObject(File.ReadAllText(file));
            }
            catch
            {
                throw new Exception($"Settings file at {file} does not contain valid JSON.");
            }

            RawAccelSettings deserializedSettings = (RawAccelSettings)deserializedObject;

            if (deserializedSettings == null)
            {
                throw new Exception($"Settings file at {file} does not contain valid Raw Accel Settings.");
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Windows.Forms;

namespace writer
{

    class Program
    {

        static void Show(string msg)
        {
            MessageBox.Show(msg, "Raw Accel writer");
        }

        static void Send(JToken settingsToken)
        {
            var settings = settingsToken.ToObject<DriverSettings>();

            var errors = DriverInterop.GetSettingsErrors(settings);
            if (errors.Empty())
            {
                DriverInterop.Write(settings);
                return;
            }

            Show($"Bad settings:\n\n{errors}");
        }

        static void Main(string[] args)
        {
            try
            {
                VersionHelper.ValidateAndGetDriverVersion(typeof(Program).Assembly.GetName().Version);
            }
            catch (VersionException e)
            {
                Show(e.Message);
                return;
            }

            if (args.Length != 1)
            {
                Show($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <settings file path>");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Show($"Settings file not found at {args[0]}");
                return;
            }

            try
            {
                var JO = JObject.Parse(File.ReadAllText(args[0]));

                if (JO.ContainsKey(DriverSettings.Key))
                {
                    Send(JO[DriverSettings.Key]);
                    return;
                }

                Send(JO);
            }
            catch (JsonException e)
            {
                Show($"Settings invalid:\n\n{e.Message}");
            }
            catch (Exception e)
            {
                Show($"Error:\n\n{e}");
            }
        }
    }
}

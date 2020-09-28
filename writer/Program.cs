using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;

namespace writer
{

    class Program
    {
        static void Send(JToken settingsToken)
        {
            var settings = settingsToken.ToObject<DriverSettings>();

            var errors = DriverInterop.GetSettingsErrors(settings);
            if (errors.Empty())
            {
                DriverInterop.Write(settings);
                return;
            }

            foreach (var err in errors.x)
            {
                Console.WriteLine(err + (settings.combineMagnitudes ? "" : " (x)"));
            }
            foreach (var err in errors.y)
            {
                Console.WriteLine(err + " (y)");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 1 || args[0].Equals("help"))
            {
                Console.WriteLine("USAGE: {0} <file>", System.AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Settings file not found at {0}", args[0]);
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
                Console.WriteLine("Settings invalid:\n{0}", e.Message.ToString());
            }
            catch (DriverNotInstalledException)
            {
                Console.WriteLine("Driver is not installed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message.ToString());
            }
        }
    }
}

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

        static void Main(string[] args)
        {
            try
            {
                VersionHelper.ValidOrThrow();
            }
            catch (InteropException e)
            {
                Show(e.Message);
                return;
            }

            if (args.Length != 1)
            {
                Show($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <settings file path>");
                return;
            }

            try
            {
                var settings = DriverSettings.FromFile(args[0]);
                var errors = new SettingsErrors(settings);

                if (errors.Empty())
                {
                    new ManagedAccel(settings).Activate();
                }
                else
                {
                    Show($"Bad settings:\n\n{errors}");
                }
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

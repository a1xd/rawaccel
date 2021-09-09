using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace writer
{

    class Program
    {
        static readonly string DefaultPath = "settings.json";
        static readonly string Usage =
            $"Usage: {AppDomain.CurrentDomain.FriendlyName} <settings file path>\n";

        static void Exit(string msg)
        {
            MessageBox.Show(msg, "Raw Accel writer");
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            try
            {
                VersionHelper.ValidOrThrow();
            }
            catch (InteropException e)
            {
                Exit(e.Message);
            }

            try
            {
                if (args.Length != 1)
                {
                    if (File.Exists(DefaultPath))
                    {
                        Exit(Usage);
                    }
                    else
                    {
                        File.WriteAllText(DefaultPath, DriverConfig.GetDefault().ToJSON());
                        Exit($"{Usage}\n(generated default settings file '{DefaultPath}')");
                    }
                }
                else
                {
                    var result = DriverConfig.Convert(File.ReadAllText(args[0]));
                    if (result.Item2 == null)
                    {
                        result.Item1.Activate();
                    }
                    else
                    {
                        Exit($"Bad settings:\n\n{result.Item2}");
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Exit(e.Message);
            }
            catch (JsonException e)
            {
                Exit($"Settings format invalid:\n\n{e.Message}");
            }
            catch (Exception e)
            {
                Exit($"Error:\n\n{e}");
            }
        }
    }
}

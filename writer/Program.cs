using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace writer
{

    class Program
    {

        static void ExitWithMessage(string msg)
        {
            MessageBox.Show(msg, "Raw Accel writer");
            Environment.Exit(1);
        }

        static void ExitWithUsage()
        {
            ExitWithMessage($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <settings file path>");
        }

        delegate string PopOption(params string[] aliases);

        static string Read(string path)
        {
            return path == null ? null : File.ReadAllText(path);
        }

        static ExtendedSettings Parse(List<string> args)
        {
            PopOption maybePop = aliases =>
            {
                int idx = args.FindIndex(aliases.Contains);

                if (idx == -1) return null;

                if (idx == args.Count - 1) ExitWithUsage();

                string val = args[idx + 1];
                args.RemoveRange(idx, 2);
                return val;
            };

            string settingsPath = null;

            string tablePath = maybePop("/table", "/t");

            if (tablePath != null)
            {
                if (args.Count > 1) ExitWithUsage();
                else if (args.Count == 1) settingsPath = args[0];

                return new ExtendedSettings(Read(settingsPath), Read(tablePath));
            }

            string xTablePath = maybePop("/xtable", "/xt");
            string yTablePath = maybePop("/ytable", "/yt");

            if (args.Count > 1) ExitWithUsage();
            else if (args.Count == 1) settingsPath = args[0];
            else if (xTablePath == null && yTablePath == null) ExitWithUsage();

            string xTableJson = Read(xTablePath);
            string yTableJson = null;

            if (xTablePath != null && xTablePath.Equals(yTablePath))
            {
                yTableJson = xTableJson;
            }
            else
            {
                yTableJson = Read(yTablePath);
            }

            return new ExtendedSettings(Read(settingsPath), xTableJson, yTableJson);
        }

        static void Main(string[] args)
        {
            try
            {
                VersionHelper.ValidOrThrow();
            }
            catch (InteropException e)
            {
                ExitWithMessage(e.Message);
            }

            try
            {
                var settings = Parse(new List<string>(args));
                var errors = new SettingsErrors(settings);

                if (errors.Empty())
                {
                    new ManagedAccel(settings).Activate();
                }
                else
                {
                    ExitWithMessage($"Bad settings:\n\n{errors}");
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                ExitWithMessage(e.Message);
            }
            catch (JsonException e)
            {
                ExitWithMessage($"Settings invalid:\n\n{e.Message}");
            }
            catch (Exception e)
            {
                ExitWithMessage($"Error:\n\n{e}");
            }
        }
    }
}

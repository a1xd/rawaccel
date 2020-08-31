using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace grapher.Models.Serialized
{
    public class SettingsManager
    {
        public SettingsManager(
            ManagedAccel activeAccel,
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWrite)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
        }

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        private Field DpiField { get; set; }

        private Field PollRateField { get; set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        public void UpdateActiveSettings(DriverSettings settings)
        {
            try
            {
                settings.SendToDriverAndUpdate(ActiveAccel);

                RawAccelSettings.AccelerationSettings = settings;
                RawAccelSettings.GUISettings = new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data
                };

                RawAccelSettings.Save();
            }
            catch (DriverWriteCDException)
            {
                Console.WriteLine("write on cooldown");
            }
        }

        public void UpdateActiveAccelFromFileSettings(DriverSettings settings)
        {
            try
            {
                settings.SendToDriverAndUpdate(ActiveAccel);
            }
            catch (DriverWriteCDException)
            {
                Console.WriteLine("write on cd during file init");
            }
            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
        }

        public void Startup()
        {
            if (RawAccelSettings.Exists())
            {
                try
                {
                    RawAccelSettings = RawAccelSettings.Load();
                    if (RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup)
                    {
                        UpdateActiveAccelFromFileSettings(RawAccelSettings.AccelerationSettings);
                    }
                    return;
                }
                catch (JsonSerializationException e)
                {
                    Console.WriteLine($"bad settings: {e}");
                }
            }

            RawAccelSettings = new RawAccelSettings(
                DriverSettings.GetActive(),
                new GUISettings
                {
                    AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked,
                    DPI = (int)DpiField.Data,
                    PollRate = (int)PollRateField.Data
                });
            RawAccelSettings.Save();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Drawing;

namespace grapher.Models.Serialized
{
    public class SettingsManager
    {
        #region Constructors

        public SettingsManager(
            ManagedAccel activeAccel,
            Field dpiField,
            Field pollRateField,
            ToolStripMenuItem autoWrite,
            ToolStripMenuItem useSpecificDevice,
            ToolStripMenuItem showLastMouseMove,
            ToolStripMenuItem showVelocityAndGain)
        {
            ActiveAccel = activeAccel;
            DpiField = dpiField;
            PollRateField = pollRateField;
            AutoWriteMenuItem = autoWrite;
            UseSpecificDeviceMenuItem = useSpecificDevice;
            ShowLastMouseMoveMenuItem = showLastMouseMove;
            ShowVelocityAndGainMoveMenuItem = showVelocityAndGain;
        }

        #endregion Constructors

        #region Properties

        public ManagedAccel ActiveAccel { get; }

        public RawAccelSettings RawAccelSettings { get; private set; }

        public Field DpiField { get; private set; }

        public Field PollRateField { get; private set; }

        private ToolStripMenuItem AutoWriteMenuItem { get; set; }

        private ToolStripMenuItem UseSpecificDeviceMenuItem { get; set; }

        private ToolStripMenuItem ShowLastMouseMoveMenuItem { get; set; }

        private ToolStripMenuItem ShowVelocityAndGainMoveMenuItem { get; set; }

        #endregion Properties

        #region Methods
        public SettingsErrors TryUpdateActiveSettings(DriverSettings settings)
        {
            var errors = TryUpdateAccel(settings);

            if (errors.Empty())
            {
                RawAccelSettings.AccelerationSettings = settings;
                RawAccelSettings.GUISettings = MakeGUISettingsFromFields();
                RawAccelSettings.Save();
            }

            return errors;
        }

        private void SpecificDeviceClickHnadler(Object o, EventArgs a, string hwid)
        {
            var item = (ToolStripMenuItem)o;
            foreach (ToolStripMenuItem i in UseSpecificDeviceMenuItem.DropDownItems)
            {
                i.Checked = false;
            }
            item.Checked = true;
            if (hwid == null || hwid == "")
            {
                UseSpecificDeviceMenuItem.Checked = false;
            } else
            {
                UseSpecificDeviceMenuItem.Checked = true;
            }
            RawAccelSettings.AccelerationSettings.deviceHardwareID = hwid;

            TryUpdateActiveSettings(RawAccelSettings.AccelerationSettings);
            
        }
        private void UpdateUseSpecificDeviceMenu()
        {
            var hwid = RawAccelSettings.AccelerationSettings.deviceHardwareID;
            if (hwid == null) { hwid = ""; }

            UseSpecificDeviceMenuItem.Checked = hwid.Length > 0;
            UseSpecificDeviceMenuItem.DropDownItems.Clear();

            var any_device = new ToolStripMenuItem();
            any_device.Text = "";
            any_device.Checked = hwid.Length == 0;
            any_device.Click += new EventHandler(delegate(Object o, EventArgs a) { SpecificDeviceClickHnadler(o, a, ""); });
            UseSpecificDeviceMenuItem.DropDownItems.Add(any_device);

            var hwid_not_found = true;

            foreach (Tuple<string,string> device in Models.Devices.DeviceList.GetDeviceHardwareIDs())
            {
                if (hwid == device.Item2)
                {
                    hwid_not_found = false;
                }
                var dev = new ToolStripMenuItem();
                dev.Text = device.Item1;
                dev.Checked = device.Item2 == RawAccelSettings.AccelerationSettings.deviceHardwareID;
                dev.Click += new EventHandler(delegate (Object o, EventArgs a) { SpecificDeviceClickHnadler(o, a, device.Item2); });
                UseSpecificDeviceMenuItem.DropDownItems.Add(dev);
            }

            if (hwid.Length > 0 && hwid_not_found)
            {
                var current_hwid = new ToolStripMenuItem();
                current_hwid.Text = "Disconnected (" + hwid + ")";
                current_hwid.ForeColor = Color.DarkGray;
                current_hwid.Checked = true;
                current_hwid.Click += new EventHandler(delegate (Object o, EventArgs a) { SpecificDeviceClickHnadler(o, a, hwid); });
                UseSpecificDeviceMenuItem.DropDownItems.Add(current_hwid);
            }
        }

        public void UpdateFieldsFromGUISettings()
        {
            DpiField.SetToEntered(RawAccelSettings.GUISettings.DPI);
            PollRateField.SetToEntered(RawAccelSettings.GUISettings.PollRate);
            ShowLastMouseMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowLastMouseMove;
            ShowVelocityAndGainMoveMenuItem.Checked = RawAccelSettings.GUISettings.ShowVelocityAndGain;
            AutoWriteMenuItem.Checked = RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
        }

        public SettingsErrors TryUpdateAccel(DriverSettings settings)
        {
            var errors = SendToDriverSafe(settings);
            if (errors.Empty()) ActiveAccel.UpdateFromSettings(settings);
            return errors;
        }

        public static void SendToDriver(DriverSettings settings)
        {
            new Thread(() => DriverInterop.Write(settings)).Start();
        }

        public static SettingsErrors SendToDriverSafe(DriverSettings settings)
        {
            var errors = DriverInterop.GetSettingsErrors(settings);
            if (errors.Empty()) SendToDriver(settings);
            return errors;
        }

        public GUISettings MakeGUISettingsFromFields()
        {
            return new GUISettings
            {
                DPI = (int)DpiField.Data,
                PollRate = (int)PollRateField.Data,
                ShowLastMouseMove = ShowLastMouseMoveMenuItem.Checked,
                ShowVelocityAndGain = ShowVelocityAndGainMoveMenuItem.Checked,
                AutoWriteToDriverOnStartup = AutoWriteMenuItem.Checked
            };
        }

        // Returns true when file settings are active
        public bool Startup()
        {
            if (RawAccelSettings.Exists())
            {
                try
                {
                    RawAccelSettings = RawAccelSettings.Load(() => MakeGUISettingsFromFields());
                    UpdateFieldsFromGUISettings();
                    UpdateUseSpecificDeviceMenu();
                    if (RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup)
                    {
                        TryUpdateAccel(RawAccelSettings.AccelerationSettings);
                    }
                    return RawAccelSettings.GUISettings.AutoWriteToDriverOnStartup;
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"bad settings: {e}");
                }
            }

            RawAccelSettings = new RawAccelSettings(
                DriverInterop.GetActiveSettings(),
                MakeGUISettingsFromFields());
            RawAccelSettings.Save();
            return true;
        }

        #endregion Methods
    }
}

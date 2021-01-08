using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Devices
{
    public class DeviceIDManager
    {
        public DeviceIDManager(ToolStripMenuItem deviceIDs)
        {
            DeviceIDsMenuItem = deviceIDs;
        }

        public ToolStripMenuItem DeviceIDsMenuItem { get; }

        public string HWID { get => SelectedDeviceID.HWID; }

        public DeviceIDItem SelectedDeviceID { get; private set; }

        public Dictionary<string, DeviceIDItem> DeviceIDs { get; private set; }

        public static IEnumerable<(string, string)> GetDeviceHardwareIDs(string PNPClass = "Mouse")
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("Win32_PnPEntity"));

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["PNPClass"] != null && obj["PNPClass"].ToString().Equals(PNPClass) && obj["HardwareID"] != null)
                {
                    string[] hwidArray = (string[])(obj["HardwareID"]);
                    if (hwidArray.Length > 0)
                    {
                        string hwid = hwidArray[0].ToString();
                        string name = obj["Name"].ToString();
                        yield return (name, hwid);
                    }
                }
            }
        }

        public void SetActive(DeviceIDItem deviceIDItem)
        {
            if (SelectedDeviceID != null)
            {
                SelectedDeviceID.SetDeactivated();
            }

            SelectedDeviceID = deviceIDItem;
            SelectedDeviceID.SetActivated();
        }

        public void OnStartup(string hwid)
        {
            var nonEmptyHwid = !string.IsNullOrWhiteSpace(hwid);

            DeviceIDsMenuItem.Checked = nonEmptyHwid;
            DeviceIDsMenuItem.DropDownItems.Clear();
            var anyDevice = new DeviceIDItem("Any", string.Empty, this);
            if (!nonEmptyHwid)
            {
                SetActive(anyDevice);
            }

            bool found = false;

            foreach (var device in GetDeviceHardwareIDs())
            {
                var deviceItem = new DeviceIDItem(device.Item1, device.Item2, this);
                if (deviceItem.HWID.Equals(hwid))
                {
                    found = true;
                    deviceItem.SetActivated();
                    SelectedDeviceID = deviceItem;
                }
            }

            if (nonEmptyHwid && !found)
            {
                var deviceItem = new DeviceIDItem(string.Empty, hwid, this);
                deviceItem.SetDisconnected();
            }
        }

    }
}

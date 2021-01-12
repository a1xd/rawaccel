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
            DeviceIDsMenuItem.Checked = false;
        }

        public ToolStripMenuItem DeviceIDsMenuItem { get; }

        public string ID { get => SelectedDeviceID.ID; }

        public DeviceIDItem SelectedDeviceID { get; private set; }

        public Dictionary<string, DeviceIDItem> DeviceIDs { get; private set; }

        public static IEnumerable<(string, string)> GetDeviceIDs(string PNPClass = "Mouse")
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("Win32_PnPEntity"));

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["PNPClass"] != null && obj["PNPClass"].ToString().Equals(PNPClass) && obj["DeviceID"] != null)
                {
                    string name = obj["Name"].ToString();

                    string devInstanceID = obj["DeviceID"].ToString();
                    string devID = devInstanceID.Remove(devInstanceID.LastIndexOf('\\'));
                    
                    yield return (name, devID);
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

        public void Update(string devID)
        {
            DeviceIDsMenuItem.DropDownItems.Clear();

            bool found = string.IsNullOrEmpty(devID);

            var anyDevice = new DeviceIDItem("Any", string.Empty, this);

            if (found) SetActive(anyDevice);

            foreach (var device in GetDeviceIDs().Distinct())
            {
                var deviceItem = new DeviceIDItem(device.Item1, device.Item2, this);
                if (!found && deviceItem.ID.Equals(devID))
                {
                    SetActive(deviceItem);
                    found = true;
                }
            }

            if (!found)
            {
                var deviceItem = new DeviceIDItem(string.Empty, devID, this);
                deviceItem.SetDisconnected();
                SetActive(deviceItem);
            }
        }

    }
}

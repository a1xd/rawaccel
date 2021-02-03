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

            foreach (var (name, id) in RawInputInterop.GetDeviceIDs())
            {
                var deviceItem = new DeviceIDItem(name, id, this);
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

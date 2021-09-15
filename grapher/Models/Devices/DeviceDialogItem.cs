using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Devices
{
    public class DeviceDialogItem
    {
        public MultiHandleDevice device;
        public DeviceSettings oldSettings;
        public DeviceConfig newConfig;
        public string newProfile;
        public bool overrideDefaultConfig;

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(device.name) ?
                device.id :
                device.name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Devices
{
    public class DeviceIDItem
    {
        public DeviceIDItem(string name, string id, DeviceIDManager manager)
        {
            Name = name;
            ID = id;
            Manager = manager;
            DeviceIDMenuItem = new ToolStripMenuItem();
            DeviceIDMenuItem.Checked = false;
            DeviceIDMenuItem.Text = MenuItemText();
            DeviceIDMenuItem.Click += OnClicked;
            manager.DeviceIDsMenuItem.DropDownItems.Add(DeviceIDMenuItem);
        }

        private ToolStripMenuItem DeviceIDMenuItem { get; }

        public string Name { get; }

        public string ID { get; }

        private DeviceIDManager Manager { get; }

        public void SetActivated()
        {
            DeviceIDMenuItem.Checked = true;
        }

        public void SetDeactivated()
        {
            DeviceIDMenuItem.Checked = false;
        }

        private string MenuItemText() => string.IsNullOrEmpty(ID) ? $"{Name}" : ID.Replace("&", "&&");

        private string DisconnectedText() => $"Disconnected: {ID}";

        public void SetDisconnected()
        {
            DeviceIDMenuItem.ForeColor = Color.DarkGray;
            DeviceIDMenuItem.Text = DisconnectedText();
        }

        public void OnClicked(object sender, EventArgs e)
        {
            Manager.SetActive(this);
        }

        public override bool Equals(object obj)
        {
            return obj is DeviceIDItem item &&
                   Name == item.Name &&
                   ID == item.ID;
        }

        public override int GetHashCode()
        {
            int hashCode = -1692744877;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ID);
            return hashCode;
        }
    }
}

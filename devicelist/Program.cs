using System;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace devicelist
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("Win32_PnPEntity"));

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["PNPClass"].ToString() == "Mouse" && obj["HardwareID"] != null) {
                    String[] hwidArray = (String[])(obj["HardwareID"]);
                    String hwid = hwidArray[0].ToString().Replace(@"\", @"\\");
                    String caption = "(" + obj["Name"].ToString() + ") Device Hardware ID:";
                    if (MessageBox.Show(hwid, caption, MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                        break;
                    }
                }
            }
        }
    }
}

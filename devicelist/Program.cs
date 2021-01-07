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
            Console.WriteLine("To use Raw Accel driver for a specific device, "
                + "replace '\"Device Hardware ID\": null' in 'settings.json' by following:");
            Console.WriteLine("");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("Win32_PnPEntity"));

            foreach (ManagementObject obj in searcher.Get())
            {
                bool is_mouse = obj["PNPClass"] != null && obj["PNPClass"].ToString() == "Mouse"; // == "HIDClass" ???

                if (is_mouse && obj["HardwareID"] != null) {
                    String[] hwidArray = (String[])(obj["HardwareID"]);
                    if (hwidArray.Length > 0) {
                        String hwid = hwidArray[0].ToString().Replace(@"\", @"\\");
                        String name = obj["Name"].ToString();
                        Console.WriteLine(name + ":");
                        Console.WriteLine("\"Device Hardware ID\": \"" + hwid + "\"");
                        Console.WriteLine("");
                    }
                }
            }

            Console.ReadKey();
        }
    }
}

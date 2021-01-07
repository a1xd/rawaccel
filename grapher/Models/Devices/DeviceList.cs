using System;
using System.Management;
using System.Collections.Generic;

namespace grapher.Models.Devices
{
    class DeviceList
    {
        public static List<Tuple<string, string>> GetDeviceHardwareIDs(string PNPClass = "Mouse")
        {
            var results = new List<Tuple<string, string>>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery("Win32_PnPEntity"));

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["PNPClass"] != null && obj["PNPClass"].ToString() == PNPClass && obj["HardwareID"] != null)
                {
                    String[] hwidArray = (String[])(obj["HardwareID"]);
                    if (hwidArray.Length > 0)
                    {
                        String hwid = hwidArray[0].ToString();
                        String name = obj["Name"].ToString();
                        results.Add(Tuple.Create(name, hwid));
                    }
                }
            }

            return results;
        }

    }
}

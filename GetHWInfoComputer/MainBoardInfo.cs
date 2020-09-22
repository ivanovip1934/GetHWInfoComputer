using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace GetHWInfoComputer
{
    [Serializable]
    public class MainBoardInfo
    {
        public string Product { get;  set; }
        public string SerialNumber { get;  set; }
        public string Manufacturer { get; set; }
        public string SMBIOSBIOSVersion { get;  set; }
        public string[] MAC { get; set; } 
        private Dictionary<string, string> dicManufacturer = new Dictionary<string, string>()
        {
            [$"{Manufacturers.Acer}"] = "acer",
            [$"{Manufacturers.Asrock}"] = "asrock",
            [$"{Manufacturers.Asus}"] = "asustek",
            [$"{Manufacturers.Gigabyte}"] = "gigabyte",
            [$"{Manufacturers.HP}"] = "hewlett-packard",
            [$"{Manufacturers.Lenovo}"] = "lenovo",
            [$"{Manufacturers.MSI}"] = "micro-star|msi",
            [$"{Manufacturers.Pegatron}"] = "pegatron"

        };
        private Dictionary<Manufacturers, string[]> dicManufacMAC = new Dictionary<Manufacturers, string[]>() { 
            [Manufacturers.Asus] =  new string[] { "04D4C4", "E03F49", "3497F6", "0C9D92", "AC220B", "60A44C", "F832E4", "BCEE7B", "107B44", "B06EBF", "3085A9", "08606E", "14DDA9", "086266", "50465D", "2C4D54" },
            [Manufacturers.Gigabyte] = new string[] {"902B34","B42E99" },
            [Manufacturers.Asrock] = new string[] { "7085C2","D05099"},
            [Manufacturers.MSI] = new string[] { "309C23","D43D7E"},
            [Manufacturers.HP] = new string[] { "3464A9"},            
            [Manufacturers.Pegatron] = new string[] { "4C72B9"}
        };
        public MainBoardInfo() {
            
        }

        public MainBoardInfo GetInfo()
        {
            Dictionary<string, string>[] dicProperties1 = GeneralStaticMethods.GetHardWareInfo<MainBoardInfo>("Win32_baseboard");
            Dictionary<string, string>[] dicProperties2 = GeneralStaticMethods.GetHardWareInfo<MainBoardInfo>("Win32_bios");
            this.Product = dicProperties1[0]["Product"];
            this.SerialNumber = GeneralStaticMethods.GetSerialNumber(TypeHW.MainBoard, dicProperties1[0]["SerialNumber"]);
            this.Manufacturer = NameManufacturer(dicProperties1[0]["Manufacturer"]);
            this.SMBIOSBIOSVersion = dicProperties2[0]["SMBIOSBIOSVersion"];
            this.MAC = MACAdresses(this.Manufacturer);

            return new MainBoardInfo { Product = this.Product, SerialNumber = this.SerialNumber, Manufacturer = this.Manufacturer, SMBIOSBIOSVersion = this.SMBIOSBIOSVersion, MAC = this.MAC };
        }

        private string NameManufacturer(string nameManufacturer) {
            Regex rgx;
            foreach (KeyValuePair<string, string> item in dicManufacturer) {
                rgx = new Regex(item.Value, RegexOptions.IgnoreCase);
                if (rgx.IsMatch(nameManufacturer)) {
                    nameManufacturer = item.Key;
                    break;
                }                
            }
            return nameManufacturer;           
        }

        private string[] MACAdresses(string manufacturers)
        {
            List<string> arrMAC = new List<string>();
            string macManufacturer = String.Empty;
            var MACAddress = NetworkInterface
                                  .GetAllNetworkInterfaces()
                                  .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                  .Select(nic => new { nic.Description, MAC = nic.GetPhysicalAddress().ToString() });
            foreach (var item in MACAddress)
            {
                if (!String.IsNullOrWhiteSpace(item.MAC) && !item.Description.Contains("VirtualBox") && !item.Description.Contains("TAP-Windows"))
                {
                    macManufacturer = item.MAC.Length >= 6
                             ? item.MAC.Substring(0, 6) : item.MAC;
                    if (macManufacturer != "005056" && dicManufacMAC.Any(x => x.Value.Contains(macManufacturer))) {
                        if (manufacturers == dicManufacMAC.First(x => x.Value.Contains(macManufacturer)).Key.ToString()) {
                            arrMAC.Clear();
                            arrMAC.Add(item.MAC);
                            return arrMAC.ToArray();
                        }
                    }else if (macManufacturer != "005056")
                        arrMAC.Add(item.MAC);
                }
            }

            return arrMAC.ToArray();
        }
    }
}

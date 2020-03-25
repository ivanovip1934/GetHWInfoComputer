using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetHWInfoComputer
{
    [Serializable]
    public class MainBoardInfo
    {
        public string Product { get;  set; }
        public string SerialNumber { get;  set; }
        public string Manufacturer { get; set; }
        public string SMBIOSBIOSVersion { get;  set; }
        public MainBoardInfo() {
            
        }

        public MainBoardInfo GetInfo()
        {
            Dictionary<string, string>[] dicProperties1 = GeneralStaticMethods.GetHardWareInfo<MainBoardInfo>("Win32_baseboard");
            Dictionary<string, string>[] dicProperties2 = GeneralStaticMethods.GetHardWareInfo<MainBoardInfo>("Win32_bios");
            this.Product = dicProperties1[0]["Product"];
            this.SerialNumber = GeneralStaticMethods.GetSerialNumber(TypeHW.MainBoard, dicProperties1[0]["SerialNumber"]);
            this.Manufacturer = dicProperties1[0]["Manufacturer"];
            this.SMBIOSBIOSVersion = dicProperties2[0]["SMBIOSBIOSVersion"];

            return new MainBoardInfo { Product = this.Product, SerialNumber = this.SerialNumber, Manufacturer = this.Manufacturer, SMBIOSBIOSVersion = this.SMBIOSBIOSVersion };
        }
    }
}

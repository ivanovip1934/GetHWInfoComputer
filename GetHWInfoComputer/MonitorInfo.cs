using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Tools;

namespace GetHWInfoComputer
{
    [Serializable]
    public class MonitorInfo : MonitorModel
    {
        public string SerailNumber { get; set; }
        private List<MonitorModel> lsMonitors;
        private Dictionary<string, Manufacturers> dicManufacturer = new Dictionary<string, Manufacturers>()
        {
            ["ACI"] = Manufacturers.Asus,
            ["ACR"] = Manufacturers.Acer,
            ["AOC"] = Manufacturers.AOC,
            ["BNQ"] = Manufacturers.BenQ,
            ["GSM"] = Manufacturers.LG,
            ["HWP"] = Manufacturers.HP,
            ["IVM"] = Manufacturers.Iiyama,
            ["LEN"] = Manufacturers.Lenovo,
            ["PHL"] = Manufacturers.Philips,
            ["SAM"] = Manufacturers.Samsung,
            ["VSC"] = Manufacturers.ViewSonic,
            ["DEL"] = Manufacturers.Dell,
            ["NEC"] = Manufacturers.NEC
        };
   
    

    public MonitorInfo() {



     }
        public MonitorInfo(MonitorModel mm, string sn) {
            
            this.Manufacturer = mm.Manufacturer;
            this.Model = mm.Model;
            this.PanelSize = mm.PanelSize;
            this.TrueResolution = mm.TrueResolution;
            this.SerailNumber = sn;
        }

        public MonitorInfo(MonitorFromWMI monFromWMI) {
            
            if (dicManufacturer.ContainsKey(monFromWMI.ManufacturerName))
            {
                this.Manufacturer = dicManufacturer[monFromWMI.ManufacturerName];
                this.Model = monFromWMI.UserFriendlyName;
            }
            else {
                this.Manufacturer = Manufacturers.Unknown;
                this.Model = $"{monFromWMI.ManufacturerName} - {monFromWMI.UserFriendlyName}";
            }

            this.PanelSize = "Unknown";
            this.TrueResolution = "Unknown";
            this.SerailNumber = monFromWMI.SerialNumberID;
        
        }

        public List<MonitorInfo> GetInfo()
        {
            string pathtolsMonitors = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lsMonitors.xml");
             this.lsMonitors =  GeneralStaticMethods.ReadFromXmlFile<List<MonitorModel>>(pathtolsMonitors);
            List<MonitorInfo> lsMonitor = new List<MonitorInfo>();
            string key = String.Empty;
            List<MonitorFromWMI> monFromWMI = (new MonitorFromWMI()).Get();
            foreach (MonitorFromWMI item in monFromWMI) {
                key = $"{item.ManufacturerName}_{item.UserFriendlyName}";
                if (lsMonitors.Any(x=>x.ID == key))
                {
                    lsMonitor.Add(new MonitorInfo(lsMonitors.First(x=>x.ID == key), item.SerialNumberID));
                }
                else
                {
                    lsMonitor.Add(new MonitorInfo(item));
                }
            }

            return lsMonitor;
        }
    }






    
    public class MonitorFromWMI
    {
        public string ManufacturerName { get; set; }
        public string UserFriendlyName { get; set; }
        public string SerialNumberID { get; set; }       

        public MonitorFromWMI() { }

        public List<MonitorFromWMI> Get()
        {
            Dictionary<string, string>[] dicpor = GeneralStaticMethods.GetMonitorInfo<MonitorFromWMI>("WMIMonitorID");
            Console.WriteLine($@"Count monitors: {dicpor.Count()}");
            List<MonitorFromWMI> lstmonitor = new List<MonitorFromWMI>();
            
            if (dicpor != null)
            {
                lstmonitor = (from item in dicpor
                              select new MonitorFromWMI { ManufacturerName = item["ManufacturerName"], UserFriendlyName = item["UserFriendlyName"], SerialNumberID = item["SerialNumberID"] }).ToList();

                return lstmonitor;
            }
            else
            {
                return null;
            }

        }
    }
   


    [Serializable]
    public class MonitorModel {
        public Manufacturers Manufacturer { get; set; }
        public string Model { get; set; }
        public string PanelSize { get; set; }
        public string TrueResolution { get; set; }
        public string ID { get; set; }

        public MonitorModel()
        {
        }
    }




    public enum Manufacturers
    {
        AOC = 1,
        Acer = 2,
        Asrock = 3,
        Asus = 4,
        BenQ = 5,
        Dell = 6,
        Gigabyte = 7,
        HP = 8,
        Iiyama = 9,
        Lenovo = 10,
        LG = 11,
        MSI = 12,
        Pegatron = 13,
        Philips = 14,
        Samsung = 15,
        ViewSonic = 16,
        Unknown = 17,
        NEC = 18
    }



    enum TrueResolution { 
    R1280X1024 = 1,
    R1440x900 =2,
    R1680X1050 = 3,
    FullHD = 4,
    R2K =5,
    R4K = 6
    }
    

}

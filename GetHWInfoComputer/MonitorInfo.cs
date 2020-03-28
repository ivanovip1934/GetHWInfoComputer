using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Tools;

namespace GetHWInfoComputer
{
    [Serializable]
    public class MonitorInfo
    {
        public string ManufacturerName { get; set; }
        public string UserFriendlyName { get; set; }
        public string SerialNumberID { get; set; }

        public MonitorInfo() { }

        public List<MonitorInfo> GetInfo()
        {
            Dictionary<string, string>[] dicpor = GeneralStaticMethods.GetMonitorInfo<MonitorInfo>("WMIMonitorID");
            List<MonitorInfo> lstmonitor = new List<MonitorInfo>();
            if (dicpor != null)
            {
                lstmonitor = (from item in dicpor
                              select new MonitorInfo { ManufacturerName = item["ManufacturerName"], UserFriendlyName = item["UserFriendlyName"], SerialNumberID = item["SerialNumberID"] }).ToList();

                return lstmonitor;
            }
            else
            {
                return null;
            }



        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetHWInfoComputer
{
    [Serializable]
    public class RAMInfo
    {
        public string Tag { get; set; }
        public string PartNumber { get; set; }
        public int Capacity { get; set; }

        public RAMInfo()
        {

        }

        public List<RAMInfo> GetRAM()
        {
            List<RAMInfo> totalRAM = new List<RAMInfo>();
            Dictionary<string, string>[] dicProperties1 = GeneralStaticMethods.GetHardWareInfo<RAMInfo>("Win32_PhysicalMemory");
            if (dicProperties1 != null)
            {

                foreach (Dictionary<string, string> dicprop in dicProperties1)
                {
                    totalRAM.Add(new RAMInfo
                    {
                        Tag = dicprop["Tag"],
                        PartNumber = dicprop["PartNumber"],
                        Capacity = (int)(long.Parse(dicprop["Capacity"]) / (1024 * 1024 * 1024))
                    });
                }
                return totalRAM;
            }
            else
            {
                return null;
            }
        }
    }
}

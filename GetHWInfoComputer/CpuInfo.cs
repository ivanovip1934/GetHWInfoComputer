using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetHWInfoComputer
{
     [Serializable]
    public class CpuInfo
    {
        public string Name { get;  set; }
        public string ProcessorId { get;  set; }
        public CpuInfo() {
         
        }

        public CpuInfo GetInfo() 
        {
            Dictionary<string, string>[] dicProperties = GeneralStaticMethods.GetHardWareInfo<CpuInfo>("Win32_processor");
            this.Name = dicProperties[0]["Name"];
            this.ProcessorId = dicProperties[0]["ProcessorId"];
            return new CpuInfo { Name = this.Name, ProcessorId = this.ProcessorId};
        }
    }
}

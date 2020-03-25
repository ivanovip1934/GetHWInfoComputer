using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetHWInfoComputer
{
     [Serializable]
    public class ComputerInfo
    {
        public string Name { get;  set; }
        public CpuInfo CPU { get;  set; }
        public MainBoardInfo MainBoard { get; set; }
        public List<RAMInfo> Memory { get;  set; }
        public List<DiskInfo> Storage { get;  set; }
        public List<PartitionInfo> Partitions { get;  set; }
        public OperationSystem OS { get;  set; }
        public List<MonitorInfo> Monitors { get;  set; } 
        public DateTime DateCollectedInfo { get; set; }

        public ComputerInfo() { }

        public ComputerInfo(string name) {
            this.Name = name;            
            this.CPU = (new CpuInfo()).GetInfo();
            this.MainBoard = (new MainBoardInfo()).GetInfo();
            this.Memory = (new RAMInfo()).GetRAM();
            this.Storage = (new DiskInfo()).GetStorage();
            this.Partitions = (new PartitionInfo()).GetPartition();
            this.OS = (new OperationSystem()).GetOS();
            this.Monitors = (new MonitorInfo()).GetInfo();
            this.DateCollectedInfo = DateTime.Now;

            
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetHWInfoComputer
{
     [Serializable]
    public class PartitionInfo
    {
        public string Name { get;  set; }
        public bool IsSystem { get;  set; }
        public int FullSize { get;  set; }
        public int AvailableFreeSpace { get;  set; }
        public int IndexDisc { get;  set; }
        

        public PartitionInfo() { 
        
        }


        public List<PartitionInfo> GetPartition()
        {
            List<DetailPartInfo> detailinfo = (new DetailPartInfo()).GetDetailPartInfos();            
            
            List<PartitionInfo> partinfo = new List<PartitionInfo>();
            
            string name = String.Empty;
            int indexDisc = 0;
            bool isSystem;
            int fullSize = 0;
            int availableFreeSpace = 0;

            string SystemPartName = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    name = drive.Name;                    
                    if (name == SystemPartName)
                        isSystem = true;
                    else
                        isSystem = false;

                    indexDisc = GetIndexDisc(name, detailinfo);

                    fullSize = (int)(drive.TotalSize/(1024*1024*1024));
                    availableFreeSpace = (int)(drive.AvailableFreeSpace / (1024 * 1024 * 1024));
                    partinfo.Add(new PartitionInfo
                    {
                        Name = name,
                        IsSystem = isSystem,
                        FullSize = fullSize,
                        AvailableFreeSpace = availableFreeSpace,
                        IndexDisc = indexDisc
                    });

                }
            }

            return partinfo;
        }

        private int GetIndexDisc(string namePartition, List<DetailPartInfo> listdetailinfo) {
            string letterPartition = namePartition.Replace(@":\", "");
            string dependent = $@"\\{Environment.MachineName}\root\cimv2:Win32_LogicalDisk.DeviceID=""{letterPartition}:""";           
            DetailPartInfo detailinfo = listdetailinfo.FirstOrDefault(x => x.Dependent == dependent);
            string antecedent = detailinfo?.Antecedent;
            antecedent = antecedent.Replace($@"\\{Environment.MachineName}\root\cimv2:Win32_DiskPartition.DeviceID=""Disk #", "").Split(',')[0];            
            if (int.TryParse(antecedent, out int indexDisc))
                return indexDisc;
            else {
                Console.WriteLine("Error while parse antecedent");
                return default(int);
            }
        }

    }
    class DetailPartInfo {
        public string Dependent { get; private set; }
        public string Antecedent { get; private set; }

        public DetailPartInfo() { 
        
        }

        public List<DetailPartInfo> GetDetailPartInfos() {
            List<DetailPartInfo> detailinfo = new List<DetailPartInfo>();
            string dependent = String.Empty;
            string antecedent = string.Empty;
            Dictionary<string, string>[] dicProperties = GeneralStaticMethods.GetHardWareInfo<DetailPartInfo>("win32_logicalDisktoPartition");
            foreach (Dictionary<string, string> dicprop in dicProperties) {
                dependent = dicprop["Dependent"];
                antecedent = dicprop["Antecedent"];
                detailinfo.Add(new DetailPartInfo
                {
                    Dependent = dependent,
                    Antecedent = antecedent
                });
            }
            return detailinfo;
        }

    }

}

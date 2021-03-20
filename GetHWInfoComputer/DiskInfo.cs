using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetHWInfoComputer
{
    public class DiskInfo
    {
        public string Model { get; set; }
        public int Index { get; set; }
        public int Size { get; set; }
        public bool IsSSD { get; set; }
        public string InterfaceType { get; set; }
        //private List<StorageInfo> Storages { get; set; }

        public DiskInfo() { }
         
        public List<DiskInfo> GetStorage()
        {
            List<DiskInfo> storages = new List<DiskInfo>();
            int indexDisk = 0;
             string pattern = "^ATA | ATA Device| SCSI Disk Device";     
            string model = String.Empty;
            bool isSSD;
            Dictionary<string, string>[] dicProperties = GeneralStaticMethods.GetHardWareInfo<DiskInfo>("Win32_DiskDrive");
            if (dicProperties != null)
            {
                foreach (Dictionary<string, string> dicprop in dicProperties)
                {
                    if (dicprop["InterfaceType"] != "USB")
                    {
                        indexDisk = int.Parse(dicprop["Index"]);
                        Console.WriteLine(indexDisk);
                        model = Regex.Replace( dicprop["Model"], pattern,"",RegexOptions.IgnoreCase).Trim();
                        Console.WriteLine(model);
                        Console.WriteLine(dicprop["InterfaceType"]);
                        if (dicprop["InterfaceType"] == "SCSI")
                            isSSD = CheckStorageIsSSD.HasNoSeekPenalty($"\\\\.\\PhysicalDrive{indexDisk}");
                        else
                            isSSD = CheckStorageIsSSD.HasNominalMediaRotationRate($"\\\\.\\PhysicalDrive{indexDisk}");
                        storages.Add(new DiskInfo
                        {
                            Model = model,
                            Index = indexDisk,
                            IsSSD = isSSD,
                            Size = (int)(long.Parse(dicprop["Size"]) / (1024 * 1024 * 1024)),
                            InterfaceType = dicprop["InterfaceType"]
                        });
                    }
                }
                return storages;
            }
            else
            {
                return null;
            }
        }
    }
}

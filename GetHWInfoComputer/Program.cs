using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

#region About this app
/*                   -----------  Get H/W info from computer and store to shared folder ------------
       List parameters
       1. Name PC
       2. Model CPU
       3. Model MainBoard
       4. Size RAM
       5. Count moduls RAM
       6. Model, Size and type (hdd or ssd ) every storage 
       7. Free space on system partition.
       8. Model monitor.
       9. Model external videocard if exist.

            using Microsoft.Win32;
            string key = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\lanmanserver\parameters";
            string computerDescription = (string)Registry.GetValue(key, "srvcomment", null);
            computerDescription
*/
#endregion

namespace GetHWInfoComputer
{
    class Program
    {
        static void Main(string[] args)
        {

            GeneralOptions generaloptions = GeneralOptions.Getinstance();
            ComputerInfo PC1 = new ComputerInfo(Environment.MachineName);
            GeneralStaticMethods.MySleep(1000, 150000);

            GeneralStaticMethods.SaveXMLToShare(generaloptions.PathToTempXmlFile, generaloptions.PathToFile, PC1);
            //GeneralStaticMethods.WriteToXmlFile(generaloptions.PathToFile, PC1, false);
            ComputerInfo PC = GeneralStaticMethods.ReadFromXmlFile<ComputerInfo>(generaloptions.PathToFile);

            //Console.WriteLine($"Computer name: {PC.Name}\n" +
            //    $" Date collected info: {PC.DateCollectedInfo}");
            //Console.WriteLine($"Processor name: { PC.CPU.Name}\n" +
            //                   $"Processor ID: {PC.CPU.ProcessorId}\n" +
            //                   $"Mainboard model: {PC.MainBoard.Product}\n" +
            //                   $"Product by: {PC.MainBoard.Manufacturer}\n" +
            //                   $"Mainboard SN: {PC.MainBoard.SerialNumber}\n" +
            //                   $"Version BIOS: {PC.MainBoard.SMBIOSBIOSVersion}\n");


            //Console.WriteLine($"Total memory: {PC.Memory.Sum(x => x.Capacity)} GB.");
            //Console.WriteLine("--------- Storage info -------------------------");
            //Console.WriteLine();
            //foreach (DiskInfo dinfo in PC.Storage)
            //{
            //    Console.WriteLine($"\tIndex: {dinfo.Index}\n" +
            //                      $"\tModel: {dinfo.Model}\n" +
            //                      $"\tIs SSD: {dinfo.IsSSD}\n" +
            //                      $"\tSize: {dinfo.Size} Gb.");
            //    Console.WriteLine();

            //}
            //Console.WriteLine(" ------------ Partition info ---------------------");
            //Console.WriteLine();
            //foreach (PartitionInfo pinfo in PC.Partitions)
            //{
            //    Console.WriteLine($"\tName partition: {pinfo.Name}\n" +
            //                      $"\tPartition is system?: {pinfo.IsSystem}\n" +
            //                      $"\tName disk where store partition: {PC.Storage.FirstOrDefault(x => x.Index == pinfo.IndexDisc).Model}\n" +
            //                      $"\tFull size : {pinfo.FullSize} Gb.\n" +
            //                      $"\tAvailable size: {pinfo.AvailableFreeSpace} Gb.");
            //    Console.WriteLine();

            ////}
            //Console.WriteLine("---------------- OS info ------------------------");
            //Console.WriteLine();
            //Console.WriteLine($"\tOS name: {PC.OS.ProductName}\n" +
            //                  $"\tOS full Version:{PC.OS.Version} {PC.OS.Build}\n" +
            //                  $"\tOS is X64?: {PC.OS.IsX64}\n" +
            //                  $"\tOS install date: {PC.OS.InstallDate}");

            //Console.WriteLine("---------------- Monitors info --------------------");
            //Console.WriteLine();
            //foreach (MonitorInfo minfo in PC.Monitors)
            //{
            //    Console.WriteLine($"\tModel made by: {minfo.ManufacturerName}\n" +
            //                      $"\tSN monitor: {minfo.SerialNumberID}\n" +
            //                      $"\tModel monitor: {minfo.UserFriendlyName}\n");
            //}
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace GetHWInfoComputer
{
    public enum TypeHW { 
    Monitor,
    Disk,
    MainBoard,
    CPU
    }
    public static class GeneralStaticMethods
    {

        public static Dictionary<string, string>[] GetHardWareInfo<T>(string wmiClass) where T : new()
        {
            Dictionary<string, string>[] dicProperty1;
            Dictionary<string, string> dicProperty = new Dictionary<string, string>();
            int searchCount = 0;
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            ManagementScope ms = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2");
            ms.Connect();
            SelectQuery searchQuery = new SelectQuery($"select * from {wmiClass}");
            Console.WriteLine($@"SelectQuery = select * from {wmiClass}");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, searchQuery);
            try
            {
                searchCount = searcher.Get().Count;
                dicProperty1 = new Dictionary<string, string>[searchCount];

                int i = 0;
                foreach (ManagementObject share in searcher.Get())
                {
                    foreach (PropertyData PC in share.Properties)
                    {
                        if (props.Any(x => x.Name == PC.Name))
                        {
                            dicProperty.Add(PC.Name, PC.Value?.ToString());
                        }
                    }
                    dicProperty1[i] = dicProperty;
                    dicProperty = new Dictionary<string, string>();
                    i++;
                }


                return dicProperty1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {t.ToString()}{e.Message}");
                return null;
            }
        }


        public static Dictionary<string, string>[] GetMonitorInfo<T>(string wmiClass) where T : new()
        {
            StringBuilder str;
            Dictionary<string, string>[] dicProperty1;
            Dictionary<string, string> dicProperty = new Dictionary<string, string>();
            int searchCount = 0;
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();           
            ManagementScope ms = new ManagementScope(@"\\.\root\wmi");
            ms.Connect();
            if (ms.IsConnected)
            {
                Console.WriteLine("ms is connected");
            }
            else
            {
                Console.WriteLine("ms is not connected");
            }
            SelectQuery searchQuery = new SelectQuery($"select * from {wmiClass}");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, searchQuery);
            
            // ManagementObjectSearcher searcher = new ManagementObjectSearcher($"select * from  + {wmiClass}");
            // searcher.Scope = ms ;

            try
            {
                ManagementObjectCollection obj = searcher.Get();
                if (obj == null)
                {
                    Console.WriteLine("Collection is  null");
                }
                else
                {
                    Console.WriteLine($"Collection count:  {obj.Count}");
                }
                if (!(obj == null))
                {
                    searchCount = obj.Count;
                    Console.WriteLine($"searchCount = {searchCount}");
                    dicProperty1 = new Dictionary<string, string>[searchCount];


                    int i = 0;
                    foreach (ManagementObject share in obj)
                    {
                        foreach (PropertyData PC in share.Properties)
                        {
                            if (props.Any(x => x.Name == PC.Name))
                            {

                                if (PC.Value is Int16[])
                                {
                                    Int16[] ischar = PC.Value as Int16[];
                                    str = new StringBuilder(50);
                                    foreach (Int16 q in ischar)
                                        str.Append((char)q);
                                    str.Replace("\0", "");
                                    dicProperty.Add(PC.Name, str.ToString());
                                }
                            }
                        }
                        dicProperty1[i] = dicProperty;
                        dicProperty = new Dictionary<string, string>();
                        i++;                        
                    }
                    return dicProperty1;
                }                
                return null;
                
            }
            catch (Exception e)
            {
                return null;            
            }
        }
        public static bool IsVirtualMachine()
        {
            string pattern = "440bx|Virtual|VirtualBox";
            Regex reg = new Regex(pattern,RegexOptions.IgnoreCase);
            Dictionary<string, string>[] dicProperties1 = GetHardWareInfo<MainBoardInfo>("Win32_baseboard");
            string Product = dicProperties1[0]["Product"];            
            if (reg.IsMatch(Product)) {
               return true;
            }
            else{
                return false;
            }
        }

        public static string GetSerialNumber(TypeHW typeHW, string sn) {
                        
            Dictionary<TypeHW, string> dicreg = new Dictionary<TypeHW, string>() {
                {TypeHW.MainBoard,"To be filled by O.E.M" },
                };
            Regex rgx = new Regex(dicreg[typeHW], RegexOptions.IgnoreCase);
            if (rgx.IsMatch(sn))
            {
                return "NO";
            }
            return sn;

        }

        public static void SaveXMLToShare<T>( string sourse, string destination,T objectToWrite) where T: new()
        {
            GeneralStaticMethods.WriteToXmlFile(sourse, (T)objectToWrite);
            
            if (File.Exists(destination))
                File.Delete(destination);

            File.Copy(sourse, destination, true);
            File.Delete(sourse);
        }

        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {

            TextWriter writer = null;
            Stream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(stream);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
            stream.Dispose();
            ReadFromXmlFile<T>(filePath);
        }

        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    reader = new StreamReader(filePath);
                    return (T)serializer.Deserialize(reader);                   
                }
                finally
                {
                    if (reader != null)
                        reader.Dispose();
                }
            }
        }

        public static void MySleep(int minValue, int maxValue)
        {

            int startvalue = 0;
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[1];
            rng.GetBytes(tokenData);
            string Varstr = String.Empty;
            for (int i = 0; i <= tokenData.Length - 1; i++)
            {
                Varstr += tokenData[i].ToString();
            }
            startvalue = int.Parse(Varstr);
            Random rnd = new Random(startvalue);
            Thread.Sleep(rnd.Next(minValue, maxValue));
        }

    }

    
}

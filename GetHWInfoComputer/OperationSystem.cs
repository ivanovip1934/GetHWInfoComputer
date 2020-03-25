using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetHWInfoComputer
{
     [Serializable]
    public class OperationSystem
    {
        public bool IsX64 { get;  set; }
        public string Version { get;  set; }
        public string Build { get;  set; }
        public string ProductName { get;  set; }
        public DateTime InstallDate { get; set; }

        public OperationSystem()
        {

        }

        public OperationSystem GetOS()
        {
            
            string version = String.Empty;
            string productName = String.Empty;
            string build = String.Empty;
            string CurrentVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", "").ToString();
            int Minor =int.Parse(CurrentVersion.Split('.')[1]);
            if (Minor == 1)
            {
                version = CurrentVersion;
                build = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();                
            }
            else if (Minor == 3) { 
                version = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", "").ToString();
                build = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            }
            productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            bool isX64 = Environment.Is64BitOperatingSystem;
            DateTime installDate = GetInstalledDate(version);





            return  new OperationSystem {ProductName = productName, Version = version, Build = build, IsX64 = isX64, InstallDate = installDate };

        }
        private DateTime GetInstalledDateOld()
        {
            DateTime installDate;
            using (var regkey = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
            {
                var key = regkey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                object objValue = key.GetValue("InstallDate");
                string stringValue = objValue.ToString();
                Int64 regVal = Convert.ToInt64(stringValue);
                installDate = startDate.AddSeconds(regVal);
            }
            return installDate;

        }
        private DateTime GetInstalledDate(string versionOS)
        {
            if (versionOS == "10")
            {
                using (var regkey = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
                {
                    var key = regkey.OpenSubKey(@"SOFTWARE\OMSU.VMR\Inventory", false);
                    if (key != null)
                    {
                        object objdate = key.GetValue("InstalledDateOS");
                        if (objdate is null)
                        {
                            return GetInstalledDateOld();
                        }
                        else
                        {
                            string strdate = objdate.ToString();
                            long.TryParse(strdate, out long longdate);
                            Console.WriteLine($"install date from registry {new DateTime(longdate)}");
                            return new DateTime(longdate);
                            
                        }
                    }
                    else
                    {
                        return GetInstalledDateOld();
                    }
                }

            }
            else {
                return GetInstalledDateOld();
            }
        }

private string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;

            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                Console.WriteLine($"OS Major: {vs.Major}");
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        Console.WriteLine($"OS Minor: {vs.Minor}");
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            return operatingSystem;

        }
    }
}

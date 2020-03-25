using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetHWInfoComputer
{
    class GeneralOptions
    {

        public string PathToDir { get; private set; }
        public string PathToFile { get; private set; } 
        public string PathToTempXmlFile { get; private set; }
        
        

        private GeneralOptions() {
            Random rnd = new Random();
            this.PathToTempXmlFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine), $"GetGWInfo{rnd.Next(1000, 2000)}.xml");
            this.PathToDir = @"\\fileserv.omsu.vmr\inventory$\ARM";
            this.PathToFile = Path.Combine(this.PathToDir, $"{Environment.MachineName}.xml");        
        }
        private static GeneralOptions generalconfig = new GeneralOptions();

        public static GeneralOptions Getinstance() {
            return generalconfig;
        }
    }
}

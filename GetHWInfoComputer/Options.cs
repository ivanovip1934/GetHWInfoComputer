using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GetHWInfoComputer
{
    class GeneralOptions
    {
        private readonly string fileName = "Config.xml";
        public string PathToDirStoreXML { get; private set; }
        public string PathToFile { get; private set; }
        public string PathToTempXmlFile { get; private set; }



        private GeneralOptions()
        {
            GetConfig(fileName);
        }
        private static GeneralOptions generalconfig = new GeneralOptions();

        public static GeneralOptions Getinstance()
        {
            return generalconfig;
        }
        private void GetConfig(string filename)
        {
            Random rnd = new Random();
            this.PathToTempXmlFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine), $"GetHWInfo{rnd.Next(1000, 2000)}.xml");
            this.PathToDirStoreXML = @"\\fileserv.omsu.vmr\inventory$\ARM1";
            

            string pathtoconfig = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
            //PathToDirXML = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "XML");

            Dictionary<string, Action<XmlNode>> dic = new Dictionary<string, Action<XmlNode>>
            {
                ["PathToDirXML"] = x => { this.PathToDirStoreXML = CheckDirectory(x.InnerText.ToString()) ? x.InnerText.ToString() : this.PathToDirStoreXML; }
            };

            try
            {
                using (Stream stream = new FileStream(pathtoconfig, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(stream);
                    XmlElement XRoot = xDoc.DocumentElement;
                    foreach (XmlNode xnode in XRoot)
                    {
                        if (dic.ContainsKey(xnode.Name.ToString()))
                            dic[xnode.Name.ToString()].Invoke(xnode);
                    }
                }
            }
            catch (Exception e)
            {
                //check here why it failed and ask user to retry if the file is in use.
                Console.WriteLine(e.Message);
            }

            this.PathToFile = Path.Combine(this.PathToDirStoreXML, $"{Environment.MachineName}.xml");

        }

        private bool CheckDirectory(string pathToDir)
        {
            if (String.IsNullOrEmpty(pathToDir))
            {
                Console.WriteLine($"Warning: PathToDirStoreXML in Config.xml - not set.");
                return false;
            }
            if (Directory.Exists(pathToDir))
            {
                return true;
            }
            Console.WriteLine($"Error: path to directory 'XML' {PathToDirStoreXML}  from Config.xml NOT exists!!!");
            return false;
        }
    }
}

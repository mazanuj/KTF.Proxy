using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;

namespace KTF.Proxy.Storage
{
    public class FileStorage
    {
        /// <summary>
        /// Path to file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Create instance of FileStorage with default path to storage file
        /// </summary>
        public FileStorage()
        {
            FilePath = DefaultFilename;
        }

        /// <summary>
        /// Create instance of FileStorage with custom path to storage file
        /// </summary>
        /// <param name="path">Path to file</param>
        public FileStorage(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// Default file name for storing proxies
        /// </summary>
        public const string DefaultFilename = "proxies.txt";

        /// <summary>
        /// Load proxies from file. File should contain strings in format 'Address:Port'
        /// </summary>       
        public IEnumerable<WebProxy> LoadFromFile()
        {
            List<WebProxy> proxies = new List<WebProxy>();

            string line = null;
            if (!File.Exists(FilePath)) return null;

            TextReader readFile = new StreamReader(FilePath);

            while (true)
            {
                line = readFile.ReadLine();
                if (line != null)
                {
                    var adress = line.Trim();
                    if (adress != "")
                    {
                        var parts = adress.Split(':');
                        int _port = 0;
                        if (parts.Count() == 2 && Int32.TryParse(parts[1], out _port))
                        {
                            proxies.Add(new WebProxy(parts[0], _port));
                        }
                    }
                }
                else
                    break;
            }
            readFile.Close();
            readFile = null;

            return proxies;
        }

        /// <summary>
        /// Save proxies to file in format 'Address:Port'
        /// </summary>
        /// <param name="proxies">Proxies to save</param>
        public void SaveToFile(IEnumerable<WebProxy> proxies)
        {
            if (proxies.Count() == 0) return;

            System.IO.TextWriter writeFile = new StreamWriter(FilePath);
            foreach (var proxy in proxies)
            {
                if (proxy != null)
                    writeFile.WriteLine(proxy.Address.Host + ":" + proxy.Address.Port);
            }
            writeFile.Flush();
            writeFile.Close();
            writeFile = null;
        }
    }
}

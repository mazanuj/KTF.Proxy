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
            var proxies = new List<WebProxy>();

            if (!File.Exists(FilePath)) return null;

            TextReader readFile = new StreamReader(FilePath);

            while (true)
            {
                var line = readFile.ReadLine();
                if (line != null)
                {
                    var adress = line.Trim();
                    if (adress == "") continue;
                    var parts = adress.Split(':');
                    int _port;
                    if (parts.Count() == 2 && Int32.TryParse(parts[1], out _port))
                    {
                        proxies.Add(new WebProxy(parts[0], _port));
                    }
                }
                else
                    break;
            }
            readFile.Close();

            return proxies;
        }

        /// <summary>
        /// Save proxies to file in format 'Address:Port'
        /// </summary>
        /// <param name="proxies">Proxies to save</param>
        public void SaveToFile(IEnumerable<WebProxy> proxies)
        {
            var webProxies = proxies as IList<WebProxy> ?? proxies.ToList();
            if (!webProxies.Any()) return;

            TextWriter writeFile = new StreamWriter(FilePath);
            foreach (var proxy in webProxies.Where(proxy => proxy != null))
            {
                writeFile.WriteLine(proxy.Address.Host + ":" + proxy.Address.Port);
            }
            writeFile.Flush();
            writeFile.Close();
        }
    }
}

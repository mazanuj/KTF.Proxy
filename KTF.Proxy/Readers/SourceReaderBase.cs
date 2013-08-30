using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace KTF.Proxy.Readers
{
    public abstract class SourceReaderBase : IProxySourceReader
    {
        public abstract IEnumerable<System.Net.WebProxy> GetProxies(string country, ConnectionType type, string port, System.Threading.CancellationToken cs);

        public IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port)
        {
            return GetProxies(country, type, port, CancellationToken.None);
        }


        public abstract string Name { get; }
    }
}

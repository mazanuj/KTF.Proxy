using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KTF.Proxy.Readers
{
    public abstract class SourceReaderBase : IProxySourceReader
    {
        public abstract IEnumerable<System.Net.WebProxy> GetProxies(string country, ConnectionType type, string port, System.Threading.CancellationTokenSource cs);

        public IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port)
        {
            return GetProxies(country, type, port, null);
        }
    }
}

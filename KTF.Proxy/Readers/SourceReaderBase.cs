using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace KTF.Proxy.Readers
{
    public abstract class SourceReaderBase : IProxySourceReader
    {
        public abstract IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port, CancellationToken cs);

        public IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port)
        {
            return GetProxies(country, type, port, CancellationToken.None);
        }


        public abstract string Name { get; }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace KTF.Proxy.Readers
{
    public interface IProxySourceReader
    {
        IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port, CancellationToken cs);
        IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port);

        string Name { get; }
    }

    public enum ConnectionType
	{
	    Any,
        Http,
        Https
	}
}

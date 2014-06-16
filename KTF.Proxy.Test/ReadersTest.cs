using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KTF.Proxy.Readers;
using System.Threading;

namespace KTF.Proxy.Test
{
    [TestClass]
    public class ReadersTest
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void CheckerproxyCancelation()
        {
            CheckerproxySourceReader c = new CheckerproxySourceReader();
            CancellationTokenSource canc = new CancellationTokenSource();
            canc.Cancel();
            c.GetProxies("", ConnectionType.Any, "",canc);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void FreeproxyCancelation()
        {
            FreeproxySourceReader c = new FreeproxySourceReader();
            CancellationTokenSource canc = new CancellationTokenSource();
            canc.Cancel();
            c.GetProxies("", ConnectionType.Any, "", canc);
        }
    }
}

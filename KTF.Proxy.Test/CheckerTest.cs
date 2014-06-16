using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace KTF.Proxy.Test
{
    [TestClass]
    public class CheckerTest
    {
        [TestMethod]
        public void InvalidProxy()
        {
            ProxyChecker checker = new ProxyChecker();
            Assert.IsFalse(checker.CheckProxy(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullProxies()
        {
            new ProxyChecker().GetTestedProxies(null, CancellationToken.None);
        }

        [TestMethod]
        public void EmptyProxies()
        {
            Assert.AreEqual(0, new ProxyChecker().GetTestedProxies(new List<WebProxy>(), CancellationToken.None).Count());
        }
    }
}

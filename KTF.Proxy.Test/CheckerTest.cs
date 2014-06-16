using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            new ProxyChecker().GetTestedProxies(null, null);
        }

        [TestMethod]
        public void EmptyProxies()
        {
            Assert.AreEqual(0, new ProxyChecker().GetTestedProxies(new List<WebProxy>(), null).Count());
        }
    }
}

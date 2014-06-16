using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace KTF.Proxy
{
    public class ProxyChecker
<<<<<<< HEAD
    {        
=======
    {
        const string UrlToCheck = "http://www.yandex.ru/";

>>>>>>> parent of 91d9a64... improvements
        /// <summary>
        /// Average time in millisecond for checking 1 proxy
        /// </summary>
        //public const int AverageChekingTime = 430;        

        private int Timeout { get; set; }
        private string UrlToCheck { get; set; }
        

        public event CheckedEventHandler Checked;

        public ProxyChecker(int timeout, string server)
        {
            Timeout = timeout;
            UrlToCheck = server;
        }

        private void OnChecked(WebProxyEventArgs e)
        {
            if (Checked != null)
                Checked(this, e);
        }

        /// <summary>
        /// Check for availability through sending request to default server
        /// </summary>
        /// <param name="proxy">Proxy to check</param>
        /// <exception cref="System.OperationCanceledException"/>
        private bool CheckProxy(WebProxy proxy)
        {
            if (proxy == null) return false;

            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(UrlToCheck);
                myHttpWebRequest.AllowAutoRedirect = false;
                myHttpWebRequest.Proxy = proxy;
<<<<<<< HEAD
                myHttpWebRequest.Timeout = Timeout;
                myHttpWebRequest.KeepAlive = false;
                var httpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
=======
                myHttpWebRequest.Timeout = 1200;
                var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
>>>>>>> parent of 91d9a64... improvements

                OnChecked(new WebProxyEventArgs(proxy, true));
                return true;
            }
            catch (WebException)
            {
                OnChecked(new WebProxyEventArgs(proxy, false));
                return false;
            }
        }

        /// <summary>
        /// Returns available proxies that gave response during set time
        /// </summary>
        /// <param name="proxies">Proxies to check</param>
        /// <param name="cs">CancellationTokenSource. Null if not needed</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OperationCanceledException"/>
        public IEnumerable<WebProxy> GetTestedProxies(IEnumerable<WebProxy> proxies, CancellationTokenSource cs = null)
        {
            if (proxies == null) throw new ArgumentNullException();
            Debug.WriteLine("Checking proxies with " + UrlToCheck);

<<<<<<< HEAD
            var prox = new List<WebProxy>(proxies.AsParallel().WithDegreeOfParallelism(10).WithCancellation(cs).Where(CheckProxy));
            Trace.WriteLine("Checking done");
=======
            List<WebProxy> prox = null;
            if(cs == null)
                prox = new List<WebProxy>(proxies.AsParallel().WithDegreeOfParallelism(10).Where(CheckProxy));
            else
                prox = new List<WebProxy>(proxies.AsParallel().WithDegreeOfParallelism(10).WithCancellation(cs.Token).Where(CheckProxy));

            Debug.WriteLine("Checking done");
>>>>>>> parent of 91d9a64... improvements

            return prox;
        }

    }

    public delegate void CheckedEventHandler(object sender, WebProxyEventArgs e);

    public class WebProxyEventArgs : EventArgs
    {
        public WebProxy WebProxy { get; private set; }

        public bool IsSuccess { get; private set; }

        public WebProxyEventArgs(WebProxy proxy, bool success)
        {
            WebProxy = proxy;
            IsSuccess = success;
        }
    }

}

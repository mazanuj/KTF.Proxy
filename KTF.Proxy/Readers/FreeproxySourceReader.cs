using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;

namespace KTF.Proxy.Readers
{
    public class FreeproxySourceReader : SourceReaderBase
    {
        public override IEnumerable<System.Net.WebProxy> GetProxies(string country, ConnectionType type, string port, System.Threading.CancellationToken cs)
        {
            List<WebProxy> proxies = new List<WebProxy>();

            string url = "http://2freeproxy.com/wp-content/plugins/proxy/load_proxy.php";
            string post = "type=standard";

            Debug.WriteLine("Sending request to " + url);

            string json = null;

            byte[] b = System.Text.Encoding.UTF8.GetBytes(post);
            var myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            myHttpWebRequest.ContentLength = b.Length;
            myHttpWebRequest.Referer = "http://2freeproxy.com/standart-ports-proxy.html";
            myHttpWebRequest.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            myHttpWebRequest.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.5";
            myHttpWebRequest.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            myHttpWebRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
            myHttpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";

            myHttpWebRequest.GetRequestStream().Write(b, 0, b.Length);
            myHttpWebRequest.AllowAutoRedirect = false;

            var asyncResult = myHttpWebRequest.BeginGetResponse(null, null);
            WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, cs.WaitHandle });
            if (cs.IsCancellationRequested)
            {
                myHttpWebRequest.Abort();
                throw new OperationCanceledException();
            }

            var myHttpWebResponse = myHttpWebRequest.EndGetResponse(asyncResult);
            Debug.WriteLine("Response is received");
            Stream responseStream = responseStream = myHttpWebResponse.GetResponseStream();
            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            System.IO.StreamReader sr = new System.IO.StreamReader(responseStream, Encoding.Default);
            json = sr.ReadToEnd();
            sr.Close();

            Debug.WriteLine("Parse response in JSON format");
            var addresses = JObject.Parse(json)["proxy"].ToString().Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            Debug.WriteLine("Processing response");
            foreach (var address in addresses)
            {
                if (cs != null && cs.IsCancellationRequested)
                    throw new OperationCanceledException();
                proxies.Add(new WebProxy(address));
            }

            Debug.WriteLine("Proxies loaded successfully");

            return proxies;
        }

        public override string Name
        {
            get { return "Freeproxy.com"; }
        }
    }
}

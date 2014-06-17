using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using HtmlAgilityPack;

namespace KTF.Proxy.Readers
{
    public class CheckerproxySourceReader : SourceReaderBase
    {
        public override IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port, CancellationToken cs)
        {
            var proxies = new List<WebProxy>();

            string url;
            if (DateTime.Now.Hour > 15)
                url = "http://checkerproxy.net/ru/" + DateTime.Now.ToString("dd-MM-yyyy");
            else
                url = "http://checkerproxy.net/ru/" + DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");

            Trace.WriteLine("Sending request to " + url);

            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Proxy = WebRequest.DefaultWebProxy;

            var asyncResult = myHttpWebRequest.BeginGetResponse(null, null);

            WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, cs.WaitHandle });
            if (cs.IsCancellationRequested)
            {
                myHttpWebRequest.Abort();
                throw new OperationCanceledException();
            }


            var myHttpWebResponse = myHttpWebRequest.EndGetResponse(asyncResult);
            var sr = new StreamReader(myHttpWebResponse.GetResponseStream());
            var HtmlResult = sr.ReadToEnd();
            sr.Close();
            Trace.WriteLine("Response is received");

            Trace.WriteLine("Parse response");
            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlResult);

            var limit = doc.GetElementbyId("result-box-table").LastChild.ChildNodes.Count();

            Trace.WriteLine("Processing response");
            for (var i = 0; i < limit; i++)
            {
                if (cs.IsCancellationRequested)
                    throw new OperationCanceledException();

                var node = doc.GetElementbyId("result-box-table").LastChild.ChildNodes[i];
                var adress = node.ChildNodes[3].InnerText;
                var _country = node.ChildNodes[5].InnerText;
                var _type = node.ChildNodes[7].InnerText;
                var post = node.ChildNodes[11].InnerText == "+";
                var cookie = node.ChildNodes[13].InnerText == "+";
                var referer = node.ChildNodes[15].InnerText == "+";
                var proxyip = node.ChildNodes[17].InnerText;
                var high = !proxyip.Contains("IP");

                if (high && post && cookie && referer)
                {
                    if (type != ConnectionType.Any)
                    {
                        if (type.ToString().ToLower() != _type.ToLower())
                            continue;
                    }

                    if (country == "ru")
                    {
                        if (!_country.Contains("Российская"))
                            continue;
                    }

                    var parts = adress.Split(':');
                    if (port != "" && (port.ToLower().Trim() != parts[1].ToLower().Trim()))
                        continue;

                    int _port;
                    if (!Int32.TryParse(parts[1], out _port)) continue;
                    if (proxies.All(p => p.Address != new Uri("http://" + adress)))
                        proxies.Add(new WebProxy(parts[0], _port));
                }
            }

            Trace.WriteLine("Proxies loaded successfully");

            return proxies;
        }

        public override string Name
        {
            get { return "Checkerproxy.net"; }
        }
    }
}

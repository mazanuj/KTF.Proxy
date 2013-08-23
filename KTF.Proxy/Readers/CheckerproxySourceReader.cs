﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;

namespace KTF.Proxy.Readers
{
    public class CheckerproxySourceReader : SourceReaderBase
    {
        public override IEnumerable<WebProxy> GetProxies(string country, ConnectionType type, string port, CancellationTokenSource cs)
        {
            List<WebProxy> proxies = new List<WebProxy>();

            string url = "";
            if(DateTime.Now.Hour > 15)
                url = "http://checkerproxy.net/ru/" + DateTime.Now.ToString("dd-MM-yyyy");
            else
                url = "http://checkerproxy.net/ru/" + DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");

            Debug.WriteLine("Sending request to " + url);

            string HtmlResult = null;

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.AllowAutoRedirect = false;

            var asyncResult = myHttpWebRequest.BeginGetResponse(null, null);
            if (cs != null)
            {
                WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle, cs.Token.WaitHandle });
                if (cs.Token.IsCancellationRequested)
                {
                    myHttpWebRequest.Abort();
                    throw new OperationCanceledException();
                }
            }
            else
                WaitHandle.WaitAny(new[] { asyncResult.AsyncWaitHandle });

            var myHttpWebResponse = myHttpWebRequest.EndGetResponse(asyncResult);
            Debug.WriteLine("Response is received");
            System.IO.StreamReader sr = new System.IO.StreamReader(myHttpWebResponse.GetResponseStream());
            HtmlResult = sr.ReadToEnd();
            sr.Close();

            Debug.WriteLine("Parse response");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlResult);

            int limit = doc.GetElementbyId("result-box-table").LastChild.ChildNodes.Count();

            Debug.WriteLine("Processing response");
            for (int i = 0; i < limit; i++)
            {
                if (cs != null && cs.IsCancellationRequested)
                    throw new OperationCanceledException();

                var node = doc.GetElementbyId("result-box-table").LastChild.ChildNodes[i];
                var adress = node.ChildNodes[3].InnerText;
                var _country = node.ChildNodes[5].InnerText;
                var _type = node.ChildNodes[7].InnerText;
                bool post = node.ChildNodes[11].InnerText == "+";
                bool cookie = node.ChildNodes[13].InnerText == "+";
                bool referer = node.ChildNodes[15].InnerText == "+";
                var proxyip = node.ChildNodes[17].InnerText;
                bool high = !proxyip.Contains("IP");

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

                    int _port = 0;
                    if (Int32.TryParse(parts[1], out _port))
                    {
                        if (!proxies.Any(p => p.Address == new Uri("http://" + adress)))
                            proxies.Add(new WebProxy(parts[0], _port));
                    }
                }
            }

            Debug.WriteLine("Proxies loaded successfully");

            return proxies;
        }
    }
}
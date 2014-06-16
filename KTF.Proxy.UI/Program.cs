using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KTF.Proxy.Readers;
using KTF.Proxy.Storage;

namespace KTF.Proxy.UI
{
    class Program
    {
        static int total = 0;
        static int check = 0;
        static void Main(string[] args)
        {
            var r = new FreeproxySourceReader();
            var c = new ProxyChecker();
            c.Checked += c_Checked;
            
            var res = r.GetProxies("", ConnectionType.Any, "");
            total = res.Count();

            Console.WriteLine("Readed: " + total);

            var s = new Stopwatch();
            s.Restart();
            Console.WriteLine();

            var tes = c.GetTestedProxies(res, CancellationToken.None);
            Console.WriteLine("Working: " + tes.Count());
            s.Stop();
            Console.WriteLine("Ellapsed: " + s.ElapsedMilliseconds / 1000 + "s");

        }

        static void c_Checked(object sender, WebProxyEventArgs e)
        {
            check++;
            if (check % 3 == 0)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("\t\t\t\t\t");
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("{0:p1}", ((double)check / total));
            }
        }
    }
}

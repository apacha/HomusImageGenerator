using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomusImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://grfia.dlsi.ua.es/homus/HOMUS.zip", Path.Combine("..","..", "..", "HOMUS.zip"));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.Utilities
{
    public static class CommonFunctions
    {
        public static string GenerateTimestampFilename()
        {
            return DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
        }

        public static List<string> determineIP()
        {
            List<string> detected_ips = new List<string>();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    detected_ips.Add(ip.ToString());
                }
            }

            return detected_ips;
        }
        

    }
}

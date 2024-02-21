using System;
using System.Collections.Generic;
using System.Linq;
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

        /*
         * 
        private void determineIP()
        {
            // get ip addresses
            debug("Get Local IP: ");

            int ipCount = 0;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    debug(ip.ToString());
                    localip = ip.ToString();
                    ipCount += 1;
                }
            }

            if (ipCount == 0)
            {
                debug("Warning: No Local IP Address Detected");
            }
            else
            {
                if (ipCount > 1)
                {
                    debug("Warning: Multiple IP Addresses Detected for this pc");
                }
            }

        }
        
         */

    }
}

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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public class NimConfig
    {
        public UInt32 frequency { get; set; }
        public UInt32 symbol_rate { get; set; }

        public override string ToString() 
        {
            return "Config: Freq: " + frequency.ToString() + "," + symbol_rate.ToString();
        }

    }
}

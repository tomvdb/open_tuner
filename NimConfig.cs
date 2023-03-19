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

        public bool polarization_supply { get; set; }
        public bool polarization_supply_horizontal { get; set; }

        public override string ToString() 
        {
            return "Config: Freq: " + frequency.ToString() + "," + symbol_rate.ToString() + " - LNB Supply :" + polarization_supply.ToString() + ", Horiz Supply :" + polarization_supply_horizontal.ToString();
        }

    }
}

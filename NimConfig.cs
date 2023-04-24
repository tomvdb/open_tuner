using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public class NimConfig
    {
        // tuner / demod specific

        public byte tuner { get; set; }

        public UInt32 frequency { get; set; }
        public UInt32 symbol_rate { get; set; }
        public uint rf_input { get; set; }

        // misc 
        public bool polarization_supply { get; set; }
        public bool polarization_supply_horizontal { get; set; }

        public bool tone_22kHz_P1 { get; set; }

        public override string ToString() 
        {
            return "Config: Freq: " + frequency.ToString() + "," + symbol_rate.ToString() + " - LNB Supply :" + polarization_supply.ToString() + ", Horiz Supply :" + polarization_supply_horizontal.ToString() + ", RF Input : " + rf_input.ToString();
        }

    }
}

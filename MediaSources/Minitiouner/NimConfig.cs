using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public class TunerConfig
    {
        // tuner / demod specific

        public byte tuner { get; set; }

        public UInt32 frequency { get; set; }
        public UInt32 symbol_rate { get; set; }
        public uint rf_input { get; set; }

        // misc 
        
        public byte lnba_psu { get; set; }
        public byte lnbb_psu { get; set; }

        public bool tone_22kHz_P1 { get; set; }

        public override string ToString() 
        {
            return "Config: Freq: " + frequency.ToString() + "," + symbol_rate.ToString() + " - LNB Supply :" + lnba_psu.ToString() + "," + lnbb_psu.ToString()  + ", RF Input : " + rf_input.ToString();
        }

    }
}

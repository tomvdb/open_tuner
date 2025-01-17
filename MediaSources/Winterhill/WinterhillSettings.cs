using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.MediaSources.Winterhill
{ 
    public class WinterhillSettings
    {
        // websocket
        public string WinterhillWSHost = "192.168.0.122";
        public int WinterhillWSPort = 8080;
        public int WinterhillWSUdpBasePort = 9900;

        public byte DefaultInterface = 1;

        public string WinterhillUdpHost = "192.168.0.124";
        public int WinterhillUdpBasePort = 9900;


        public uint[] RFPort = new uint[] { 0, 0, 0, 0 };

        
        // lnba1, lnbb1, lnba2, lnbb2
        public uint[] LNBVoltage = new uint[] { 0, 0 };

        public uint[] DefaultFrequency = new uint[] { 10491500, 10491500, 10491500, 10491500 };
        public uint[] DefaultSR = new uint[] { 1500, 1500, 1500, 1500 };
        public uint[] DefaultOffset = new uint[] { 9750000, 9750000, 9750000, 9750000 };
        public uint[] DefaultVolume = new uint[] { 50, 50, 50, 50 };
        public bool[] DefaultMuted = new bool[] { true, true, true, true };
        public bool[] DefaultUDPStreaming = new bool[] { false, false, false, false };
    }
}

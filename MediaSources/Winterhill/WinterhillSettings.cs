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

        public byte DefaultInterface = 1;

        public int WinterhillUdpBasePort = 9900;

        public uint[] Offset = new uint[] { 9750000, 9750000, 9750000, 9750000 };
        public uint[] DefaultVolume = new uint[] { 50, 50, 50, 50 };
        public bool[] DefaultMuted = new bool[] { true, true, true, true };
    }
}

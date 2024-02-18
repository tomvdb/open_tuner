using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.MediaSources.Longmynd
{
    public class LongmyndSettings
    {
        // websocket
        public string LongmyndWSHost = "192.168.0.109";        
        public int LongmyndWSPort = 8080;

        // mqtt
        public string LongmyndMqttHost = "192.168.0.178";
        public int LongmyndMqttPort = 1883;
        public string CmdTopic = "cmd/longmynd/";

        public byte DefaultInterface = 0;   // 0 = always ask, 1 = longmynd ws, 2 = longmynd mqtt

        public uint Offset1 = 9750000;
        public int DefaultVolume1 = 0;

    }
}

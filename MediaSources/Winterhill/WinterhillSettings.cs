namespace opentuner.MediaSources.WinterHill
{ 
    public class WinterHillSettings
    {
        // websocket
        public string WinterHillWSHost = "192.168.0.122";
        public int WinterHillWSPort = 8080;
        public int WinterHillWSUdpBasePort = 9900;

        public byte DefaultInterface = 1;

        public string WinterHillUdpHost = "192.168.0.124";
        public int WinterHillUdpBasePort = 9900;


        public uint[] RFPort = new uint[] { 0, 0, 0, 0 };

        
        // lnba1, lnbb1, lnba2, lnbb2
        public uint[] LNBVoltage = new uint[] { 0, 0 };

        public uint[] DefaultFrequency = new uint[] { 10491500, 10491500, 10491500, 10491500 };
        public uint[] DefaultSR = new uint[] { 1500, 1500, 1500, 1500 };
        public uint[] DefaultOffset = new uint[] { 9750000, 9750000, 9750000, 9750000 };
        public uint[] DefaultVolume = new uint[] { 50, 50, 50, 50 };
        public bool[] DefaultMuted = new bool[] { true, true, true, true };
    }
}

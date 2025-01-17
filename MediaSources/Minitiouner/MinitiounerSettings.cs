using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.MediaSources.Minitiouner
{
    public class MinitiounerSettings
    {
        public int Version = 2;

        public byte DefaultInterface = 0;   // 0 = always ask, 0 = FTDI, 2 = PicoTuner, 3 = Ethernet (Future)

        public uint Offset1 = 9750000;
        public uint Offset2 = 9750000;

        public byte DefaultLnbASupply = 0;   // 0 = off, 1 = vert, 2 = horiz
        public byte DefaultLnbBSupply = 0;   // 0 = off, 1 = vert, 2 = horiz

        public byte DefaultRFInput = 0;     // 0 = both tuners fed through A, 1 = Tuner1 is A, Tuner2 is B

        public uint[] DefaultVolume = new uint[] { 50, 50 };
        public bool[] DefaultMuted = new bool[] { true, true };
        public bool[] DefaultUDPStreaming = new bool[] { false, false };
    }
}

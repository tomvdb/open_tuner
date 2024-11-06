using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.QuickTuneControl
{
    public class QuickTuneControlSettings
    {
        public int[] UDPListenPorts = { 6789, 6790, 6791, 6792 };
        public int[] TuningMode = { 0, 0, 0, 0 };
        public bool[] AvoidBeacon = { true, true, true, true };
    }
}

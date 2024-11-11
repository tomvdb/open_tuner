using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.BATCSpectrum
{
    public class BATCSpectrumSettings
    {
        public int[] tuneMode = { 0, 0, 0, 0 };
        public bool[] avoidBeacon = { false, false, false, false };
        public int overPowerIndicatorLayout = 0;
    }
}

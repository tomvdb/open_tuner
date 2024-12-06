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
        public bool[] avoidBeacon = { true, true, true, true };
        public float treshHold = -6.0f;
        public int autoHoldTimeValue = 5;
        public int autoTuneTimeValue = 30;
        public int overPowerIndicatorLayout = 0;
    }
}

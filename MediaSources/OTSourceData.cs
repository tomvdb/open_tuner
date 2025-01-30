using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.MediaSources
{
    public class OTSourceData
    {
        public int video_number = 0;
        public bool demod_locked = false;
        public double db_margin = 0.0;
        public double mer = 0.0;
        public long frequency = 0;
        public int symbol_rate = 0;
        public string service_name = "";
        public string modcode = "";
        public int volume = 0;
        public bool streaming = false;
        public bool recording = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.DATVReporter
{
    public class ISawMessage
    {
        public string target_callsign;
        public string observer_callsign;
        public string observer_gridlocator;
        public string observed_time;
        public string application;
        public string application_version;
        public double db_margin;
        public double mer;
        public long frequency;
        public int symbol_rate;

        public ISawMessage(string callsign, double db_margin, double mer, long frequency, int symbol_rate)
        {
            this.target_callsign = callsign;
            this.observed_time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            this.db_margin = db_margin;
            this.mer = mer;
            this.frequency = frequency;
            this.symbol_rate = symbol_rate;
        }
    }
}

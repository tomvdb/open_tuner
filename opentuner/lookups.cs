using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public static class lookups
    {
        public static Dictionary<int, string> demod_state_lookup = new Dictionary<int, string>()
        {
            { 0 , "Hunting" },
            { 1 , "Header" },
            { 2 , "Lock DVB-S2" },
            { 3 , "Lock DVB-S" }
        };

        public static Dictionary<uint, string> modcod_lookup_dvbs = new Dictionary<uint, string>()
        {
            { 4 , "QPSK 1/2" },
            { 5 , "QPSK 3/5" },
            { 6 , "QPSK 2/3" },
            { 7 , "QPSK 3/4" },
            { 9 , "QPSK 5/6" },
            { 10 , "QPSK 6/7" },
            { 11 , "QPSK 7/8" }
        };

        // values obtained from longmynd.py in rydeplayer
        public static Dictionary<uint, double> modcod_lookup_dvbs_threshold = new Dictionary<uint, double>()
        {
            { 4 , 1.7 },
            { 5 , 4.8 }, // not sure about this one
            { 6 , 3.3 },
            { 7 , 4.2 },
            { 9 , 5.1 },
            { 10 , 5.5  },
            { 11 , 5.8 }
        };

        public static Dictionary<uint, string> modcod_lookup_dvbs2 = new Dictionary<uint, string>()
        {
            { 0, "DummyPL"},
            {  1, "QPSK 1/4"},
            {  2, "QPSK 1/3"},
            {  3, "QPSK 2/5"},
            {  4, "QPSK 1/2"},
            {  5, "QPSK 3/5"},
            {  6, "QPSK 2/3"},
            {  7, "QPSK 3/4"},
            {  8, "QPSK 4/5"},
            {  9, "QPSK 5/6"},
            {  10, "QPSK 8/9"},
            {  11, "QPSK 9/10"},
            {  12, "8PSK 3/5"},
            {  13, "8PSK 2/3"},
            {  14, "8PSK 3/4"},
            {  15, "8PSK 5/6"},
            {  16, "8PSK 8/9"},
            {  17, "8PSK 9/10"},
            {  18, "16APSK 2/3"},
            {  19, "16APSK 3/4"},
            {  20, "16APSK 4/5"},
            {  21, "16APSK 5/6"},
            {  22, "16APSK 8/9"},
            {  23, "16APSK 9/10"},
            {  24, "32APSK 3/4"},
            {  25, "32APSK 4/5"},
            {  26, "32APSK 5/6"},
            {  27, "32APSK 8/9"},
            {  28, "32APSK 9/10"}
        };


        public static Dictionary<uint, double> modcod_lookup_dvbs2_threshold = new Dictionary<uint, double>()
        {
            { 0, 0},
            {  1, -2.3},
            {  2, -1.2},
            {  3, -0.3},
            {  4, 1.0},
            {  5, 2.3},
            {  6, 3.1},
            {  7, 4.1},
            {  8, 4.7},
            {  9, 5.2},
            {  10, 6.2},
            {  11, 6.5},
            {  12, 5.5},
            {  13, 6.6},
            {  14, 7.9},
            {  15, 9.4},
            {  16, 10.7},
            {  17, 11.0},
            {  18, 9.0},
            {  19, 10.2},
            {  20, 11.0},
            {  21, 11.6},
            {  22, 12.9},
            {  23, 13.2},
            {  24, 12.8},
            {  25, 13.7},
            {  26, 14.3},
            {  27, 15.7},
            {  28, 16.1}
        };

        public static Dictionary<int, string> mpeg_type_lookup = new Dictionary<int, string>()
        {
            { 1, "MPEG1 Video" },
            { 3, "MPEG1 Audio"},
            { 15, "AAC Audio"},
            { 16, "H.263 Video"},
            { 27, "H.264 Video"},
            { 33, "JPEG2K Video"},
            { 36, "H.265 Video"},
            { 129, "AC3 Audio"}
        };
    }
}

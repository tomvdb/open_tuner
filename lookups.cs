using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public static class lookups
    {
        public static List<string> stream_format_lookups = new List<string>()
        {
            "Generic Packetized",
            "Generic Continuous",
            "Generic Packetized", // ?
            "Transport"
        };

        // https://wiki.batc.org.uk/MiniTiouner_Power_Level_Indication
        public static List<int> agc1_lookup = new List<int>()
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            10,
            21800,
            25100,
            27100,
            28100,
            28900,
            29600,
            30100,
            30550,
            31000,
            31350,
            31700,
            32050,
            32400,
            32700,
            33000,
            33300,
            33600,
            33900,
            34200,
            34500,
            34750,
            35000,
            35250,
            35500,
            35750,
            36000,
            36200,
            36400,
            36600,
            36800,
            37000,
            37200,
            37400,
            37600,
        };

        // https://wiki.batc.org.uk/MiniTiouner_Power_Level_Indication
        public static List<int> agc2_lookup = new List<int>()
        {
            3200,
            2740,
            2560,
            2380,
            2200,
            2020,
            1840,
            1660,
            1480,
            1300,
            1140,
            1000,
            880,
            780,
            700,
            625,
            560,
            500,
            450,
            400,
            360,
            325,
            290,
            255,
            225,
            200,
            182,
            164,
            149,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
            148,
        };

        // https://wiki.batc.org.uk/MiniTiouner_Power_Level_Indication
        public static List<short> rf_power_level = new List<short>()
        {
            -97,
            -96,
            -95,
            -94,
            -93,
            -92,
            -91,
            -90,
            -89,
            -88,
            -87,
            -86,
            -85,
            -84,
            -83,
            -82,
            -81,
            -80,
            -79,
            -78,
            -77,
            -76,
            -75,
            -74,
            -73,
            -72,
            -71,
            -70,
            -69,
            -68,
            -67,
            -66,
            -65,
            -64,
            -63,
            -62,
            -61,
            -60,
            -59,
            -58,
            -57,
            -56,
            -55,
            -54,
            -53,
            -52,
            -51,
            -50,
            -49,
            -48,
            -47,
            -46,
            -45,
            -44,
            -43,
            -42,
            -41,
            -40,
            -39,
            -38,
            -37,
            -36,
            -35,
        };



        public static Dictionary<int, string> demod_state_lookup = new Dictionary<int, string>()
        {
            { 0 , "Hunting" },
            { 1 , "Header" },
            { 2 , "Lock DVB-S2" },
            { 3 , "Lock DVB-S" }
        };

        public static Dictionary<uint, string> modcod_lookup_dvbs = new Dictionary<uint, string>()
        {
            { 0, "" },
            { 1, "QPSK 1/2" },
            { 2, "QPSK 2/3" },
            { 3, "QPSK 3/4" },
            { 4, "" },
            { 5, "QPSK 5/6" },
            { 6, "QPSK 6/7" },
            { 7, "QPSK 7/8" },
            { 8, "" },

            /*
            { 4 , "QPSK 1/2" },
            { 5 , "QPSK 3/5" },
            { 6 , "QPSK 2/3" },
            { 7 , "QPSK 3/4" },
            { 9 , "QPSK 5/6" },
            { 10 , "QPSK 6/7" },
            { 11 , "QPSK 7/8" }
            */
        };

        // values obtained from longmynd.py in rydeplayer
        public static Dictionary<uint, double> modcod_lookup_dvbs_threshold = new Dictionary<uint, double>()
        {

            { 0, 0 },
            { 1, 1.7 },
            { 2, 3.3 },
            { 3, 4.2 },
            { 4, 0 },
            { 5, 5.1 },
            { 6, 5.5 },
            { 7, 5.8 },
            { 8, 0 },

            /*
            { 4 , 1.7 },
            { 5 , 4.8 }, // not sure about this one
            { 6 , 3.3 },
            { 7 , 4.2 },
            { 9 , 5.1 },
            { 10 , 5.5  },
            { 11 , 5.8 }
            */
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

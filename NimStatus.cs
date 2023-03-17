using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public class NimStatus
    {
        public byte demod_status { get; set; }
        public bool lna_top_ok { get; set; }
        public bool lna_bottom_ok { get; set; }
        public ushort lna_gain { get; set; }
        public byte power_i { get; set; }
        public byte power_q { get; set; }
        public UInt32 mer { get; set; }
        public bool short_frame { get; set; }
        public bool pilots { get; set; }
        public Int32 frequency_carrier_offset { get; set; }
        public UInt32 symbol_rate { get; set;  }
        public UInt32 modcode { get; set; }
        public byte puncture_rate { get; set; }
        public bool errors_bch_uncorrected { get; set; }
        public UInt32 viterbi_error_rate { get; set; }
        public UInt32 ber { get; set; }
        public UInt32 errors_bch_count { get; set; }
        public UInt32 errors_ldpc_count { get; set; }

        public bool build_queue { get; set; }

        public bool reset { get; set; }

        public byte[,] constellation { get; set; }
    }
}

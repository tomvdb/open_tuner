using System;

namespace opentuner.Hardware
{
    public class TunerStatus
    {
        // nim specific
        public bool lna_top_ok { get; set; }
        public bool lna_bottom_ok { get; set; }
        public UInt32 errors_ldpc_count { get; set; }


        // tuner 1 - demod 2 (TS2)(P2)

        public byte T1P2_demod_status { get; set; }
        public UInt32 T1P2_ts_status { get; set; }
        public UInt32 T1P2_stream_format { get; set; }
        public ushort T1P2_lna_gain { get; set; }
        public byte T1P2_power_i { get; set; }
        public byte T1P2_power_q { get; set; }
        public Int32 T1P2_mer { get; set; }
        public bool T1P2_short_frame { get; set; }
        public bool T1P2_pilots { get; set; }
        public Int32 T1P2_frequency_carrier_offset { get; set; }
        public UInt32 T1P2_symbol_rate { get; set;  }
        public UInt32 T1P2_modcode { get; set; }
        public byte T1P2_puncture_rate { get; set; }
        public bool T1P2_errors_bch_uncorrected { get; set; }
        public UInt32 T1P2_viterbi_error_rate { get; set; }
        public UInt32 T1P2_ber { get; set; }
        public UInt32 T1P2_errors_bch_count { get; set; }
        public ushort T1P2_agc1_gain { get; set; }
        public ushort T1P2_agc2_gain { get; set; }
        public short T1P2_input_power_level { get; set; }
        public bool T1P2_build_queue { get; set; }
        public bool T1P2_reset { get; set; }
        public byte[,] T1P2_constellation { get; set; }
        public byte T1P2_rf_input { get; set; }

        // tuner 2 - demod 1 (TS1)(P1)
        public byte T2P1_demod_status { get; set; }
        public UInt32 T2P1_ts_status { get; set; }
        public UInt32 T2P1_stream_format { get; set; }
        public ushort T2P1_lna_gain { get; set; }
        public byte T2P1_power_i { get; set; }
        public byte T2P1_power_q { get; set; }
        public Int32 T2P1_mer { get; set; }
        public bool T2P1_short_frame { get; set; }
        public bool T2P1_pilots { get; set; }
        public Int32 T2P1_frequency_carrier_offset { get; set; }
        public UInt32 T2P1_symbol_rate { get; set; }
        public UInt32 T2P1_modcode { get; set; }
        public byte T2P1_puncture_rate { get; set; }
        public bool T2P1_errors_bch_uncorrected { get; set; }
        public UInt32 T2P1_viterbi_error_rate { get; set; }
        public UInt32 T2P1_ber { get; set; }
        public UInt32 T2P1_errors_bch_count { get; set; }
        public ushort T2P1_agc1_gain { get; set; }
        public ushort T2P1_agc2_gain { get; set; }
        public short T2P1_input_power_level { get; set; }
        public bool T2P1_build_queue { get; set; }
        public bool T2P1_reset { get; set; }
        public byte[,] T2P1_constellation { get; set; }
        public byte T2P1_rf_input { get; set; }
    }
}

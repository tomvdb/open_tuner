﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace opentuner
{
    public delegate void SourceStatusCallback(TunerStatus status);

    public class NimThread
    {
        ftdi hardware;

        nim _nim;
        stv0910 _stv0910;
        stv6120 _stv6120;
        stvvglna stvvglna_top;
        stvvglna stvvglna_bottom;

        ConcurrentQueue<TunerConfig> config_queue;
        private List<SourceStatusCallback> status_callback = null;

        bool lna_top_ok = false;
        bool lna_bottom_ok = false;
        bool reset = false;
        bool no_lna = false;

        //byte current_demod = stv0910.STV0910_DEMOD_BOTTOM;  

        public event EventHandler<StatusEvent> onNewStatus;

        public NimThread(ConcurrentQueue<TunerConfig> _config_queue, ftdi _hardware, SourceStatusCallback _status_callback, bool _no_lna)
        {
            hardware = _hardware;
            config_queue = _config_queue;
            //status_callback = _status_callback;
            status_callback = new List<SourceStatusCallback>();
            status_callback.Add(_status_callback);

            _nim = new nim(hardware);

            _stv0910 = new stv0910(_nim);
            _stv6120 = new stv6120(_nim);
            no_lna = _no_lna;
            stvvglna_top = new stvvglna(_nim);
            stvvglna_bottom = new stvvglna(_nim);
        }

        public void register_callback(SourceStatusCallback cb)
        {
            status_callback.Add(cb);
        }

        // https://wiki.batc.org.uk/MiniTiouner_Power_Level_Indication
        short get_rf_level(ushort agc1, ushort agc2)
        {
            int index = -1;

            if (agc1 >= 0)
            {
                index = lookups.agc1_lookup.BinarySearch(agc1);

                if (index < 0)
                    index = ~index;
            }
            else
            {
                index = lookups.agc2_lookup.BinarySearch(agc2);

                if (index < 0)
                    index = ~index;

            }

            if (index < 0) index = 0;

            if (index >= lookups.rf_power_level.Count())
                index = lookups.rf_power_level.Count() - 1;

            return lookups.rf_power_level[index];
        }

        byte get_nim_status()
        {
            TunerStatus nim_status = new TunerStatus();

            byte err = 0;

            nim_status.T1P2_reset = reset;

            /*
            if (no_lna)
            {
                nim_status.lna_bottom_ok = false;
                nim_status.lna_top_ok = false;
                nim_status.lna_gain = 0;
            }
            else
            {
                // get lna info
                nim_status.lna_bottom_ok = lna_bottom_ok;
                nim_status.lna_top_ok = lna_top_ok;

                byte lna_gain = 0, lna_vgo = 0;
                if (err == 0) stvvglna_top.stvvglna_read_agc(nim.NIM_INPUT_TOP, ref lna_gain, ref lna_vgo);
                nim_status.lna_gain = (ushort)((lna_gain << 5) | lna_vgo);
            }
            */

            byte rf_input_1 = 0;
            byte rf_input_2 = 0;
            byte temp = 0;
            err = _stv6120.stv6120_read_rf_sel(ref temp);

            rf_input_1 = (byte)(temp & stv6120_regs.STV6120_CTRL9_RFSEL_1_MASK);
            rf_input_2 = (byte)(temp & stv6120_regs.STV6120_CTRL9_RFSEL_2_MASK);

            if (rf_input_1 == 1)
                nim_status.T1P2_rf_input = nim.NIM_INPUT_TOP;
            else
                nim_status.T1P2_rf_input = nim.NIM_INPUT_BOTTOM;

            if (rf_input_2 == 4)
                nim_status.T2P1_rf_input = nim.NIM_INPUT_TOP;
            else
                nim_status.T2P1_rf_input = nim.NIM_INPUT_BOTTOM;

            // get scan state (demod state)
            byte demod_state = 0;
            err = _stv0910.stv0910_read_scan_state(stv0910.STV0910_DEMOD_TOP, ref demod_state);
            nim_status.T1P2_demod_status = demod_state;

            byte demod2_state = 0;
            err = _stv0910.stv0910_read_scan_state(stv0910.STV0910_DEMOD_BOTTOM, ref demod2_state);
            nim_status.T2P1_demod_status = demod2_state;

            /* not using this right now
            // power
            byte power_i = 0;
            byte power_q = 0;
            if (err == 0) err = _stv0910.stv0910_read_power(stv0910.STV0910_DEMOD_TOP, ref power_i, ref power_q);
            nim_status.T1P2_power_i = power_i;
            nim_status.T1P2_power_q = power_q;

            byte[,] constellation_data = new byte[16, 2];

            byte con_i = 0;
            byte con_q = 0;
            if (err == 0)
            {
                for (byte count = 0; count < 16; count++)
                {
                    _stv0910.stv0910_read_constellation(stv0910.STV0910_DEMOD_TOP, ref con_i, ref con_q);
                    constellation_data[count,0] = con_i;
                    constellation_data[count,1] = con_q;
                }
            }

            nim_status.T1P2_constellation = constellation_data;
            */

            /* LDPC Error Count */
            UInt32 errors_ldpc_count = 0;
            if (err == 0) err = _stv0910.stv0910_read_errors_ldpc_count(stv0910.STV0910_DEMOD_TOP, ref errors_ldpc_count);
            nim_status.errors_ldpc_count = errors_ldpc_count;

            /* puncture rate */
            byte puncture_rate = 0;
            if (err == 0) err = _stv0910.stv0910_read_puncture_rate(stv0910.STV0910_DEMOD_TOP, ref puncture_rate);
            nim_status.T1P2_puncture_rate = puncture_rate;

            if (err == 0) err = _stv0910.stv0910_read_puncture_rate(stv0910.STV0910_DEMOD_BOTTOM, ref puncture_rate);
            nim_status.T2P1_puncture_rate = puncture_rate;

            /* carrier frequency offset we are trying */
            Int32 frequency_offset = 0;
            if (err == 0) err = _stv0910.stv0910_read_car_freq(stv0910.STV0910_DEMOD_TOP, ref frequency_offset);
            nim_status.T1P2_frequency_carrier_offset = frequency_offset;
            if (err == 0) err = _stv0910.stv0910_read_car_freq(stv0910.STV0910_DEMOD_BOTTOM, ref frequency_offset);
            nim_status.T2P1_frequency_carrier_offset = frequency_offset;

            /* symbol rate we are trying */
            UInt32 sr = 0;
            if (err == 0) err = _stv0910.stv0910_read_sr(stv0910.STV0910_DEMOD_TOP, ref sr);
            nim_status.T1P2_symbol_rate = sr;
            if (err == 0) err = _stv0910.stv0910_read_sr(stv0910.STV0910_DEMOD_BOTTOM, ref sr);
            nim_status.T2P1_symbol_rate = sr;

            /* viterbi error rate */
            UInt32 viterbi_error_rate = 0;
            if (err == 0) err = _stv0910.stv0910_read_err_rate(stv0910.STV0910_DEMOD_TOP, ref viterbi_error_rate);
            nim_status.T1P2_viterbi_error_rate = viterbi_error_rate;
            if (err == 0) err = _stv0910.stv0910_read_err_rate(stv0910.STV0910_DEMOD_BOTTOM, ref viterbi_error_rate);
            nim_status.T2P1_viterbi_error_rate = viterbi_error_rate;

            /* BER */
            UInt32 ber = 0;
            if (err == 0) err = _stv0910.stv0910_read_ber(stv0910.STV0910_DEMOD_TOP, ref ber);
            nim_status.T1P2_ber = ber;
            if (err == 0) err = _stv0910.stv0910_read_ber(stv0910.STV0910_DEMOD_BOTTOM, ref ber);
            nim_status.T2P1_ber = ber;

            /* BCH Uncorrected Flag */
            bool errors_bch_uncorrected = false;
            if (err == 0) err = _stv0910.stv0910_read_errors_bch_uncorrected(stv0910.STV0910_DEMOD_TOP, ref errors_bch_uncorrected);
            nim_status.T1P2_errors_bch_uncorrected = errors_bch_uncorrected;
            if (err == 0) err = _stv0910.stv0910_read_errors_bch_uncorrected(stv0910.STV0910_DEMOD_BOTTOM, ref errors_bch_uncorrected);
            nim_status.T2P1_errors_bch_uncorrected = errors_bch_uncorrected;

            /* BCH Error Count */
            UInt32 errors_bch_count = 0;
            if (err == 0) err = _stv0910.stv0910_read_errors_bch_count(stv0910.STV0910_DEMOD_TOP, ref errors_bch_count);
            nim_status.T1P2_errors_bch_count = errors_bch_count;
            if (err == 0) err = _stv0910.stv0910_read_errors_bch_count(stv0910.STV0910_DEMOD_BOTTOM, ref errors_bch_count);
            nim_status.T2P1_errors_bch_count = errors_bch_count;


            // agc1 gain
            ushort agc1_gain = 0;
            if (err == 0) err = _stv0910.stv0910_read_agc1_gain(stv0910.STV0910_DEMOD_TOP, ref agc1_gain);
            nim_status.T1P2_agc1_gain = agc1_gain;
            if (err == 0) err = _stv0910.stv0910_read_agc1_gain(stv0910.STV0910_DEMOD_BOTTOM, ref agc1_gain);
            nim_status.T2P1_agc1_gain = agc1_gain;

            // agc2 gain
            ushort agc2_gain = 0;
            if (err == 0) err = _stv0910.stv0910_read_agc2_gain(stv0910.STV0910_DEMOD_TOP, ref agc2_gain);
            nim_status.T1P2_agc2_gain = agc2_gain;
            nim_status.T1P2_input_power_level = get_rf_level(agc1_gain, agc2_gain);

            if (err == 0) err = _stv0910.stv0910_read_agc2_gain(stv0910.STV0910_DEMOD_BOTTOM, ref agc2_gain);
            nim_status.T2P1_agc2_gain = agc2_gain;
            nim_status.T2P1_input_power_level = get_rf_level(agc1_gain, agc2_gain);

            // ma type
            UInt32 ma_type1 = 0;
            UInt32 ma_type2 = 0;

            if (err == 0) _stv0910.stv0910_read_matype(stv0910.STV0910_DEMOD_TOP, ref ma_type1, ref ma_type2);
            nim_status.T1P2_stream_format = (ma_type1 & 0xC0) >> 6; ;

            if (err == 0) _stv0910.stv0910_read_matype(stv0910.STV0910_DEMOD_BOTTOM, ref ma_type1, ref ma_type2);
            nim_status.T2P1_stream_format = (ma_type1 & 0xC0) >> 6; ;

            Int32 mer = 0;

            if (nim_status.T1P2_demod_status == stv0910.DEMOD_S || nim_status.T1P2_demod_status == stv0910.DEMOD_S2)
            {
                if (err == 0) err = _stv0910.stv0910_read_mer(stv0910.STV0910_DEMOD_TOP, ref mer);
            }

            nim_status.T1P2_mer = mer;

            if (nim_status.T2P1_demod_status == stv0910.DEMOD_S || nim_status.T2P1_demod_status == stv0910.DEMOD_S2)
            {
                if (err == 0) err = _stv0910.stv0910_read_mer(stv0910.STV0910_DEMOD_BOTTOM, ref mer);
            }

            nim_status.T2P1_mer = mer;


            /* MODCOD, Short Frames, Pilots */
            UInt32 modcod = 0;
            bool short_frame = false;
            bool pilots = false;
            if (err == 0) err = _stv0910.stv0910_read_modcod_and_type(stv0910.STV0910_DEMOD_TOP, ref modcod, ref short_frame, ref pilots);
            nim_status.T1P2_modcode = modcod;

            if (err == 0) err = _stv0910.stv0910_read_modcod_and_type(stv0910.STV0910_DEMOD_BOTTOM, ref modcod, ref short_frame, ref pilots);
            nim_status.T2P1_modcode = modcod;

            /*
            // tsstatus registers
            UInt32 ts_status = 0;
            if (err == 0) err = _stv0910.stv0910_read_ts_status(stv0910.STV0910_DEMOD_TOP, ref ts_status);

            nim_status.T1P2_ts_status = ts_status;
            */

            if (nim_status.T1P2_demod_status != stv0910.DEMOD_S2)
            {
                /* short frames & pilots only valid for S2 DEMOD state */
                nim_status.T1P2_short_frame = false;
                nim_status.T1P2_pilots = false;
            }

            if (nim_status.T2P1_demod_status != stv0910.DEMOD_S2)
            {
                /* short frames & pilots only valid for S2 DEMOD state */
                nim_status.T2P1_short_frame = false;
                nim_status.T2P1_pilots = false;
            }

            // send status callback if available
            for ( int c = 0;c < status_callback.Count; c++)
            {
                status_callback[c](nim_status);
            }


            reset = false;

            return err;
        }

        public void worker_thread()
        {
            int hw_errors = 0;

            try
            {

                Console.WriteLine("Nim Thread: Starting...");

            bool initialConfig = false;

            TunerConfig nim_config = null;

                byte err = _stv0910.stv0910_init();

                if (err != 0)
                {
                    Console.WriteLine("STV0910 Init Error: " + err.ToString());
                }

                Console.WriteLine("Init Nim");
                err = _nim.nim_init();

                while (true)
                {
                    if (initialConfig == false)
                    {
                        Console.WriteLine("Nim Thread: Initial Config");
                    }

                    if (config_queue.Count() > 0 || initialConfig == false)
                    {
                        while (config_queue.TryDequeue(out nim_config))
                        {
                            Thread.Sleep(10);


                            // setup demod
                            if (err == 0)
                            {
                                Console.WriteLine("Configure Demod Receive - " + nim_config.tuner);
                                if (nim_config.tuner == 1)
                                {
                                    err = _stv0910.stv0910_setup_receive(stv0910.STV0910_DEMOD_TOP, nim_config.symbol_rate);
                                }
                                else
                                {
                                    err = _stv0910.stv0910_setup_receive(stv0910.STV0910_DEMOD_BOTTOM, nim_config.symbol_rate);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error before Demod");
                            }

                            // configure tuner
                            if (err == 0)
                            {
                                Console.WriteLine("Configure Tuner - " + nim_config.tuner.ToString());

                                if (nim_config.tuner == 1)
                                {
                                    err = _stv6120.stv6120_init(1, nim_config.frequency, nim_config.rf_input, nim_config.symbol_rate);
                                }
                                else
                                {
                                    //err = _stv6120.stv6120_init(2, 749246, nim.NIM_INPUT_TOP, 333);
                                    err = _stv6120.stv6120_init(2, nim_config.frequency, nim_config.rf_input, nim_config.symbol_rate);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error before Tuner");
                            }

                            // demod - start scan
                            if (err == 0)
                            {
                                Console.WriteLine("Demod Start Scan - " + nim_config.tuner.ToString() );

                                if (nim_config.tuner == 1)
                                {
                                    err = _stv0910.stv0910_start_scan(stv0910.STV0910_DEMOD_TOP);
                                }
                                else
                                {
                                    err = _stv0910.stv0910_start_scan(stv0910.STV0910_DEMOD_BOTTOM);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error before demod scan");
                            }

                           
                            // 22 kHz - P1
                            if (err == 0)
                            {
                                err = _stv0910.stv0910_switch_22Khz_p1(nim_config.tone_22kHz_P1);
                            }
                            

                            // done, if we have errors, then exit thread
                            if (err != 0)
                            {
                                Console.WriteLine("****** Nim Thread: Hardware Error: " + err.ToString() + " ******");
                                hw_errors += 1;
                                if (hw_errors > 5)
                                {
                                    Console.WriteLine("Too many hardware errors");
                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Nim Thread: Nim Init Good");
                            }

                            initialConfig = true;
                            reset = true;
                        }
                    }
                    else
                    {
                        get_nim_status();
                        Thread.Sleep(200);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Nim Thread: Closing");
            }

        }

    }

    public class StatusEvent : EventArgs
    {
        public TunerStatus nim_status { get; set; }
    }
}

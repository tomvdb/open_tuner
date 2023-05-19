using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using opentuner.Classes;
using opentuner.Transport;

namespace opentuner.Hardware
{
    public class MinitiounerSource : OTSource
    {
        public ftdi ftdi_hw = new ftdi();
        public bool hardware_connected = false;

        public int ts_devices = 1;

        ConcurrentQueue<TunerConfig> config_queue = new ConcurrentQueue<TunerConfig>();

        public CircularBuffer ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);
        public CircularBuffer ts_data_queue2 = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        public TSThread ts_thread;
        public TSThread ts_thread2;

        // threads
        Thread nim_thread_t = null;

        Thread ts_thread_t = null;
        Thread ts_thread_2_t = null;

        public int current_offset_A = 10;
        public int current_offset_B = 10;

        // tuner specific
        public uint current_frequency_1 = 0;
        public uint current_sr_1 = 0;
        public uint current_rf_input_1 = nim.NIM_INPUT_TOP; 

        public uint current_frequency_2 = 0;
        public uint current_sr_2 = 0;
        public uint current_rf_input_2 = nim.NIM_INPUT_TOP; 

        // other
        public bool current_enable_lnb_supply = false;
        public bool current_enable_horiz_supply = false;
        public bool current_tone_22kHz_P1 = false; // true is on, false is off


        private VideoChangeCallback VideoChangeCB;
        private SourceStatusCallback SourceStatusCB;

        public string HardwareDevice { get; set; }

        public MinitiounerSource() { }

        public void Close()
        {
            if (ts_thread_t != null)
                ts_thread_t.Abort();
            if (ts_thread_2_t != null)
                ts_thread_2_t.Abort();
            if (nim_thread_t != null)
                nim_thread_t.Abort();
        }

        public void nim_status_feedback(TunerStatus nim_status)
        {
            if (SourceStatusCB != null)
            {
                SourceStatusCB(nim_status);
            }

        }

        public void change_frequency(byte tuner, UInt32 freq, UInt32 sr, bool lnb_supply, bool polarization_supply_horizontal, uint rf_input, bool tone_22kHz_P1)
        {
            if (!hardware_connected)
                return;

            Console.WriteLine("Change Frequency: " + tuner.ToString());
            switch (rf_input)
            {
                case nim.NIM_INPUT_TOP: Console.WriteLine("RF Input: Nim Input Top Specified"); break;
                case nim.NIM_INPUT_BOTTOM: Console.WriteLine("RF Input: Nim Input Bottom Specified"); break;
                default: Console.WriteLine("Error: Invalid RF Input: " + rf_input.ToString()); break;
            }

            TunerConfig newConfig = new TunerConfig();

            newConfig.tuner = tuner;
            newConfig.frequency = freq;
            newConfig.symbol_rate = sr;
            newConfig.polarization_supply = lnb_supply;
            newConfig.polarization_supply_horizontal = polarization_supply_horizontal;
            newConfig.rf_input = rf_input;
            newConfig.tone_22kHz_P1 = tone_22kHz_P1;

            if (newConfig.frequency < 144000 || newConfig.frequency > 2450000)
            {
                Console.WriteLine("Error: Invalid Frequency: " + newConfig.frequency);
                return;
            }

            Console.WriteLine("Main: New Config: " + newConfig.ToString());

            if (tuner == 1)
            {
                current_frequency_1 = newConfig.frequency;
                current_sr_1 = sr;
                current_rf_input_1 = rf_input;
                //tuner1ControlForm.set_freq(newConfig);

                // we about to change something that will affect the ts stream, lets stop vlc playing
                if (VideoChangeCB != null)
                {
                    VideoChangeCB(1, false);
                    //stop_video1();
                }
            }
            else
            {
                current_frequency_2 = newConfig.frequency;
                current_sr_2 = sr;
                current_rf_input_2 = rf_input;
                //tuner2ControlForm.set_freq(newConfig);

                // we about to change something that will affect the ts stream, lets stop vlc playing
                if (VideoChangeCB != null)
                {
                    VideoChangeCB(2, false);
                    //stop_video2();
                }

            }

            current_enable_lnb_supply = lnb_supply;
            current_enable_horiz_supply = polarization_supply_horizontal;
            current_tone_22kHz_P1 = tone_22kHz_P1;

            config_queue.Enqueue(newConfig);
        }

        public bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB)
        {
            return Initialize(VideoChangeCB, SourceStatusCB, false, "", "", "");
        }


        public bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB, bool manual, string i2c_serial, string ts_serial, string ts2_serial)
        {
            bool result = true;

            this.VideoChangeCB = VideoChangeCB;
            this.SourceStatusCB = SourceStatusCB;

            hardware_init(manual, i2c_serial, ts_serial, ts2_serial);

            if (!hardware_connected)
            {
                MessageBox.Show("Error: No Working Hardware Detected");
                return false;
            }

            Console.WriteLine("Main: Starting Nim Thread");

            // switch off ts leds
            //ftdi_hw.ftdi_ts_led(0, false);
            //ftdi_hw.ftdi_ts_led(1, false);

            // set default lnb supply
            ftdi_hw.ftdi_set_polarization_supply(0, current_enable_lnb_supply, current_enable_horiz_supply);

            // NIM thread
            //SourceStatusCallback status_callback = new SourceStatusCallback(nim_status_feedback);

            NimThread nim_thread = new NimThread(config_queue, ftdi_hw, this.SourceStatusCB, false);

            nim_thread_t = new Thread(nim_thread.worker_thread);

            // set startup frequencies
            StoredFrequency default_freq = new StoredFrequency();
            default_freq.Offset = 0;
            default_freq.Frequency = 741525;
            default_freq.SymbolRate = 1500;
            default_freq.RFInput = 1;

            StoredFrequency tuner_freq1 = default_freq;
            StoredFrequency tuner_freq2 = default_freq;

            /*
             * tofix
            Console.WriteLine("Startup Freq 1: " + setting_tuner1_startfreq);
            Console.WriteLine("Startup Freq 2: " + setting_tuner2_startfreq);

            for (int c = 0; c < stored_frequencies.Count; c++)
            {
                Console.WriteLine(stored_frequencies[c].Name + " - " + stored_frequencies[c].DefaultTuner);

                if (stored_frequencies[c].Name == setting_tuner1_startfreq && stored_frequencies[c].DefaultTuner == 0)
                {
                    Console.WriteLine("Setting startup freq tuner 1 to " + stored_frequencies[c].Name);
                    tuner_freq1 = stored_frequencies[c];
                }
                if (stored_frequencies[c].Name == setting_tuner2_startfreq && stored_frequencies[c].DefaultTuner == 1)
                {
                    Console.WriteLine("Setting startup freq tuner 2 to " + stored_frequencies[c].Name);
                    tuner_freq2 = stored_frequencies[c];
                }
            }
            */

            change_frequency(1, tuner_freq1.Frequency - tuner_freq1.Offset, tuner_freq1.SymbolRate, current_enable_lnb_supply, current_enable_horiz_supply, tuner_freq1.RFInput, current_tone_22kHz_P1);
            change_frequency(2, tuner_freq2.Frequency - tuner_freq2.Offset, tuner_freq2.SymbolRate, current_enable_lnb_supply, current_enable_horiz_supply, tuner_freq2.RFInput, current_tone_22kHz_P1);

            nim_thread_t.Start();

            // TS thread - T1P2
            ts_thread = new TSThread(ftdi_hw, ts_data_queue, nim_thread, ftdi.TS2);
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            if (ts_devices == 2)
            {
                ts_thread2 = new TSThread(ftdi_hw, ts_data_queue2, nim_thread, ftdi.TS1);
                ts_thread_2_t = new Thread(ts_thread2.worker_thread);
                ts_thread_2_t.Start();
            }

            return result;
        }

        private void hardware_init(bool manual, string i2c_serial, string ts_serial, string ts2_serial)
        {

            // detect ftdi devices
            uint i2c_port = 99;
            uint ts_port = 99;
            uint ts_port2 = 99;

            string deviceName = "Unknown";

            byte err = 0;

            if (manual) 
            { 
                err = ftdi_hw.ftdi_detect(ref i2c_port, ref ts_port, ref ts_port2, ref deviceName, i2c_serial, ts_serial, ts2_serial);
            }
            else
            {
                err = ftdi_hw.ftdi_detect(ref i2c_port, ref ts_port, ref ts_port2, ref deviceName);
            }

            //err = ftdi_hw.ftdi_detect(ref i2c_port, ref ts_port, ref ts_port2, ref deviceName);

            if (ts_port2 == 99)
                ts_devices = 1;
            else
                ts_devices = 2;

            //ts_devices = 1;

            if (i2c_port == 99 || ts_port == 99)    // not detected properly, revert to 0 and 1 and hope for the best
            {
                ts_devices = 1;
                Console.WriteLine("Hardware not detected properly, reverting to 0,1");
                err = ftdi_hw.ftdi_init(0, 1, 99);
            }
            else
            {
                Console.WriteLine("Trying detected ports:");
                Console.WriteLine("i2c port: " + i2c_port.ToString());
                Console.WriteLine("ts port: " + ts_port.ToString());
                Console.WriteLine("ts2 port: " + ts_port2.ToString());
                err = ftdi_hw.ftdi_init(i2c_port, ts_port, ts_port2);
            }

            if (err != 0)
            {
                Console.WriteLine("Main: Error: FTDI Failed " + err.ToString());
                hardware_connected = false;
                return;
            }

            hardware_connected = true;

            HardwareDevice = deviceName;
        }
    }
}

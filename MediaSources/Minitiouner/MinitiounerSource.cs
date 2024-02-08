using opentuner.MediaSources.Minitiouner.HardwareInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public class MinitiounerSource : OTSource
    {
        public MTHardwareInterface hardware_interface;
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

        bool T1P2_prevLocked = false;
        bool T2P1_prevLocked = false;


        // tuner specific

        // OTSource Properties
        uint _current_rf_input_1 = nim.NIM_INPUT_TOP;
        public override uint current_rf_input_1 
        {
            get { return _current_rf_input_1; }
            set {  _current_rf_input_1 = value;}
        }

        uint _current_rf_input_2 = nim.NIM_INPUT_TOP;
        public override uint current_rf_input_2
        {
            get { return _current_rf_input_2; }
            set { _current_rf_input_2 = value; }
        }

        uint _current_frequency_1 = 0;
        public override uint current_frequency_1
        {
            get { return _current_frequency_1; }
            set { _current_frequency_1 = value; }
        }

        uint _current_frequency_2 = 0;
        public override uint current_frequency_2
        {
            get { return _current_frequency_2; }
            set { _current_frequency_2 = value; }
        }

        uint _current_sr_1 = 0;
        public override uint current_sr_1
        {
            get { return _current_sr_1; }
            set { _current_sr_1 = value; }
        }

        uint _current_sr_2 = 0;
        public override uint current_sr_2
        {
            get { return _current_sr_2; }
            set { _current_sr_2 = value; }
        }

        int _current_offset_A = 10;
        public override int current_offset_A
        {
            get { return _current_offset_A; }
            set { _current_offset_A = value; }
        }

        int _current_offset_B = 10;
        public override int current_offset_B
        {
            get { return _current_offset_B; }
            set { _current_offset_B = value; }
        }

        public override bool HardwareConnected
        {
            get { return hardware_connected; }
        }


        // other

        bool _current_enable_lnb_supply = false;
        public override bool current_enable_lnb_supply
        {
            get { return _current_enable_lnb_supply; }
            set { _current_enable_lnb_supply = value; }
        }

        bool _current_enable_horiz_supply = false;
        public override bool current_enable_horiz_supply
        {
            get { return _current_enable_horiz_supply; }
            set { _current_enable_horiz_supply = value; }
        }

        bool _current_tone_22kHz_P1 = false;
        public override bool current_tone_22kHz_P1
        {
            get { return _current_tone_22kHz_P1; }
            set { _current_tone_22kHz_P1 = value; }
        }



        private VideoChangeCallback VideoChangeCB;
        private SourceStatusCallback SourceStatusCB;

        public string HardwareDevice { get; set; }

        private int _hardwareInterface = 0;

        // 0 = ftdi
        // 1 = picotuner
        // 2 = picotuner ethernet

        public MinitiounerSource(int HardwareInterface) 
        { 
            _hardwareInterface = HardwareInterface;

            switch(_hardwareInterface)
            {
                case 0: hardware_interface = new FTDIInterface(); break;
                case 1: hardware_interface =  new PicoTunerInterface(); break;
            }
        }


        public override int GetVideoSourceCount()
        {
            return ts_devices;
        }

        public override string GetHardwareDescription()
        {
            return HardwareDevice;
        }

        public override void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue)
        {
            switch (device)
            {
                case 0: ts_thread.RegisterTSConsumer(ts_buffer_queue); break;
                case 1: ts_thread2.RegisterTSConsumer(ts_buffer_queue); break;
            }
        }


        public override void StartStreaming(int device)
        {
            switch(device)
            {
                case 0:
                    if (ts_thread != null)
                        ts_thread.start_ts();
                    break;
                case 1:
                    if (ts_thread2 != null)
                        ts_thread2.start_ts();
                    break;
            }
        }

        public override void StopStreaming(int device)
        {
            switch (device)
            {
                case 0:
                    if (ts_thread != null)
                        ts_thread.stop_ts();
                    break;
                case 1:
                    if (ts_thread2 != null)
                        ts_thread2.stop_ts();
                    break;
            }
        }


        public override long GetCurrentFrequency(int device, bool offset_included)
        {
            long frequency = 0;

            switch (device)
            {
                case 0:  
                    int offset0 = offset_included ? current_offset_A : 0;
                    frequency = current_frequency_1 + offset0; 
                    break;
                case 1:  
                    int offset1 = offset_included ? current_offset_B : 0;
                    frequency = current_frequency_2 + offset1; 
                    break;
            }

            return frequency;
        }

        public override void Close()
        {
            // switch off TS led's
            hardware_interface.hw_ts_led(0, false);
            hardware_interface.hw_ts_led(1, false);

            if (ts_thread_t != null)
                ts_thread_t.Abort();
            if (ts_thread_2_t != null)
                ts_thread_2_t.Abort();
            if (nim_thread_t != null)
                nim_thread_t.Abort();
        }


        public override byte set_polarization_supply(byte lnb_num, bool supply_enable, bool supply_horizontal)
        {
            return hardware_interface.hw_set_polarization_supply(lnb_num, supply_enable, supply_horizontal);
        }

        public override void change_frequency(byte tuner, UInt32 freq, UInt32 sr, bool lnb_supply, bool polarization_supply_horizontal, uint rf_input, bool tone_22kHz_P1)
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

                if (VideoChangeCB != null)
                {
                    VideoChangeCB(1, false);
                }
            }
            else
            {
                current_frequency_2 = newConfig.frequency;
                current_sr_2 = sr;
                current_rf_input_2 = rf_input;

                if (VideoChangeCB != null)
                {
                    VideoChangeCB(2, false);
                }

            }

            current_enable_lnb_supply = lnb_supply;
            current_enable_horiz_supply = polarization_supply_horizontal;
            current_tone_22kHz_P1 = tone_22kHz_P1;

            config_queue.Enqueue(newConfig);
        }

        public override bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB)
        {
            return Initialize(VideoChangeCB, SourceStatusCB, false, "", "", "");
        }


        public void nim_status_feedback(TunerStatus nim_status)
        {
            bool T1P2locked = false;
            bool T2P1Locked = false;

            if (nim_status.T1P2_demod_status >= 2) T1P2locked = true;
            if (nim_status.T2P1_demod_status >= 2) T2P1Locked = true;


            if (T1P2_prevLocked != T1P2locked)
            {
                Console.WriteLine("T1P2 - Lock State Change: " + T1P2_prevLocked.ToString() + "->" + T1P2locked.ToString());

                if (nim_status.T1P2_demod_status >= 2)
                {
                    if (VideoChangeCB != null)
                    {
                        VideoChangeCB(1, true);
                    }
                    hardware_interface.hw_ts_led(0, true);
                }
                else
                {
                    if (VideoChangeCB != null)
                    {
                        VideoChangeCB(1, false);
                    }

                    hardware_interface.hw_ts_led(0, false);
                }

                T1P2_prevLocked = T1P2locked;
            }

            if (GetVideoSourceCount() == 2)
            {
                if (T2P1_prevLocked != T2P1Locked)
                {
                    Console.WriteLine("T2P1 - Lock State Change: " + T2P1_prevLocked.ToString() + "->" + T2P1Locked.ToString());

                    if (nim_status.T2P1_demod_status >= 2)
                    {
                        if (VideoChangeCB != null)
                        {
                            VideoChangeCB(2, true);
                        }

                        hardware_interface.hw_ts_led(1, true);
                    }
                    else
                    {
                        if (VideoChangeCB != null)
                        {
                            VideoChangeCB(2, false);
                        }

                        hardware_interface.hw_ts_led(1, false);
                    }

                    T2P1_prevLocked = T2P1Locked;
                }
            }

            if (SourceStatusCB != null)
            {
                SourceStatusCB(nim_status);
            }

        }

        public override bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB, bool manual, string i2c_serial, string ts_serial, string ts2_serial)
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

            Console.WriteLine("Switch LED's");
            
            
            // switch off ts leds
            hardware_interface.hw_ts_led(0, false);
            hardware_interface.hw_ts_led(1, false);

            // set default lnb supply
            Console.WriteLine(current_enable_lnb_supply.ToString());
            Console.WriteLine(current_enable_horiz_supply.ToString());

            hardware_interface.hw_set_polarization_supply(0, current_enable_lnb_supply, current_enable_horiz_supply);

            // NIM thread
            //SourceStatusCallback status_callback = new SourceStatusCallback(nim_status_feedback);

            NimThread nim_thread = new NimThread(config_queue, hardware_interface, nim_status_feedback, false);

            nim_thread_t = new Thread(nim_thread.worker_thread);

            // set startup frequencies
            StoredFrequency default_freq = new StoredFrequency();
            default_freq.Offset = 0;
            default_freq.Frequency = 741525;
            default_freq.SymbolRate = 1500;
            default_freq.RFInput = 1;

            StoredFrequency tuner_freq1 = default_freq;
            StoredFrequency tuner_freq2 = default_freq;

            change_frequency(1, tuner_freq1.Frequency - tuner_freq1.Offset, tuner_freq1.SymbolRate, current_enable_lnb_supply, current_enable_horiz_supply, tuner_freq1.RFInput, current_tone_22kHz_P1);
            change_frequency(2, tuner_freq2.Frequency - tuner_freq2.Offset, tuner_freq2.SymbolRate, current_enable_lnb_supply, current_enable_horiz_supply, tuner_freq2.RFInput, current_tone_22kHz_P1);

            nim_thread_t.Start();

            // TS thread - T1P2
            ts_thread = new TSThread(ts_data_queue, nim_thread, FlushTS2, ReadTS2, "MT TS2");
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            if (ts_devices == 2)
            {
                ts_thread2 = new TSThread(ts_data_queue2, nim_thread, FlushTS1, ReadTS1, "MT TS1");
                ts_thread_2_t = new Thread(ts_thread2.worker_thread);
                ts_thread_2_t.Start();
            }

            return result;
        }

        public override CircularBuffer GetVideoDataQueue(int device)
        {
            switch (device)
            {
                case 0: return ts_data_queue;
                case 1: return ts_data_queue2;
            }

            return ts_data_queue;
        }

        void FlushTS2()
        {
            hardware_interface.transport_flush(PicoTunerInterface.TS2);
        }

        byte ReadTS2(ref byte[] data, ref uint dataRead)
        {
            return hardware_interface.transport_read(PicoTunerInterface.TS2, ref data, ref dataRead);
        }

        byte ReadTS1(ref byte[] data, ref uint dataRead)
        {
            return hardware_interface.transport_read(PicoTunerInterface.TS1, ref data, ref dataRead);
        }


        void FlushTS1()
        {
            hardware_interface.transport_flush(PicoTunerInterface.TS1);
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
                err = hardware_interface.hw_detect(ref i2c_port, ref ts_port, ref ts_port2, ref deviceName, i2c_serial, ts_serial, ts2_serial);
            }
            else
            {
                err = hardware_interface.hw_detect(ref i2c_port, ref ts_port, ref ts_port2, ref deviceName);
            }


            if (ts_port2 == 99)
                ts_devices = 1;
            else
                ts_devices = 2;

            //ts_devices = 1;

            if (i2c_port == 99 || ts_port == 99)    // not detected properly, revert to 0 and 1 and hope for the best
            {
                ts_devices = 1;
                Console.WriteLine("Hardware not detected properly, reverting to 0,1");
                err = hardware_interface.hw_init(0, 1, 99);
            }
            else
            {
                Console.WriteLine("Trying detected ports:");
                Console.WriteLine("i2c port: " + i2c_port.ToString());
                Console.WriteLine("ts port: " + ts_port.ToString());
                Console.WriteLine("ts2 port: " + ts_port2.ToString());
                err = hardware_interface.hw_init(i2c_port, ts_port, ts_port2);
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

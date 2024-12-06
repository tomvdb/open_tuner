using opentuner.MediaSources.Minitiouner.HardwareInterfaces;
using opentuner.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using opentuner.MediaPlayers;
using Serilog;

namespace opentuner.MediaSources.Minitiouner
{
    public partial class MinitiounerSource : OTSource
    {
        public MTHardwareInterface hardware_interface;
        public bool hardware_connected = false;

        public int ts_devices = 1;

        ConcurrentQueue<TunerConfig> config_queue = new ConcurrentQueue<TunerConfig>();

        public CircularBuffer ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);
        public CircularBuffer ts_data_queue2 = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        public TSThread ts_thread;
        public TSThread ts_thread2;

        Thread ts_parser_t = null;
        Thread ts_parser_2_t = null;

        // threads
        Thread nim_thread_t = null;

        Thread ts_thread_t = null;
        Thread ts_thread_2_t = null;

        bool T1P2_prevLocked = false;
        bool T2P1_prevLocked = false;


        List<OTMediaPlayer> _media_player = new List<OTMediaPlayer> ();
        private List<TSRecorder> _ts_recorders;
        private List<TSUdpStreamer> _ts_streamers;
        private List<TunerControlForm> _tuner_forms;

        string _mediapath = "";

        public override bool DeviceConnected
        {
            get { return hardware_connected; }
        }


        // tuner specific

        // OTSource Properties
        private uint current_rf_input_0 = nim.NIM_INPUT_TOP;
        private uint current_rf_input_1 = nim.NIM_INPUT_TOP;
        private uint current_frequency_0 = 0;
        private uint current_frequency_1 = 0;
        private uint current_sr_0 = 0;
        private uint current_sr_1 = 0;

        private bool current_tone_22kHz_P1 = false;
        private uint current_offset_0 = 0;
        private uint current_offset_1 = 0;

        private byte current_lnba_psu = 0;
        private byte current_lnbb_psu = 0;

        private string last_service_name_0 = "";
        private string last_service_provider_0 = "";
        private string last_dbm_0 = "";
        private string last_mer_0 = "";

        private string last_service_name_1 = "";
        private string last_service_provider_1 = "";
        private string last_dbm_1 = "";
        private string last_mer_1 = "";


        private VideoChangeCallback VideoChangeCB;

        // todo: this is the callback we want to remove and keep internal
        private SourceStatusCallback SourceStatusCB;

        public string HardwareDevice { get; set; }

        private int _hardwareInterface = 0;

        // 0 = ftdi
        // 1 = picotuner
        // 2 = picotuner ethernet

        private SettingsManager<MinitiounerSettings> _settingsManager;
        private MinitiounerSettings _settings;

        public MinitiounerSource() 
        {
            // settings
            _settings = new MinitiounerSettings();
            _settingsManager = new SettingsManager<MinitiounerSettings>("minitiouner_settings");
            _settings = (_settingsManager.LoadSettings(_settings));
        }

        public override int GetVideoSourceCount()
        {
            return ts_devices;
        }

        public override string GetDeviceName()
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


        public override long GetFrequency(int device, bool offset_included)
        {
            long frequency = 0;

            switch (device)
            {
                case 0:  
                    int offset0 = offset_included ? (int)current_offset_0 : 0;
                    frequency = current_frequency_0 + offset0; 
                    break;
                case 1:  
                    int offset1 = offset_included ? (int)current_offset_1 : 0;
                    frequency = current_frequency_1 + offset1; 
                    break;
            }

            return frequency;
        }

        // device : 0 = Tuner 1, 1 = Tuner 2
        public override void SetFrequency(int device, uint frequency, uint symbol_rate, bool offset_included)
        {
            uint freq = 0;
            
            if (device == 0 )
                freq = frequency - (offset_included ? current_offset_0 : 0);
            else
                freq = frequency - (offset_included ? current_offset_1 : 0);

            change_frequency((byte)(device), freq, symbol_rate,  (device == 0 ? current_rf_input_0 : current_rf_input_1 ), current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
        }


        public void change_frequency(byte device, UInt32 freq, UInt32 sr,  uint rf_input, bool tone_22kHz_P1, byte lnbA_supply, byte lnbB_supply)
        {
            if (!hardware_connected)
                return;

            if (device >= GetVideoSourceCount())
            {
                return;
            }

            Log.Information("Change Frequency: " + device.ToString());

            switch (rf_input)
            {
                case nim.NIM_INPUT_TOP: Log.Information("RF Input: Nim Input Top Specified"); break;
                case nim.NIM_INPUT_BOTTOM: Log.Information("RF Input: Nim Input Bottom Specified"); break;
                default: Log.Information("Error: Invalid RF Input: " + rf_input.ToString()); break;
            }

            TunerConfig newConfig = new TunerConfig();

            newConfig.tuner = (byte)(device+1);
            newConfig.frequency = freq;
            newConfig.symbol_rate = sr;
            newConfig.rf_input = rf_input;
            newConfig.tone_22kHz_P1 = tone_22kHz_P1;
            newConfig.lnba_psu = lnbA_supply;
            newConfig.lnbb_psu = lnbB_supply;

            if (newConfig.frequency < 144000 || newConfig.frequency > 2450000)
            {
                Log.Information("Error: Invalid Frequency: " + newConfig.frequency);
                return;
            }

            Log.Information("Main: New Config: " + newConfig.ToString());

            if (device == 0)
            {
                current_frequency_0 = newConfig.frequency;
                current_sr_0 = sr;
                current_rf_input_0 = rf_input;

                VideoChangeCB?.Invoke(0, false);
            }
            else
            {
                current_frequency_1 = newConfig.frequency;
                current_sr_1 = sr;
                current_rf_input_1 = rf_input;

                VideoChangeCB?.Invoke(1, false);
            }

            current_tone_22kHz_P1 = tone_22kHz_P1;

            config_queue.Enqueue(newConfig);

            

            if (_tuner_forms[device] != null)
            {
                _tuner_forms[device].UpdateTuner( (device == 0) ? current_frequency_0 : current_frequency_1, (device == 0) ? current_sr_0 : current_sr_0, (device == 0 ) ? current_offset_0 : current_offset_1);
            }

        }

        //public byte set_polarization_supply(byte lnb_num, bool supply_enable, bool supply_horizontal)
        //{
        //    return hardware_interface.hw_set_polarization_supply(lnb_num, supply_enable, supply_horizontal);
        //}

        public override int Initialize(VideoChangeCallback VideoChangeCB, Control Parent, bool mute_at_startup)
        {            
            switch (_settings.DefaultInterface)
            {
                case 0:
                    var hw_ask = new ChooseMinitiounerHardwareInterfaceForm();

                    if (hw_ask.ShowDialog() == DialogResult.OK)
                    {
                        switch(hw_ask.comboHardwareSelect.SelectedIndex)
                        {
                            case 0: hardware_interface = new FTDIInterface(); break;
                            case 1: hardware_interface = new PicoTunerInterface(); break;
                        }
                    }
                    else return -1;

                    break;
                case 1: hardware_interface = new FTDIInterface(); break;
                case 2: hardware_interface = new PicoTunerInterface(); break;
            }

            int retCode = Initialize(VideoChangeCB, SourceStatusCB, false, "", "", "", Parent, mute_at_startup);
            if(retCode == -1)
            {
                hardware_interface = null;
            }
            return retCode;
        }

        public void nim_status_feedback(TunerStatus nim_status)
        {
            bool T1P2locked = false;
            bool T2P1Locked = false;

            if (nim_status.T1P2_demod_status >= 2) T1P2locked = true;
            if (nim_status.T2P1_demod_status >= 2) T2P1Locked = true;


            if (T1P2_prevLocked != T1P2locked)
            {
                Log.Information("T1P2 - Lock State Change: " + T1P2_prevLocked.ToString() + "->" + T1P2locked.ToString());

                if (nim_status.T1P2_demod_status >= 2)
                {
                    VideoChangeCB?.Invoke(0, true);
                    hardware_interface.hw_ts_led(0, true);
                }
                else
                {
                    VideoChangeCB?.Invoke(0, false);
                    hardware_interface.hw_ts_led(0, false);
                }

                T1P2_prevLocked = T1P2locked;
            }

            if (GetVideoSourceCount() == 2)
            {
                if (T2P1_prevLocked != T2P1Locked)
                {
                    Log.Information("T2P1 - Lock State Change: " + T2P1_prevLocked.ToString() + "->" + T2P1Locked.ToString());

                    if (nim_status.T2P1_demod_status >= 2)
                    {
                        VideoChangeCB?.Invoke(1, true);
                        hardware_interface.hw_ts_led(1, true);
                    }
                    else
                    {
                        VideoChangeCB?.Invoke(1, false);
                        hardware_interface.hw_ts_led(1, false);
                    }

                    T2P1_prevLocked = T2P1Locked;
                }
            }

            nim_status.T2P1_requested_frequency = current_frequency_0;
            nim_status.T1P2_requested_frequency = current_frequency_1;

            // update properties
            UpdateTunerProperties(nim_status);
        }

        private void ChangeRFInput(byte Tuner, uint RFInput)
        {
            switch(Tuner)
            {
                case 0: 
                    change_frequency(Tuner, current_frequency_0, current_sr_0, RFInput, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case 1:
                    change_frequency(Tuner, current_frequency_1, current_sr_1, RFInput, false, current_lnba_psu, current_lnbb_psu);
                    break;
            }

        }

        private void ChangeSymbolRate(byte Tuner, uint SymbolRate)
        {
            switch (Tuner)
            {
                case 0:
                    change_frequency(Tuner, current_frequency_0, SymbolRate, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case 1:
                    change_frequency(Tuner, current_frequency_1, SymbolRate, current_rf_input_1, false, current_lnba_psu, current_lnbb_psu);
                    break;
            }
        }

        private void ChangeOffset(byte Tuner, int offset)
        {
            switch(Tuner)
            {
                case 0:
                    current_offset_0 = (uint)offset;
                    break;
                case 1:
                    current_offset_1 = (uint)offset;
                    break;
            }

            if (_tuner_forms[Tuner] != null)
            {
                _tuner_forms[Tuner].UpdateTuner((Tuner == 0) ? current_frequency_0 : current_frequency_1, (Tuner == 0) ? current_sr_0 : current_sr_0, (Tuner == 0) ? current_offset_0 : current_offset_1);
            }

        }

        private int Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB, bool manual, string i2c_serial, string ts_serial, string ts2_serial, Control Parent, bool mute_at_startup)
        {

            this.VideoChangeCB = VideoChangeCB;
            this.SourceStatusCB = SourceStatusCB;

            hardware_init(manual, i2c_serial, ts_serial, ts2_serial);

            if (!hardware_connected)
            {
                MessageBox.Show("Error: No Working Hardware Detected");
                return -1;
            }

            // build properties
            _parent = Parent;
            BuildSourceProperties(mute_at_startup);

            _source_properties.UpdateValue("source_hw_interface", hardware_interface.GetName);

            Log.Information("Main: Starting Nim Thread");

            Log.Information("Switch LED's");
            
            
            // switch off ts leds
            hardware_interface.hw_ts_led(0, false);
            hardware_interface.hw_ts_led(1, false);

            // configure nim thread
            NimThread nim_thread = new NimThread(config_queue, hardware_interface, nim_status_feedback, false);
            nim_thread_t = new Thread(nim_thread.worker_thread);



            /*
             
            TODO: default lnb voltage supply settings

            // set default lnb supply
            Log.Information(current_enable_lnb_supply.ToString());
            Log.Information(current_enable_horiz_supply.ToString());

            */

            /*
            current_lnba_psu = _settings.DefaultLnbASupply;
            switch (current_lnba_psu)
            {
                case 0: hardware_interface.hw_set_polarization_supply(0, false, false);
                    break;
                case 1:
                    hardware_interface.hw_set_polarization_supply(0, true, false);
                    break;
                case 2:
                    hardware_interface.hw_set_polarization_supply(0, true, true);
                    break;
            }

            current_lnbb_psu = _settings.DefaultLnbBSupply;
            switch (current_lnbb_psu)
            {
                case 0:
                    hardware_interface.hw_set_polarization_supply(1, false, false);
                    break;
                case 1:
                    hardware_interface.hw_set_polarization_supply(1, true, false);
                    break;
                case 2:
                    hardware_interface.hw_set_polarization_supply(1, true, true);
                    break;
            }
            */

            current_lnba_psu = _settings.DefaultLnbASupply;
            current_lnbb_psu = _settings.DefaultLnbBSupply;

            current_tone_22kHz_P1 = false;

            
            hardware_interface.hw_set_polarization_supply(1, false, false);

            // set startup rf inputs according to settings
            current_rf_input_0 = nim.NIM_INPUT_TOP;
            current_rf_input_1 = nim.NIM_INPUT_TOP;

            switch (_settings.DefaultRFInput )
            {
                case 1:
                    current_rf_input_0 = nim.NIM_INPUT_TOP;
                    current_rf_input_1 = nim.NIM_INPUT_BOTTOM;
                    break;
                case 2:
                    current_rf_input_0 = nim.NIM_INPUT_BOTTOM;
                    current_rf_input_1 = nim.NIM_INPUT_TOP;
                    break;
                case 3:
                    current_rf_input_0 = nim.NIM_INPUT_BOTTOM;
                    current_rf_input_1 = nim.NIM_INPUT_BOTTOM;
                    break;
            }

            current_offset_0 = _settings.Offset1;
            current_offset_1 = _settings.Offset2;

            current_frequency_0 = 10491500 - current_offset_0;
            current_frequency_1 = 10491500 - current_offset_1;

            current_sr_0 = 1500;
            current_sr_1 = 1500;

            // setup tuner 0
            change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);

            if (ts_devices == 2)
            {
                // setup tuner 1
                change_frequency(1, current_frequency_1, current_sr_1, current_rf_input_1, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
            }

            nim_thread_t.Start();

            // TS thread - T1P2
            ts_thread = new TSThread(ts_data_queue, FlushTS2, ReadTS2, "MT TS2");
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            if (ts_devices == 2)
            {
                ts_thread2 = new TSThread(ts_data_queue2, FlushTS1, ReadTS1, "MT TS1");
                ts_thread_2_t = new Thread(ts_thread2.worker_thread);
                ts_thread_2_t.Start();
            }

            // start TS Parser 
            TSParserThread ts_parser_thread = new TSParserThread(parse_ts_data_callback);
            RegisterTSConsumer(0, ts_parser_thread.parser_ts_data_queue);
            ts_parser_t = new Thread(ts_parser_thread.worker_thread);
            ts_parser_t.Start();

            if (ts_devices == 2)
            {
                TSParserThread ts_parser_thread2 = new TSParserThread(parse_ts2_data_callback);
                RegisterTSConsumer(1, ts_parser_thread2.parser_ts_data_queue);
                ts_parser_2_t = new Thread(ts_parser_thread2.worker_thread);
                ts_parser_2_t.Start();
            }

            return ts_devices;
        }

        public void parse_ts_data_callback(TSStatus ts_status)
        {
            UpdateTSProperties(1, ts_status);
        }

        public void parse_ts2_data_callback(TSStatus ts_status)
        {
            UpdateTSProperties(2, ts_status);
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
            byte err = hardware_interface.transport_read(PicoTunerInterface.TS2, ref data, ref dataRead);
            DataReceived(PicoTunerInterface.TS2);
            return err;
        }

        byte ReadTS1(ref byte[] data, ref uint dataRead)
        {
            byte err = hardware_interface.transport_read(PicoTunerInterface.TS1, ref data, ref dataRead);
            DataReceived(PicoTunerInterface.TS1);
            return err;
        }


        void FlushTS1()
        {
            hardware_interface.transport_flush(PicoTunerInterface.TS1);
        }


        private void DataReceived(byte TS)
        {
            if (TS == 1)
            {
                ts_thread.NewDataPresent();
            }
            else
            {
                ts_thread2.NewDataPresent();
            }
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
                Log.Information("Hardware not detected properly, reverting to 0,1");
                err = hardware_interface.hw_init(0, 1, 99);
            }
            else
            {
                Log.Information("Trying detected ports:");
                Log.Information("i2c port: " + i2c_port.ToString());
                Log.Information("ts port: " + ts_port.ToString());
                Log.Information("ts2 port: " + ts_port2.ToString());
                err = hardware_interface.hw_init(i2c_port, ts_port, ts_port2);
            }

            if (err != 0)
            {
                Log.Error("Main: Error: FTDI Failed " + err.ToString());
                hardware_connected = false;
                return;
            }

            hardware_connected = true;

            HardwareDevice = deviceName;
        }



        public override void ConfigureVideoPlayers(List<OTMediaPlayer> MediaPlayers)
        {
            if (MediaPlayers.Count != ts_devices)
            {
                Log.Information("Error: MinitiounerSource Expected " +  ts_devices.ToString() + " video players, but only received " + MediaPlayers.Count);
            }

            for (int c = 0; c < MediaPlayers.Count; c++)
            {
                _media_player.Add(MediaPlayers[c]);
                _media_player[c].onVideoOut += MinitiounerSource_onVideoOut;
                if (_settings.DefaultMuted[c])
                {
                    _media_player[c].SetVolume(0);
                }
                else
                {
                    _media_player[c].SetVolume((int)_settings.DefaultVolume[c]);
                }
            }

        }

        private void MinitiounerSource_onVideoOut(object sender, MediaStatus e)
        {
            for (int c = 0; c < _media_player.Count; c++)
            {
                if ((OTMediaPlayer)sender == _media_player[c])
                {
                    preMute[c] = (int)_settings.DefaultVolume[c];
                    muted[c] = _settings.DefaultMuted[c];
                    if (muted[c] == true)
                    {
                        _media_player[c].SetVolume(0);
                    }
                    else
                    {
                        _media_player[c].SetVolume(preMute[c]);
                    }

                    UpdateMediaProperties(c, e);
                    break;
                }
            }
        }

        public override string GetName()
        {
            return "Minitiouner Variant";
        }

        public override void OverrideDefaultMuted(bool Override)
        {
            if (Override)
            {
                preMute[0] = (int)_settings.DefaultVolume[0];               // save DefaultVolume in preMute
                _tuner1_properties.UpdateValue("volume_slider_1", "0");     // side effect: will set DefaultVolume to 0
                _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.PaleVioletRed);
                muted[0] = _settings.DefaultMuted[0] = true;
                _settings.DefaultVolume[0] = (uint)preMute[0];              // restore DefaultVolume

                if (ts_devices == 2)
                {
                    preMute[1] = (int)_settings.DefaultVolume[1];           // save DefaultVolume in preMute
                    _tuner2_properties.UpdateValue("volume_slider_2", "0"); // side effect: will set DefaultVolume to 0
                    _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.PaleVioletRed);
                    muted[1] = _settings.DefaultMuted[1] = true;
                    _settings.DefaultVolume[1] = (uint)preMute[1];          // restore DefaultVolume
                }
            }
        }

        public override string GetDescription()
        {
            return "Minitiouner Variant" +
            Environment.NewLine + Environment.NewLine +
            "Should work with most Minitiouner Variants" +
            Environment.NewLine + Environment.NewLine +
            "Select FTDI or PicoTuner interface in Settings";
        }

        public override void Start()
        {
        }

        public override void Close()
        {
            _settingsManager.SaveSettings(_settings);

            // switch off TS led's
            hardware_interface?.hw_ts_led(0, false);
            hardware_interface?.hw_ts_led(1, false);

            ts_parser_t?.Abort();
            ts_parser_2_t?.Abort();
            ts_thread_t?.Abort();
            ts_thread_2_t?.Abort();
            nim_thread_t?.Abort();
            hardware_interface?.hw_close();
        }

        public override void ShowSettings()
        {
            MinitiounerSettingsForm settings_form = new MinitiounerSettingsForm(ref _settings);
            if (settings_form.ShowDialog() == DialogResult.OK) 
            {
                _settingsManager.SaveSettings(_settings); 
            }
            
        }

        public override void ConfigureTSRecorders(List<TSRecorder> TSRecorders)
        {
            _ts_recorders = TSRecorders;
            
            for (int c = 0; c < _ts_recorders.Count; c++)
            {
                _ts_recorders[c].onRecordStatusChange += MinitiounerSource_onRecordStatusChange;
            }
        }

        private void MinitiounerSource_onRecordStatusChange(object sender, bool e)
        {
            Log.Information(((TSRecorder)(sender)).ID.ToString() + " recording status : " + e.ToString());
        }

        public override void ConfigureTSStreamers(List<TSUdpStreamer> TSStreamers)
        {
            _ts_streamers = TSStreamers;
            
            for (int c = 0; c < _ts_streamers.Count; c++)
            {
                _ts_streamers[c].onStreamStatusChange += MinitiounerSource_onStreamStatusChange;
            }
        }

        private void MinitiounerSource_onStreamStatusChange(object sender, bool e)
        {
            Log.Information(((TSUdpStreamer)(sender)).ID.ToString() + " streaming status : " + e.ToString());
        }

        public override void ConfigureMediaPath(string MediaPath)
        {
            _mediapath = MediaPath;
        }

        public override string GetMoreInfoLink()
        {
            return "https://www.zr6tg.co.za/opentuner-minitiouner-source/";
        }
    }
}

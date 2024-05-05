using LibVLCSharp.Shared;
using NAudio.Gui;
using Newtonsoft.Json.Linq;
using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static opentuner.Transmit.F5OEOPlutoControl;
using System.Drawing;
using System.Runtime.CompilerServices;
using FlyleafLib.MediaFramework.MediaFrame;
using Vortice.MediaFoundation;
using Serilog;

namespace opentuner.MediaSources.Minitiouner
{
    public enum MinitiounerPropertyCommands
    {
        SETFREQUENCY,
        SETRFINPUTA,
        SETRFINPUTB,
        SETSYMBOLRATE,
        SETOFFSET,
        LNBA_OFF,
        LNBA_VERTICAL,
        LNBA_HORIZONTAL,
        LNBB_OFF,
        LNBB_VERTICAL,
        LNBB_HORIZONTAL
    }

    public partial class MinitiounerSource
    {
        // properties management
        Control _parent = null;

        private static DynamicPropertyGroup _tuner1_properties = null;
        private static DynamicPropertyGroup _tuner2_properties = null;
        private static DynamicPropertyGroup _source_properties = null;

        // context menu strip
        ContextMenuStrip _genericContextStrip;

        public override event SourceDataChange OnSourceData;

        private bool BuildSourceProperties()
        {
            if (_parent == null)
            {
                Log.Information("Fatal Error: No Properties Panel");
                return false;
            }

            _genericContextStrip = new ContextMenuStrip();
            _genericContextStrip.Opening += _genericContextStrip_Opening;


            if (ts_devices == 2)
            {
                _tuner2_properties = ConfigureTunerProperties(2);
                muted[1] = _settings.DefaultMuted[1];
                preMute[1] = (int)_settings.DefaultVolume[1];

                if (!_settings.DefaultMuted[1])
                {
                    _tuner2_properties.UpdateValue("volume_slider_2", _settings.DefaultVolume[1].ToString());
                    _settings.DefaultVolume[1] = (uint)preMute[1];  // restore as changed by update volume_slider function
                    _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.Transparent);
                }
                else
                {
                    _tuner2_properties.UpdateValue("volume_slider_2", "0");
                    _settings.DefaultVolume[1] = (uint)preMute[1];  // restore as changed by update volume_slider function
                    _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.PaleVioletRed);
                }
            }

            _tuner1_properties = ConfigureTunerProperties(1);
            muted[0] = _settings.DefaultMuted[0];
            preMute[0] = (int)_settings.DefaultVolume[0];

            if (!_settings.DefaultMuted[0])
            {
                _tuner1_properties.UpdateValue("volume_slider_1", _settings.DefaultVolume[0].ToString());
                _settings.DefaultVolume[0] = (uint)preMute[0];  // restore as changed by update volume_slider function
                _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.PaleVioletRed);
            }
            else
            {
                _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.PaleVioletRed);
                _settings.DefaultVolume[0] = (uint)preMute[0];  // restore as changed by update volume_slider function
                _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.PaleVioletRed);
            }

            // source properties
            _source_properties = new DynamicPropertyGroup("Minitiouner Properties", _parent);
            _source_properties.setID(99);
            _source_properties.AddItem("source_hw_interface", "Hardware Interface");
            _source_properties.AddItem("source_hw_ldpc_errors", "LPDC Errors");

            _source_properties.AddItem("hw_lnba", "LNB-A Power Supply", _genericContextStrip);
            _source_properties.AddItem("hw_lnbb", "LNB-B Power Supply", _genericContextStrip);

            _tuner_forms = new List<TunerControlForm>();
            // tuner for each device
            for (int c = 0; c < ts_devices; c++)
            {
                var tunerControl = new TunerControlForm(c, 0, 0, (int)(c == 0 ? current_offset_0 : current_offset_1), this);
                tunerControl.OnTunerChange += TunerControl_OnTunerChange;
                _tuner_forms.Add(tunerControl);
            }
            return true;
        }


        private DynamicPropertyGroup ConfigureTunerProperties(int tuner)
        {
            DynamicPropertyGroup dynamicPropertyGroup = new DynamicPropertyGroup("Tuner " +  tuner.ToString(), _parent);
            dynamicPropertyGroup.setID(tuner);
            dynamicPropertyGroup.OnSlidersChanged += DynamicPropertyGroup_OnSliderChanged;
            dynamicPropertyGroup.OnMediaButtonPressed += DynamicPropertyGroup_OnMediaButtonPressed;
            dynamicPropertyGroup.AddItem("demodstate", "Demod State", Color.PaleVioletRed);
            dynamicPropertyGroup.AddItem("mer", "Mer");
            //dynamicPropertyGroup.AddItem("db_margin", "db Margin");
            dynamicPropertyGroup.AddItem("rf_input_level", "RF Input Level");
            dynamicPropertyGroup.AddItem("rf_input", "RF Input", _genericContextStrip);
            dynamicPropertyGroup.AddItem("requested_freq_" + tuner.ToString(), "Requested Freq", _genericContextStrip);
            dynamicPropertyGroup.AddItem("symbol_rate", "Symbol Rate", _genericContextStrip);
            dynamicPropertyGroup.AddItem("offset", "Freq Offset", _genericContextStrip);
            dynamicPropertyGroup.AddItem("modcod", "Modcod");
            dynamicPropertyGroup.AddItem("lna_gain", "LNA Gain");
            dynamicPropertyGroup.AddItem("ber", "Ber");
            dynamicPropertyGroup.AddItem("freq_carrier_offset", "Freq Carrier Offset");
            dynamicPropertyGroup.AddItem("stream_format", "Stream Format");
            dynamicPropertyGroup.AddItem("service_name", "Service Name");
            dynamicPropertyGroup.AddItem("service_name_provider", "Service Name Provider");
            dynamicPropertyGroup.AddItem("null_packets", "Null Packets");
            dynamicPropertyGroup.AddItem("video_codec", "Video Codec");
            dynamicPropertyGroup.AddItem("video_resolution", "Video Resolution");
            dynamicPropertyGroup.AddItem("audio_codec", "Audio Codec");
            dynamicPropertyGroup.AddItem("audio_rate", "Audio Rate");
            dynamicPropertyGroup.AddSlider("volume_slider_" + tuner.ToString(), "Volume", 0, 200);
            dynamicPropertyGroup.AddMediaControls("media_controls_" + tuner.ToString(), "Media Controls");
            return dynamicPropertyGroup;
        }

        private int[] preMute = new int[] { 50, 50 };
        private bool[] muted = new bool[] { true, true };
        private int indicatorStatus1 = 0;
        private int indicatorStatus2 = 0;

        

        public void SetIndicator(ref int indicatorInput, PropertyIndicators indicator)
        {
            indicatorInput |= (byte)(1 << (int)indicator);
        }

        public void ClearIndicator(ref int indicatorInput, PropertyIndicators indicator)
        {
            indicatorInput &= (byte)~(1 << (int)indicator);
        }

        private void DynamicPropertyGroup_OnMediaButtonPressed(string key, int function)
        {
            int tuner = 0;

            if (key == "media_controls_2")
                tuner = 1;

            switch (function)
            {
                case 0: // mute
                    if (tuner == 0)
                    {
                        if (!muted[0])
                        {
                            preMute[0] = _media_players[0].GetVolume();
                            _media_players[0].SetVolume(0);
                            _tuner1_properties.UpdateValue("volume_slider_1", "0");
                            _settings.DefaultVolume[0] = (byte)preMute[0];
                            _settings.DefaultMuted[0] = muted[0] = true;
                            _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.PaleVioletRed);
                        }
                        else
                        {
                            _media_players[0].SetVolume(preMute[0]);
                            _tuner1_properties.UpdateValue("volume_slider_1", preMute[0].ToString());
                            _settings.DefaultMuted[0] = muted[0] = false;
                            _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.Transparent);
                        }
                    }
                    else
                    {
                        if (!muted[1])
                        {
                            preMute[1] = _media_players[1].GetVolume();
                            _media_players[1].SetVolume(0);
                            _tuner2_properties.UpdateValue("volume_slider_2", "0");
                            _settings.DefaultVolume[1] = (byte)preMute[1];
                            _settings.DefaultMuted[1] = muted[1] = true;
                            _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.PaleVioletRed);
                        }
                        else
                        {
                            _media_players[1].SetVolume(preMute[1]);
                            _tuner2_properties.UpdateValue("volume_slider_2", preMute[1].ToString());
                            _settings.DefaultMuted[1] = muted[1] = false;
                            _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.Transparent);
                        }

                    }

                    break;
                case 1: // snaphost
                    Log.Information("Snapshot: " + tuner.ToString());
                    _media_players[tuner].SnapShot(_mediapath + CommonFunctions.GenerateTimestampFilename() + ".png");

                    break;
                case 2: // mute
                    Log.Information("Record: " + tuner.ToString());

                    if (_ts_recorders[tuner].record)
                    {
                        ClearIndicator(ref ((tuner == 0) ? ref indicatorStatus1 : ref indicatorStatus2), PropertyIndicators.RecordingIndicator);
                        _ts_recorders[tuner].record = false;    // stop recording
                    }
                    else
                    {
                        SetIndicator(ref ((tuner == 0) ? ref indicatorStatus1 : ref indicatorStatus2), PropertyIndicators.RecordingIndicator);
                        _ts_recorders[tuner].record = true;     // start recording
                    }

                    if (tuner == 0)
                        _tuner1_properties.UpdateValue("media_controls_1", indicatorStatus1.ToString());
                    else
                        _tuner2_properties.UpdateValue("media_controls_2", indicatorStatus2.ToString());

                    break;
                case 3: // udp stream
                    Log.Information("UDP Stream: " + tuner.ToString());

                    if (_ts_streamers[tuner].stream)
                    {
                        ClearIndicator(ref ((tuner == 0) ? ref indicatorStatus1 : ref indicatorStatus2), PropertyIndicators.StreamingIndicator);
                        _ts_streamers[tuner].stream = false;
                    }
                    else
                    {
                        SetIndicator(ref ((tuner == 0) ? ref indicatorStatus1 : ref indicatorStatus2), PropertyIndicators.StreamingIndicator);
                        _ts_streamers[tuner].stream = true;
                    }

                    if (tuner == 0)
                        _tuner1_properties.UpdateValue("media_controls_1", indicatorStatus1.ToString());
                    else
                        _tuner2_properties.UpdateValue("media_controls_2", indicatorStatus2.ToString());

                    break;
            }
        }

        private void DynamicPropertyGroup_OnSliderChanged(string key, int value)
        {
            switch (key)
            {
                case "volume_slider_1":
                    // cancel mute
                    _settings.DefaultMuted[0] = muted[0] = false;
                    if (_media_players.Count > 0) 
                    {
                        _media_players[0]?.SetVolume(value);
                    }
                    
                    _settings.DefaultVolume[0] = (byte)value;
                    _tuner1_properties.UpdateMuteButtonColor("media_controls_1", Color.Transparent);
                    break;
                case "volume_slider_2":
                    // cancel mute
                    _settings.DefaultMuted[1] = muted[1] = false;
                    if (_media_players.Count > 0)
                    {
                        _media_players[1]?.SetVolume(value);
                    }
                    _settings.DefaultVolume[1] = (byte)value;
                    _tuner2_properties.UpdateMuteButtonColor("media_controls_2", Color.Transparent);
                    break;
            }
        }

        private void UpdateTSProperties(int tuner, TSStatus ts_status)
        {
            DynamicPropertyGroup _tuner = (tuner == 1 ? _tuner1_properties : _tuner2_properties);

            _tuner.UpdateValue("service_name", ts_status.ServiceName);
            _tuner.UpdateValue("service_name_provider", ts_status.ServiceProvider);
            _tuner.UpdateValue("null_packets", ts_status.NullPacketsPerc.ToString() + "%");

            if (tuner == 1)
            {
                last_service_name_0 = ts_status.ServiceName;
                last_service_provider_0 = ts_status.ServiceProvider;
            }
            else
            {
                last_service_name_1 = ts_status.ServiceName;
                last_service_provider_1 = ts_status.ServiceProvider;
            }

        }

        private void UpdateMediaProperties(int player, MediaStatus media_status)
        {
            int tuner = player + 1;

            DynamicPropertyGroup _tuner = (tuner == 1 ? _tuner1_properties : _tuner2_properties);

            _tuner.UpdateTitle("Tuner " + tuner.ToString() + " - " + _media_players[player].GetName());

            string video_res = media_status.VideoWidth.ToString() + " x " + media_status.VideoHeight.ToString();
            string audio_rate = media_status.AudioRate.ToString() + " Hz, " + media_status.AudioChannels.ToString() + " channels";

            _tuner.UpdateValue("video_codec", media_status.VideoCodec);
            _tuner.UpdateValue("video_resolution", video_res);
            _tuner.UpdateValue("audio_codec", media_status.AudioCodec);
            _tuner.UpdateValue("audio_rate", audio_rate);
        }

        private void UpdateTunerProperties(TunerStatus new_status)
        {

            double dbmargin = 0;
            double mer = Convert.ToDouble(new_status.T1P2_mer) / 10;
            double mer2 = Convert.ToDouble(new_status.T2P1_mer) / 10;

            // general
            _source_properties.UpdateValue("source_hw_ldpc_errors", new_status.errors_ldpc_count.ToString());

            switch(current_lnba_psu)
            {
                case 0:
                    _source_properties.UpdateValue("hw_lnba", "OFF");
                    break;
                case 1:
                    _source_properties.UpdateValue("hw_lnba", "Vertical (12V)");
                    break;
                case 2:
                    _source_properties.UpdateValue("hw_lnba", "Horizontal (18V)");
                    break;
            }

            switch (current_lnbb_psu)
            {
                case 0:
                    _source_properties.UpdateValue("hw_lnbb", "OFF");
                    break;
                case 1:
                    _source_properties.UpdateValue("hw_lnbb", "Vertical (12V)");
                    break;
                case 2:
                    _source_properties.UpdateValue("hw_lnbb", "Horizontal (18V)");
                    break;
            }


            // tuner 1 properties  *************
            _tuner1_properties.UpdateValue("demodstate", lookups.demod_state_lookup[new_status.T1P2_demod_status]);

            if (new_status.T1P2_demod_status > 1)
            {
                _tuner1_properties.UpdateColor("demodstate", Color.PaleGreen);
            }
            else
            {
                _tuner1_properties.UpdateColor("demodstate", Color.PaleVioletRed);
            }

            _tuner1_properties.UpdateValue("mer", mer.ToString() + " dB");
            _tuner1_properties.UpdateValue("lna_gain", new_status.T1P2_lna_gain.ToString());
            _tuner1_properties.UpdateValue("rf_input_level", new_status.T1P2_input_power_level.ToString() + " dB");
            _tuner1_properties.UpdateValue("symbol_rate", (new_status.T1P2_symbol_rate / 1000).ToString());
            _tuner1_properties.UpdateValue("ber", new_status.T1P2_ber.ToString());
            _tuner1_properties.UpdateValue("freq_carrier_offset", new_status.T1P2_frequency_carrier_offset.ToString());
            _tuner1_properties.UpdateValue("requested_freq_1", "(" + GetFrequency(0, true).ToString("N0") + ") (" + GetFrequency(0, false).ToString("N0") + ")");
            _tuner1_properties.UpdateValue("rf_input", (new_status.T1P2_rf_input == 1 ? "A" : "B"));
            _tuner1_properties.UpdateValue("stream_format", lookups.stream_format_lookups[Convert.ToInt32(new_status.T1P2_stream_format)].ToString());
            _tuner1_properties.UpdateValue("offset", current_offset_0.ToString());

            // clear ts data if not locked onto signal
            if (new_status.T1P2_demod_status < 2)
            {
                _tuner1_properties.UpdateValue("service_name_provider", "");
                _tuner1_properties.UpdateValue("service_name", "");
                _tuner1_properties.UpdateValue("stream_format", "");

                _tuner1_properties.UpdateValue("video_codec", "");
                _tuner1_properties.UpdateValue("video_resolution", "");                    
                _tuner1_properties.UpdateValue("audio_codec", "");
                _tuner1_properties.UpdateValue("audio_rate", "");

                // stop recording if recording
                if (_ts_recorders[0].record)
                {
                    ClearIndicator(ref indicatorStatus1, PropertyIndicators.RecordingIndicator);
                    _ts_recorders[0].record = false;    // stop recording
                    _tuner1_properties.UpdateValue("media_controls_1", indicatorStatus1.ToString());
                }

                // stop streaming
                if (_ts_streamers[0].stream)
                {
                    ClearIndicator(ref indicatorStatus1, PropertyIndicators.StreamingIndicator);
                    _ts_streamers[0].stream = false;    // stop streaming
                    _tuner1_properties.UpdateValue("media_controls_1", indicatorStatus1.ToString());
                }


            }

            // db margin / modcod
            string modcod_text = "Unknown";
            string db_margin_text = "";
            try
            {
                switch (new_status.T1P2_demod_status)
                {
                    case 2:
                        modcod_text = lookups.modcod_lookup_dvbs2[new_status.T1P2_modcode];
                        dbmargin = (mer - lookups.modcod_lookup_dvbs2_threshold[new_status.T1P2_modcode]);
                        db_margin_text = "D" + dbmargin.ToString("N1");
                        break;
                    case 3:
                        modcod_text = lookups.modcod_lookup_dvbs[new_status.T1P2_modcode];
                        dbmargin = (mer - lookups.modcod_lookup_dvbs_threshold[new_status.T1P2_modcode]);
                        db_margin_text = "D" + dbmargin.ToString("N1");
                        break;
                }
            }
            catch (Exception Ex)
            {
            }

            last_dbm_0 = db_margin_text;
            last_mer_0 = mer.ToString();

            _tuner1_properties.UpdateBigLabel(db_margin_text);
            //_tuner1_properties.UpdateValue("db_margin", db_margin_text);
            _tuner1_properties.UpdateValue("modcod", modcod_text);

            var data1 = _tuner1_properties.GetAll();
            data1.Add("frequency", GetFrequency(0, true).ToString());
            OnSourceData?.Invoke(data1, "Tuner 1");



            if (ts_devices == 2 && _tuner2_properties != null)
            {
                // tuner 2 properties  *************
                _tuner2_properties.UpdateValue("demodstate", lookups.demod_state_lookup[new_status.T2P1_demod_status]);

                if (new_status.T2P1_demod_status > 1)
                {
                    _tuner2_properties.UpdateColor("demodstate", Color.PaleGreen);
                }
                else
                {
                    _tuner2_properties.UpdateColor("demodstate", Color.PaleVioletRed);
                }

                _tuner2_properties.UpdateValue("mer", mer2.ToString() + " dB");
                _tuner2_properties.UpdateValue("lna_gain", new_status.T2P1_lna_gain.ToString());
                _tuner2_properties.UpdateValue("rf_input_level", new_status.T2P1_input_power_level.ToString() + " dB");
                _tuner2_properties.UpdateValue("symbol_rate", (new_status.T2P1_symbol_rate / 1000).ToString());
                _tuner2_properties.UpdateValue("ber", new_status.T2P1_ber.ToString());
                _tuner2_properties.UpdateValue("freq_carrier_offset", new_status.T2P1_frequency_carrier_offset.ToString());
                _tuner2_properties.UpdateValue("requested_freq_2", "(" + GetFrequency(1, true).ToString("N0") + ") (" + GetFrequency(1, false).ToString("N0") + ")");

                _tuner2_properties.UpdateValue("rf_input", (new_status.T2P1_rf_input == 1 ? "A" : "B"));
                _tuner2_properties.UpdateValue("stream_format", lookups.stream_format_lookups[Convert.ToInt32(new_status.T2P1_stream_format)].ToString());
                _tuner2_properties.UpdateValue("offset", current_offset_1.ToString());




                // clear ts data if not locked onto signal
                if (new_status.T2P1_demod_status < 2)
                {
                    _tuner2_properties.UpdateValue("service_provider_name", "");
                    _tuner2_properties.UpdateValue("service_name", "");
                    _tuner2_properties.UpdateValue("service_provider_name", "");
                    _tuner2_properties.UpdateValue("stream_format", "");

                    _tuner2_properties.UpdateValue("video_codec", "");
                    _tuner2_properties.UpdateValue("video_resolution", "");
                    _tuner2_properties.UpdateValue("audio_codec", "");
                    _tuner2_properties.UpdateValue("audio_rate", "");


                    // stop recording if recording
                    if (_ts_recorders[1].record)
                    {
                        ClearIndicator(ref indicatorStatus2, PropertyIndicators.RecordingIndicator);
                        _ts_recorders[1].record = false;    // stop recording
                        _tuner2_properties.UpdateValue("media_controls_2", indicatorStatus2.ToString());
                    }

                    // stop streaming
                    if (_ts_streamers[1].stream)
                    {
                        ClearIndicator(ref indicatorStatus2, PropertyIndicators.StreamingIndicator);
                        _ts_streamers[1].stream = false;    // stop streaming
                        _tuner2_properties.UpdateValue("media_controls_2", indicatorStatus2.ToString());
                    }


                }

                // db margin / modcod
                modcod_text = "Unknown";
                db_margin_text = "";
                try
                {
                    switch (new_status.T2P1_demod_status)
                    {
                        case 2:
                            modcod_text = lookups.modcod_lookup_dvbs2[new_status.T2P1_modcode];
                            dbmargin = (mer2 - lookups.modcod_lookup_dvbs2_threshold[new_status.T2P1_modcode]);
                            db_margin_text = "D" + dbmargin.ToString("N1");
                            break;
                        case 3:
                            modcod_text = lookups.modcod_lookup_dvbs[new_status.T2P1_modcode];
                            dbmargin = (mer2 - lookups.modcod_lookup_dvbs_threshold[new_status.T2P1_modcode]);
                            db_margin_text = "D" + dbmargin.ToString("N1");
                            break;
                    }
                }
                catch (Exception Ex)
                {
                }

                last_dbm_1 = db_margin_text;
                last_mer_1 = mer2.ToString();

                _tuner2_properties.UpdateBigLabel(db_margin_text);
                //_tuner2_properties.UpdateValue("db_margin", db_margin_text);
                _tuner2_properties.UpdateValue("modcod", modcod_text);

                var data2 = _tuner2_properties.GetAll();
                data2.Add("frequency", GetFrequency(1, true).ToString());
                OnSourceData?.Invoke(data2, "Tuner 2");

            }
        }

        private void _genericContextStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            Log.Information("Opening Context Menu :" + contextMenuStrip.SourceControl.Name + " tag = " + contextMenuStrip.SourceControl.Tag.ToString());

            contextMenuStrip.Items.Clear();

            switch (contextMenuStrip.SourceControl.Name)
            {
                // change frequency
                case "requested_freq_1":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Tuner Control", MinitiounerPropertyCommands.SETFREQUENCY, new int[] {0}));
                    break;
                case "requested_freq_2":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Tuner Control", MinitiounerPropertyCommands.SETFREQUENCY, new int[] { 1 }));
                    break;                  
                case "rf_input":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("A", MinitiounerPropertyCommands.SETRFINPUTA, new int[] { (int)contextMenuStrip.SourceControl.Tag - 1 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("B", MinitiounerPropertyCommands.SETRFINPUTB, new int[] { (int)contextMenuStrip.SourceControl.Tag - 1 }));
                    break;
                case "symbol_rate":
                    uint[] symbol_rates = new uint[] { 2000, 1500, 1000, 500, 333, 250, 125, 66 };
                    foreach (uint rate in symbol_rates)
                        contextMenuStrip.Items.Add(ConfigureMenuItem(rate.ToString(), MinitiounerPropertyCommands.SETSYMBOLRATE, new int[] { (int)contextMenuStrip.SourceControl.Tag - 1, (int)rate}));
                    break;
                case "offset":
                    int tuner = (int)contextMenuStrip.SourceControl.Tag - 1;
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Default: " + (tuner == 0 ? _settings.Offset1 : _settings.Offset2), MinitiounerPropertyCommands.SETOFFSET, new int[] { (int)contextMenuStrip.SourceControl.Tag - 1, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Zero" , MinitiounerPropertyCommands.SETOFFSET, new int[] { (int)contextMenuStrip.SourceControl.Tag - 1, 1 }));
                    break;
                case "hw_lnba":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("OFF", MinitiounerPropertyCommands.LNBA_OFF, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Vertical", MinitiounerPropertyCommands.LNBA_VERTICAL, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Horizontal", MinitiounerPropertyCommands.LNBA_HORIZONTAL, new int[] { 0, 0 }));
                    break;
                case "hw_lnbb":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("OFF", MinitiounerPropertyCommands.LNBB_OFF, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Vertical", MinitiounerPropertyCommands.LNBB_VERTICAL, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Horizontal", MinitiounerPropertyCommands.LNBB_HORIZONTAL, new int[] { 0, 0 }));
                    break;
            }

        }

        private ToolStripMenuItem ConfigureMenuItem(string Text, MinitiounerPropertyCommands command, int[] option)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.Click += (sender, e) =>
            {
                properties_OnPropertyMenuSelect(command, option);
            };

            return item;
        }



        private void properties_OnPropertyMenuSelect(MinitiounerPropertyCommands command, int[] options)
        {
            Log.Information("Config Change: " + command.ToString() + " - " + options.Length.ToString());

            int tuner = 0;

            switch (command)
            {
                case MinitiounerPropertyCommands.SETFREQUENCY:
                    tuner = options[0];
                    _tuner_forms[tuner].ShowTuner((int)(tuner == 0 ? current_frequency_0 : current_frequency_1), (int)(tuner == 0 ? current_sr_0 : current_sr_1), (int)(tuner == 0 ? current_offset_0 : current_offset_1));
                    break;
                case MinitiounerPropertyCommands.SETRFINPUTA:
                    ChangeRFInput((byte)options[0], nim.NIM_INPUT_TOP);
                    ResetVideo(options[0]);
                    break;
                case MinitiounerPropertyCommands.SETRFINPUTB:
                    ChangeRFInput((byte)options[0], nim.NIM_INPUT_BOTTOM);
                    ResetVideo(options[0]);
                    break;
                case MinitiounerPropertyCommands.SETSYMBOLRATE:
                    ChangeSymbolRate((byte)options[0],(uint)options[1]);
                    ResetVideo(options[0]);
                    break;
                case MinitiounerPropertyCommands.SETOFFSET:
                    tuner = options[0];

                    if (options[1] == 0)    // set to default
                        ChangeOffset((byte)tuner, ((tuner == 0) ? (int)_settings.Offset1 : (int)_settings.Offset2));
                    if (options[1] == 1)    // zero out
                        ChangeOffset((byte)tuner, 0);

                    break;

                default:
                    Log.Information("Unconfigured Command Change - " + command.ToString());
                    break;

                case MinitiounerPropertyCommands.LNBA_OFF:
                    current_lnba_psu = 0;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case MinitiounerPropertyCommands.LNBA_VERTICAL:
                    current_lnba_psu = 1;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case MinitiounerPropertyCommands.LNBA_HORIZONTAL:
                    current_lnba_psu = 2;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case MinitiounerPropertyCommands.LNBB_OFF:
                    current_lnbb_psu = 0;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case MinitiounerPropertyCommands.LNBB_VERTICAL:
                    current_lnbb_psu = 1;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
                case MinitiounerPropertyCommands.LNBB_HORIZONTAL:
                    current_lnbb_psu = 2;
                    change_frequency(0, current_frequency_0, current_sr_0, current_rf_input_0, current_tone_22kHz_P1, current_lnba_psu, current_lnbb_psu);
                    break;
            }
        }

        private void TunerControl_OnTunerChange(int id, uint freq)
        {
            Log.Information("set frequency : " + id.ToString() + "," + freq.ToString());
            SetFrequency(id, freq, id == 0 ? current_sr_0 : current_sr_1, false);
        }

        private void ResetVideo(int tuner)
        {
            if (VideoChangeCB != null)
            {
                VideoChangeCB(tuner + 1, false);
            }

        }

        public override Dictionary<string, string> GetSignalData(int device)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (device == 0)
            {
                data.Add("ServiceName", last_service_name_0);
                data.Add("ServiceProvider", last_service_provider_0);
                data.Add("dbMargin", last_dbm_0);
                data.Add("Mer", last_mer_0);
                data.Add("SR", current_sr_0.ToString());
                data.Add("Frequency", current_frequency_0.ToString());
            }

            if (device == 1)
            {
                data.Add("ServiceName", last_service_name_1);
                data.Add("ServiceProvider", last_service_provider_1);
                data.Add("dbMargin", last_dbm_1);
                data.Add("Mer", last_mer_1);
                data.Add("SR", current_sr_1.ToString());
                data.Add("Frequency", current_frequency_1.ToString());
            }

            return data;
        }
    }
}

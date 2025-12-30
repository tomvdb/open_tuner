using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using opentuner.MediaSources.Longmynd;
using Serilog;
using System.Globalization;

namespace opentuner.MediaSources.WinterHill
{
    public partial class WinterHillSource
    {
        private Control _parent;

        private DynamicPropertyGroup[] _tuner_properties = new DynamicPropertyGroup[4];
        private DynamicPropertyGroup _source_properties;

        ContextMenuStrip _genericContextStrip;

        private List<StoredFrequency> _frequency_presets = null;

        Dictionary<int, string> scanstate_lookup = new Dictionary<int, string>()
        {
            { 0 , "Hunting" },
            { 1 , "Header" },
            { 2 , "Lock DVB-S2" },
            { 3 , "Lock DVB-S" },
            { 0x80 , "Lost" },
            { 0x81 , "Timeout" },
            { 0x82 , "Idle" },
        };


        private bool BuildSourceProperties()
        {
            if (_parent == null)
            {
                Log.Information("Fatal Error: No Properties Panel");
                return false;
            }

            _genericContextStrip = new ContextMenuStrip();
            _genericContextStrip.Opening += _genericContextStrip_Opening;


            for (int c = ts_devices-1; c >= 0; c--)
            {
                _tuner_properties[c] = new DynamicPropertyGroup("Tuner " +  (c+1).ToString(), _parent);
                _tuner_properties[c].setID(c);  // set this before creating any items
                _tuner_properties[c].OnSlidersChanged += WinterHillSource_OnSlidersChanged;
                _tuner_properties[c].OnMediaButtonPressed += WinterHillSource_OnMediaButtonPressed;
                _tuner_properties[c].AddItem("demodstate", "Demod State", Color.PaleVioletRed);
                _tuner_properties[c].AddItem("mer", "Mer");

                if (hw_device == 2) // picotuner
                {
                    _tuner_properties[c].AddItem("rf_input", "RF Input", _genericContextStrip);
                }
                
                _tuner_properties[c].AddItem("frequency", "Frequency" ,_genericContextStrip);
                _tuner_properties[c].AddItem("offset", "Freq Offset", _genericContextStrip);
                _tuner_properties[c].AddItem("nim_frequency", "Nim Frequency");
                _tuner_properties[c].AddItem("symbol_rate", "Symbol Rate", _genericContextStrip);
                _tuner_properties[c].AddItem("modcod", "Modcod");
                _tuner_properties[c].AddItem("service_name", "Service Name");
                _tuner_properties[c].AddItem("service_name_provider", "Service Name Provider");
                _tuner_properties[c].AddItem("null_packets", "Null Packets");
                _tuner_properties[c].AddItem("ts_addr", "TS Address", _genericContextStrip);
                _tuner_properties[c].AddItem("ts_port", "TS Port");
                _tuner_properties[c].AddItem("video_codec", "Video Codec");
                _tuner_properties[c].AddItem("video_resolution", "Video Resolution");
                _tuner_properties[c].AddItem("audio_codec", "Audio Codec");
                _tuner_properties[c].AddItem("audio_rate", "Audio Rate");
                _tuner_properties[c].AddSlider("volume_slider_" + c.ToString(), "Volume", 0, 200);
                _tuner_properties[c].AddMediaControls("media_controls_" + c.ToString(), "Media Controls");

                muted[c] = _settings.DefaultMuted[c];
                preMute[c] = (int)_settings.DefaultVolume[c];

                if (!_settings.DefaultMuted[c])
                {
                    _tuner_properties[c].UpdateValue("volume_slider_" + c.ToString(), _settings.DefaultVolume[c].ToString());
                    _settings.DefaultVolume[c] = (uint)preMute[c];  // restore as changed by update volume_slider function
                    _tuner_properties[c].UpdateMuteButtonColor("media_controls_" + c.ToString(), Color.Transparent);
                }
                else
                {
                    _tuner_properties[c].UpdateValue("volume_slider_" + c.ToString(), "0");
                    _settings.DefaultVolume[c] = (uint)preMute[c];  // restore as changed by update volume_slider function
                    _tuner_properties[c].UpdateMuteButtonColor("media_controls_" + c.ToString(), Color.PaleVioletRed);
                }
            }

            // source properties
            _source_properties = new DynamicPropertyGroup("Hardware Properties", _parent);
            _source_properties.setID(99);
            _source_properties.AddItem("hardware", "Hardware");

            if (hw_device == 1)
            {
                _source_properties.UpdateValue("hardware", "WinterHill");
            }
            else
            {
                _source_properties.UpdateValue("hardware", "Picotuner (ETH)");
                _source_properties.AddItem("hw_lnba", "LNB-A Power Supply", _genericContextStrip);
                _source_properties.AddItem("hw_lnbb", "LNB-B Power Supply", _genericContextStrip);
            }

            _tuner_forms = new List<TunerControlForm>();

            // tuner for each device
            for (int c = 0; c < ts_devices; c++)
            {
                var tunerControl = new TunerControlForm(c, 0, 0, (uint)_current_offset[c], this);
                tunerControl.OnTunerChange += TunerControl_OnTunerChange;

                _tuner_forms.Add(tunerControl);
            }

            return true;
        }

        private void TunerControl_OnTunerChange(int id, uint freq)
        {
            Log.Information("set frequency : " + id.ToString() + "," + freq.ToString());
            SetFrequency(id, freq,  (uint)_current_sr[id] , false);
        }

        private void _genericContextStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            Log.Information("Opening Context Menu :" + contextMenuStrip.SourceControl.Name + ", Tag: " + contextMenuStrip.SourceControl.Tag);

            contextMenuStrip.Items.Clear();

            switch (contextMenuStrip.SourceControl.Name)
            {
                case "symbol_rate":
                    uint[] symbol_rates = new uint[] { 2000, 1500, 1000, 500, 333, 250, 125, 66 };
                    foreach (uint rate in symbol_rates)
                        contextMenuStrip.Items.Add(ConfigureMenuItem(rate.ToString(), LongmyndPropertyCommands.SETSYMBOLRATE, new int[] { (int)contextMenuStrip.SourceControl.Tag, (int)rate }));
                    break;

                case "hw_lnba":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("OFF", LongmyndPropertyCommands.LNBA_OFF, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Vertical", LongmyndPropertyCommands.LNBA_VERTICAL, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Horizontal", LongmyndPropertyCommands.LNBA_HORIZONTAL, new int[] { 0, 0 }));
                    break;
                case "hw_lnbb":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("OFF", LongmyndPropertyCommands.LNBB_OFF, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Vertical", LongmyndPropertyCommands.LNBB_VERTICAL, new int[] { 0, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Switch Horizontal", LongmyndPropertyCommands.LNBB_HORIZONTAL, new int[] { 0, 0 }));
                    break;

                case "rf_input":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("A", LongmyndPropertyCommands.SETRFINPUTA, new int[] { (int)contextMenuStrip.SourceControl.Tag }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("B", LongmyndPropertyCommands.SETRFINPUTB, new int[] { (int)contextMenuStrip.SourceControl.Tag  }));
                    break;


                case "offset":
                    int tuner = (int)contextMenuStrip.SourceControl.Tag;
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Default: " + _settings.DefaultOffset[tuner], LongmyndPropertyCommands.SETOFFSET, new int[] { (int)contextMenuStrip.SourceControl.Tag, 0 }));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Zero", LongmyndPropertyCommands.SETOFFSET, new int[] { (int)contextMenuStrip.SourceControl.Tag, 1 }));
                    break;
                // change frequency
                case "frequency":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Tuner Control", LongmyndPropertyCommands.SETFREQUENCY, new int[] { (int)contextMenuStrip.SourceControl.Tag }));

                    if (_frequency_presets != null)
                    {
                        contextMenuStrip.Items.Add(new ToolStripSeparator());

                        for (int c = 0; c < _frequency_presets.Count; c++)
                        {
                            contextMenuStrip.Items.Add(ConfigureMenuItem(_frequency_presets[c].Name + " (" + _frequency_presets[c].Frequency + ")", LongmyndPropertyCommands.SETPRESET, new int[] { (int)contextMenuStrip.SourceControl.Tag, c }));
                        }
                    }
                    break;
                case "ts_addr":

                    // get local ip's
                    if (_LocalIp.Length == 0)
                    {
                        Log.Error("Warning: No Ip's detected");
                    }
                    else
                    {
                        contextMenuStrip.Items.Add(ConfigureMenuItem("Update TS to " + _LocalIp, LongmyndPropertyCommands.SETTSLOCAL, new int[] { (int)contextMenuStrip.SourceControl.Tag }));
                    }
                    break;
            }
        }

        private ToolStripMenuItem ConfigureMenuItem(string Text, LongmyndPropertyCommands command, int[] option)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.Click += (sender, e) =>
            {
                properties_OnPropertyMenuSelect(command, option);
            };

            return item;
        }

        private void properties_OnPropertyMenuSelect(LongmyndPropertyCommands command, int[] option)
        {
            Log.Information("Config Change: " + command.ToString() + " - " + option.ToString());

            int tuner = 0;

            switch (command)
            {
                case LongmyndPropertyCommands.LNBA_OFF:
                    UDPSetVoltage(0, 0);
                    break;

                case LongmyndPropertyCommands.LNBA_VERTICAL:
                    UDPSetVoltage(0, 13);
                    break;

                case LongmyndPropertyCommands.LNBA_HORIZONTAL:
                    UDPSetVoltage(0, 18);
                    break;

                case LongmyndPropertyCommands.LNBB_OFF:
                    UDPSetVoltage(1, 0);
                    break;

                case LongmyndPropertyCommands.LNBB_VERTICAL:
                    UDPSetVoltage(1, 13);
                    break;

                case LongmyndPropertyCommands.LNBB_HORIZONTAL:
                    UDPSetVoltage(1, 18);
                    break;


                case LongmyndPropertyCommands.SETRFINPUTA:
                    tuner = option[0];

                    SetRFPort(tuner, 0);
                    //_settings.RFPort[tuner] = 0;

                    //SetFrequency(tuner, (uint)_current_frequency[tuner], (uint)_current_sr[tuner], false);
                    break;

                case LongmyndPropertyCommands.SETRFINPUTB:
                    tuner = option[0];

                    SetRFPort(tuner, 1);
                    //_settings.RFPort[tuner] = 1;

                    //SetFrequency(tuner, (uint)_current_frequency[tuner], (uint)_current_sr[tuner], false);
                    break;

                case LongmyndPropertyCommands.SETOFFSET:
                    tuner = option[0];
                    
                    switch (option[1])
                    {
                        case 0: _current_offset[tuner] = (int)_settings.DefaultOffset[tuner]; break;
                        case 1: _current_offset[tuner] = 0; break;
                    }

                    SetFrequency(tuner, (uint)_current_frequency[tuner], (uint)_current_sr[tuner], false);
                    break;

                case LongmyndPropertyCommands.SETSYMBOLRATE:
                    tuner = option[0];
                    var new_rate = option[1];
                    SetFrequency(tuner, (uint)_current_frequency[option[0]], (uint)new_rate, false);
                    break;

                case LongmyndPropertyCommands.SETFREQUENCY:
                    tuner = option[0];
                    _tuner_forms[tuner].ShowTuner((uint)_current_frequency[tuner], (uint)_current_sr[tuner], (uint)_current_offset[tuner]);
                    break;

                case LongmyndPropertyCommands.SETPRESET:
                    tuner = option[0];
                    int presetNumber = option[1];

                    if (_frequency_presets != null)
                    {
                        if (_frequency_presets.Count > presetNumber)
                        {
                            Log.Information("Tuning to preset " + presetNumber);
                            Log.Information("Preset Name: " + _frequency_presets[presetNumber].Name);
                            Log.Information("Preset Frequency: " + _frequency_presets[presetNumber].Frequency.ToString());
                            Log.Information("Preset Offset: " + _frequency_presets[presetNumber].Offset.ToString());
                            Log.Information("Preset SymbolRate: " + _frequency_presets[presetNumber].SymbolRate.ToString());
                            Log.Information("Preset RF Input: " + _frequency_presets[presetNumber].RFInput.ToString());

                            _current_offset[tuner] = (int)_frequency_presets[presetNumber].Offset;
                            _settings.RFPort[tuner] = (uint)(_frequency_presets[presetNumber].RFInput- 1);
                            SetFrequency(tuner, _frequency_presets[presetNumber].Frequency, _frequency_presets[presetNumber].SymbolRate, true) ;
                        }
                    }
                    break;

                case LongmyndPropertyCommands.SETTSLOCAL:

                    if (_LocalIp.Length > 0)
                    {
                        Log.Information("Updating TS Ip to " + _LocalIp);

                        if (hw_device == 1)
                        {
                            WSSetTS(option[0]);
                        }
                        else
                        {
                            // to change ts for pico tuner ethernet we need to send a tuning command
                            SetFrequency(tuner, (uint)_current_frequency[option[0]], (uint)_current_sr[option[0]], false);
                        }

                        // reset status

                        VideoChangeCB?.Invoke(option[0] + 1, false);
                        playing[option[0]] = false;
                        _tuner_properties[option[0]].UpdateColor("demodstate", Color.PaleVioletRed);
                        demodstate[option[0]] = -1;
                    }
                    break;

                default:
                    Log.Warning("Unconfigured Command Change - " + command.ToString());
                    break;
            }
        }

//        public void SetIndicator(ref int indicatorInput, PropertyIndicators indicator)
//        {
//            indicatorInput |= (byte)(1 << (int)indicator);
//        }

//        public void ClearIndicator(ref int indicatorInput, PropertyIndicators indicator)
//        {
//            indicatorInput &= (byte)~(1 << (int)indicator);
//        }

        private void MediaControlsHandler(int tuner, int function)
        {
            switch (function)
            {
                case 0: // mute
                    if (!muted[tuner])
                    {
                        preMute[tuner] = _media_player[tuner].GetVolume();
                        _media_player[tuner].SetVolume(0);
                        _tuner_properties[tuner].UpdateValue("volume_slider_" + tuner.ToString(), "0");
                        _settings.DefaultVolume[tuner] = (byte)preMute[tuner];
                        _settings.DefaultMuted[tuner] = muted[tuner] = true;
                        _tuner_properties[tuner].UpdateMuteButtonColor("media_controls_" + tuner.ToString(), Color.PaleVioletRed);
                    }
                    else
                    {
                        _media_player[tuner].SetVolume(preMute[tuner]);
                        _tuner_properties[tuner].UpdateValue("volume_slider_" + tuner.ToString(), preMute[tuner].ToString());
                        _settings.DefaultMuted[tuner] = muted[tuner] = false;
                        _tuner_properties[tuner].UpdateMuteButtonColor("media_controls_" + tuner.ToString(), Color.Transparent);
                    }

                    break;
                case 1: // snapshot
                    if (playing[tuner])
                    {
                        _media_player[tuner].SnapShot(_MediaPath + CommonFunctions.GenerateTimestampFilename() + ".png");
                    }
                    else
                    {
                        Log.Error("Can't do snapshot, not locked to a signal");
                    }

                    break;

                case 2: // record
                    if (_recorders[tuner].record)
                    {
                        _recorders[tuner].record = false;
//                        ClearIndicator(ref indicatorStatus[tuner], PropertyIndicators.RecordingIndicator);
                        _tuner_properties[tuner].UpdateRecordButtonColor("media_controls_" + tuner.ToString(), Color.Transparent);
                    }
                    else
                    {
                        if (demodstate[tuner] == 3 || demodstate[tuner] == 2)
                        {
                            _recorders[tuner].record = true;
//                            SetIndicator(ref indicatorStatus[tuner], PropertyIndicators.RecordingIndicator);
                            _tuner_properties[tuner].UpdateRecordButtonColor("media_controls_" + tuner.ToString(), Color.PaleVioletRed);
                        }
                        else
                        {
                            Log.Error("Can't record, not locked to a signal");
                        }
                    }

                    _tuner_properties[tuner].UpdateValue("media_controls_" + tuner.ToString(), indicatorStatus[tuner].ToString());

                    break;
                case 3: // udp stream
                    if (_streamer[tuner].stream)
                    {
                        _streamer[tuner].stream = false;
//                        ClearIndicator(ref indicatorStatus[tuner], PropertyIndicators.StreamingIndicator);
                        _tuner_properties[tuner].UpdateStreamButtonColor("media_controls_" + tuner.ToString(), Color.Transparent);
                    }
                    else
                    {
                        if (demodstate[tuner] == 3 || demodstate[tuner] == 2)
                        {
                            _streamer[tuner].stream = true;
//                            SetIndicator(ref indicatorStatus[tuner], PropertyIndicators.StreamingIndicator);
                            _tuner_properties[tuner].UpdateStreamButtonColor("media_controls_" + tuner.ToString(), Color.PaleTurquoise);
                        }
                        else
                        {
                            Log.Error("Can't stream, not locked to a signal");
                        }
                    }

                    _tuner_properties[tuner].UpdateValue("media_controls_" + tuner.ToString(), indicatorStatus[tuner].ToString());

                    break;
            }
        }

        private void WinterHillSource_OnMediaButtonPressed(string key, int function)
        {
            switch(key)
            {
                case "media_controls_0":
                    MediaControlsHandler(0, function);
                    break;
                case "media_controls_1":
                    MediaControlsHandler(1, function);
                    break;
                case "media_controls_2":
                    MediaControlsHandler(2, function);
                    break;
                case "media_controls_3":
                    MediaControlsHandler(3, function);
                    break;
            }
        }

        private bool[] muted = new bool[] {true, true, true, true};
        private int[] preMute = new int[] { 50, 50, 50, 50 };
        private int[] indicatorStatus = new int[] {0, 0, 0, 0};

        private void WinterHillSource_OnSlidersChanged(string key, int value)
        {
            // TODO: pull in id and simplify
            switch(key)
            {
                case "volume_slider_0":
                    _settings.DefaultMuted[0] = muted[0] = false;
                    if (_media_player.Count() > 0 )
                        _media_player[0]?.SetVolume(value);
                    _settings.DefaultVolume[0] = (byte)value;
                    _tuner_properties[0].UpdateMuteButtonColor("media_controls_0", Color.Transparent);
                    break;
                case "volume_slider_1":
                    _settings.DefaultMuted[1] = muted[1] = false;
                    if (_media_player.Count() > 1)
                        _media_player[1]?.SetVolume(value);
                    _settings.DefaultVolume[1] = (byte)value;
                    _tuner_properties[1].UpdateMuteButtonColor("media_controls_1", Color.Transparent);
                    break;
                case "volume_slider_2":
                    _settings.DefaultMuted[2] = muted[2] = false;
                    if (_media_player.Count() > 2)
                        _media_player[2]?.SetVolume(value);
                    _settings.DefaultVolume[2] = (byte)value;
                    _tuner_properties[2].UpdateMuteButtonColor("media_controls_2", Color.Transparent);
                    break;
                case "volume_slider_3":
                    _settings.DefaultMuted[3] = muted[3] = false;
                    if (_media_player.Count() > 3)
                        _media_player[3]?.SetVolume(value);
                    _settings.DefaultVolume[3] = (byte)value;
                    _tuner_properties[3].UpdateMuteButtonColor("media_controls_3", Color.Transparent);
                    break;
            }
        }

        private void UpdateMediaProperties(int player, MediaStatus media_status)
        {
            int tuner = player + 1;

            _tuner_properties[player].UpdateTitle("Tuner " + tuner.ToString() + " - " + _media_player[player].GetName());

            string video_res = media_status.VideoWidth.ToString() + " x " + media_status.VideoHeight.ToString();
            string audio_rate = media_status.AudioRate.ToString("#,##0") + " Hz, " + media_status.AudioChannels.ToString() + " channels";

            _tuner_properties[player].UpdateValue("video_codec", media_status.VideoCodec);
            _tuner_properties[player].UpdateValue("video_resolution", video_res);
            _tuner_properties[player].UpdateValue("audio_codec", media_status.AudioCodec);
            _tuner_properties[player].UpdateValue("audio_rate", audio_rate);
        }

        private void UpdateInfo(monitorMessage mm)
        {
            try
            {
                // still setting up
                if (!_videoPlayersReady)
                    return;

                if (_tuner_properties == null)
                    return;


                //Log.Information("REady for info");

                for (int c = 0; c < mm.rx.Length - 1; c++)
                {
                    ReceiverMessage rx = mm.rx[c + 1];

                    // provision for single messages from picotuner ethernet
                    if (rx.rx == 99)
                        continue;

                    if (demodstate[c] != rx.scanstate)
                    {
                        if (rx.scanstate == 2 || rx.scanstate == 3)
                        {
                            Log.Information("Playing" + c.ToString());
                            VideoChangeCB?.Invoke(c + 1, true);
                            playing[c] = true;
                            _tuner_properties[c].UpdateColor("demodstate", Color.PaleGreen);
                        }
                        else
                        {
                            _tuner_properties[c].UpdateBigLabel("");

                            Log.Information("Stopping " + c.ToString() + " - " + rx.scanstate.ToString());

                            VideoChangeCB?.Invoke(c + 1, false);
                            playing[c] = false;
                            _tuner_properties[c].UpdateColor("demodstate", Color.PaleVioletRed);
                            if (_recorders[c].record)
                            {
                                _recorders[c].record = false;
                                _tuner_properties[c].UpdateRecordButtonColor("media_controls_" + c.ToString(), Color.Transparent);
                            }
                            if (_streamer[c].stream)
                            {
                                _streamer[c].stream = false;
                                _tuner_properties[c].UpdateStreamButtonColor("media_controls_" + c.ToString(), Color.Transparent);
                            }


                        }

                        demodstate[c] = rx.scanstate;

                        float sent_freq = 0;

                        try
                        {
                            if (float.TryParse(rx.frequency, NumberStyles.Any, CultureInfo.InvariantCulture, out sent_freq))
                            {
                                _current_frequency[c] = Convert.ToInt32((sent_freq * 1000) - _current_offset[c]);
                            }
                        }
                        catch (Exception Ex)
                        {
                            Log.Error(Ex, "Frequency Parse Error : " + rx.frequency + " - " + (10.5f).ToString(CultureInfo.InvariantCulture));
                        }

                        uint symbol_rate = 0;

                        if (uint.TryParse(rx.symbol_rate, out symbol_rate))
                        {
                            _current_sr[c] = (int)symbol_rate;
                        }


                    }

                    
                    if (hw_device == 2)
                    {
                        for (int counter = 0; counter < 2; counter++)
                        {
                            switch (_settings.LNBVoltage[counter])
                            {
                                case 0:
                                    _source_properties.UpdateValue(counter == 0 ? "hw_lnba" : "hw_lnbb", "OFF");
                                    break;
                                case 13:
                                    _source_properties.UpdateValue(counter == 0 ? "hw_lnba" : "hw_lnbb", "Vertical (12V)");
                                    break;
                                case 18:
                                    _source_properties.UpdateValue(counter == 0 ? "hw_lnba" : "hw_lnbb", "Horizontal (18V)");
                                    break;
                            }
                        }
                    }
                    
                    _tuner_properties[c].UpdateValue("demodstate", scanstate_lookup[rx.scanstate]);
                    _tuner_properties[c].UpdateValue("mer", rx.mer);
                    _tuner_properties[c].UpdateValue("frequency", GetFrequency(c, true).ToString("#,##0") + "  (" + GetFrequency(c, false).ToString("#,##0") + ")");
                    _tuner_properties[c].UpdateValue("offset", _current_offset[c].ToString());
                    _tuner_properties[c].UpdateValue("nim_frequency", GetFrequency(c, false).ToString());
                    _tuner_properties[c].UpdateValue("symbol_rate", rx.symbol_rate);
                    _tuner_properties[c].UpdateValue("modcod", rx.modcod.ToString());
                    _tuner_properties[c].UpdateValue("service_name", rx.service_name);
                    _tuner_properties[c].UpdateValue("service_name_provider", rx.service_provider_name);
                    _tuner_properties[c].UpdateValue("null_packets", rx.null_percentage);
                    _tuner_properties[c].UpdateValue("ts_addr", rx.ts_addr);

                    if (hw_device == 2)
                    {
                        _tuner_properties[c].UpdateValue("rf_input", (rx.rfport == 0 ? "A" : "B"));
                    }

                    last_mer[c] = rx.mer;
                    last_service_name[c] = rx.service_name;
                    last_service_provider[c] = rx.service_provider_name;
                    last_dbm[c] = rx.dbmargin.ToString();

                    if (rx.ts_addr != _LocalIp)
                    {
                        _tuner_properties[c].UpdateColor("ts_addr", Color.PaleVioletRed);
                    }
                    else
                    {
                        _tuner_properties[c].UpdateColor("ts_addr", Color.Bisque);
                    }

                    _tuner_properties[c].UpdateValue("ts_port", rx.ts_port.ToString());
                    _tuner_properties[c].UpdateBigLabel("D" + rx.dbmargin.ToString());

                    //Log.Information("(S)ROLF Test: " + rx.mer);
                    //Log.Information("(D)ROLF Test: " + Convert.ToDouble(rx.mer).ToString());

                    CultureInfo specific_culture = CultureInfo.CreateSpecificCulture("en-US");

                    var source_data = new OTSourceData();
                    source_data.frequency = GetFrequency(c, true);
                    source_data.video_number = c;
                    double mer_d = 0.0;
                    double.TryParse(rx.mer, NumberStyles.Number, CultureInfo.InvariantCulture, out mer_d);
                    source_data.mer = mer_d;

                    //Log.Information(" * ROLF Test: " + (mer_d).ToString());

                    double db_margin_d = 0.0;
                    double.TryParse(rx.dbmargin, NumberStyles.Number, CultureInfo.InvariantCulture, out db_margin_d);
                    source_data.db_margin = db_margin_d;
                    int symbol_rate_i = 0;
                    int.TryParse(rx.symbol_rate, NumberStyles.Number, CultureInfo.InvariantCulture, out symbol_rate_i);
                    source_data.symbol_rate = symbol_rate_i;
                    source_data.demod_locked = (rx.scanstate == 2 || rx.scanstate == 3);
                    source_data.service_name = rx.service_name;

                    if (_media_player.Count() > c)
                    {
                        if (_media_player[c] != null)
                            source_data.volume = _media_player[c].GetVolume();
                    }

                    OnSourceData?.Invoke(c, source_data, "Tuner " + c.ToString());
                }
            }
            catch ( Exception Ex)
            {
                Log.Warning(Ex, "Error");
            }
        }

        private void WebSocketTimeout()
        {
            try
            {
                // still setting up
                if (!_videoPlayersReady)
                    return;

                if (_tuner_properties == null)
                    return;

                for (int c = 0; c < ts_devices; c++)
                {
                    demodstate[c] = 0x81;   // timeout
                    Log.Information("Stopping " + c.ToString() + " - " + demodstate[c].ToString());

                    VideoChangeCB?.Invoke(c + 1, false);
                    playing[c] = false;
                    _tuner_properties[c].UpdateColor("demodstate", Color.PaleVioletRed);
                    _tuner_properties[c].UpdateValue("demodstate", scanstate_lookup[demodstate[c]]);

                    //var data = _tuner_properties[c].GetAll();
                    //OnSourceData?.Invoke(c, data, "Tuner " + c.ToString());

                    var source_data = new OTSourceData();
                    source_data.frequency = GetFrequency(c, true);
                    source_data.video_number = 0;
                    source_data.mer = 0;
                    source_data.db_margin = 0;
                    source_data.demod_locked = false;

                    OnSourceData?.Invoke(c, source_data, "Tuner " + (c+1).ToString());
                }
            }
            catch (Exception Ex)
            {
                Log.Warning(Ex, "Error");
            }
        }

        public override Dictionary<string, string> GetSignalData(int device)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            // Gets a NumberFormatInfo associated with the en-US culture.
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

            data.Add("ServiceName", last_service_name[device]);
            data.Add("ServiceProvider", last_service_provider[device]);
            data.Add("dbMargin", last_dbm[device]);
            data.Add("Mer", last_mer[device]);
            data.Add("SR", _current_sr[device].ToString());
            data.Add("Frequency", ((float)(_current_frequency[device] + _current_offset[device])/1000.0f).ToString("F", nfi));

            return data;
        }

        public override void UpdateFrequencyPresets(List<StoredFrequency> FrequencyPresets)
        {
            _frequency_presets = FrequencyPresets;
        }

        public override void UpdateVolume(int device, int volume)
        {
            if (device >= _media_player.Count() || device < 0)
                return;

            if (_media_player[device] == null)
                return;

            int new_volume = _media_player[device].GetVolume() + volume;

            if (new_volume < 0) new_volume = 0;
            if (new_volume > 200) new_volume = 200;
            
            _media_player[device].SetVolume(new_volume);

            if (device >= _tuner_properties.Count())
                return;

            if (_tuner_properties[device] == null)
                return;

            _tuner_properties[device].UpdateValue("volume_slider_" + device.ToString(), new_volume.ToString());
        }

        public override void ToggleMute(int device)
        {
        }

        public override int GetVolume(int device)
        {
            if (device >= _media_player.Count() || device < 0)
                return -1;

            if (_media_player[device] == null)
                return -1;

            return _media_player[device].GetVolume();
        }
    }
}

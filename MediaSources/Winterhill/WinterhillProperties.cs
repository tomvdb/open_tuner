using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using opentuner.MediaSources.Longmynd;
using Serilog;
using System.Globalization;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource
    {
        private Control _parent;

        private DynamicPropertyGroup[] _tuner_properties = new DynamicPropertyGroup[4];

        ContextMenuStrip _genericContextStrip;


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


            for (int c = 3; c >= 0; c--)
            {
                _tuner_properties[c] = new DynamicPropertyGroup("Tuner " +  (c+1).ToString(), _parent);
                _tuner_properties[c].setID(c);  // set this before creating any items
                _tuner_properties[c].OnSlidersChanged += WinterhillSource_OnSlidersChanged;
                _tuner_properties[c].OnMediaButtonPressed += WinterhillSource_OnMediaButtonPressed;
                _tuner_properties[c].AddItem("demodstate", "Demod State", Color.PaleVioletRed);
                _tuner_properties[c].AddItem("mer", "Mer");
                _tuner_properties[c].AddItem("frequency", "Frequency" ,_genericContextStrip);
                _tuner_properties[c].AddItem("nim_frequency", "Nim Frequency");
                _tuner_properties[c].AddItem("symbol_rate", "Symbol Rate");
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
                }
                else
                {
                    _tuner_properties[c].UpdateMuteButtonColor("media_controls_" + c.ToString(), Color.Tomato);
                }
            }

            _tuner_forms = new List<TunerControlForm>();
            // tuner for each device
            for (int c = 0; c < ts_devices; c++)
            {
                var tunerControl = new TunerControlForm(c, 0, 0, (int)_settings.Offset[c], this);
                tunerControl.OnTunerChange += TunerControl_OnTunerChange;

                _tuner_forms.Add(tunerControl);
            }

            return true;
        }

        private void TunerControl_OnTunerChange(int id, uint freq, uint symbol_rate)
        {
            Log.Information("set frequency : " + id.ToString() + "," + freq.ToString() + " , " + symbol_rate.ToString());
            SetFrequency(id, freq, symbol_rate, false);
        }

        private void _genericContextStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            Log.Information("Opening Context Menu :" + contextMenuStrip.SourceControl.Name + ", Tag: " + contextMenuStrip.SourceControl.Tag);

            contextMenuStrip.Items.Clear();

            switch (contextMenuStrip.SourceControl.Name)
            {
                // change frequency
                case "frequency":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Change Frequency", LongmyndPropertyCommands.SETFREQUENCY, (int)contextMenuStrip.SourceControl.Tag));
                    break;
                case "ts_addr":

                    // get local ip's
                    if (_LocalIp.Length == 0)
                    {
                        Log.Error("Warning: No Ip's detected");
                    }
                    else
                    {
                        contextMenuStrip.Items.Add(ConfigureMenuItem("Update TS to " + _LocalIp, LongmyndPropertyCommands.SETTSLOCAL, (int)contextMenuStrip.SourceControl.Tag));
                    }
                    break;
            }
        }

        private ToolStripMenuItem ConfigureMenuItem(string Text, LongmyndPropertyCommands command, int option)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.Click += (sender, e) =>
            {
                properties_OnPropertyMenuSelect(command, option);
            };

            return item;
        }

        private void properties_OnPropertyMenuSelect(LongmyndPropertyCommands command, int option)
        {
            Log.Information("Config Change: " + command.ToString() + " - " + option.ToString());

            switch (command)
            {
                case LongmyndPropertyCommands.SETFREQUENCY:
                    int tuner = option;
                    _tuner_forms[tuner].ShowTuner(_current_frequency[tuner], _current_sr[tuner], (int)_settings.Offset[tuner]);
                    break;

                case LongmyndPropertyCommands.SETTSLOCAL:

                    if (_LocalIp.Length > 0)
                    {
                        Log.Information("Updating TS Ip to " + _LocalIp);
                        string wh_command = ("U" + (option + 1).ToString() + "," + _LocalIp.ToString());
                        Log.Debug(wh_command);
                        controlWS.Send(wh_command);
                        // reset status

                        VideoChangeCB?.Invoke(option + 1, false);
                        playing[option] = false;
                        _tuner_properties[option].UpdateColor("demodstate", Color.PaleVioletRed);
                        demodstate[option] = -1;
                    }
                    break;

                default:
                    Log.Warning("Unconfigured Command Change - " + command.ToString());
                    break;
            }
        }

        public void SetIndicator(ref int indicatorInput, PropertyIndicators indicator)
        {
            indicatorInput |= (byte)(1 << (int)indicator);
        }

        public void ClearIndicator(ref int indicatorInput, PropertyIndicators indicator)
        {
            indicatorInput &= (byte)~(1 << (int)indicator);
        }


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
                        _tuner_properties[tuner].UpdateMuteButtonColor("media_controls_" + tuner.ToString(), Color.Tomato);
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
                    _media_player[tuner].SnapShot(_MediaPath + CommonFunctions.GenerateTimestampFilename() + ".png");
                    break;

                case 2: // record
                    if (_recorders[tuner].record)
                    {
                        _recorders[tuner].record = false;
                        ClearIndicator(ref indicatorStatus[tuner], PropertyIndicators.RecordingIndicator);
                    }
                    else
                    {
                        if (demodstate[tuner] == 3 || demodstate[tuner] == 2)
                        {
                            _recorders[tuner].record = true;
                            SetIndicator(ref indicatorStatus[tuner], PropertyIndicators.RecordingIndicator);
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
                        ClearIndicator(ref indicatorStatus[tuner], PropertyIndicators.StreamingIndicator);
                    }
                    else
                    {                        
                        if (demodstate[tuner] == 3 || demodstate[tuner] == 2)
                        {
                            _streamer[tuner].stream = true;
                            SetIndicator(ref indicatorStatus[tuner], PropertyIndicators.StreamingIndicator);
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

        private void WinterhillSource_OnMediaButtonPressed(string key, int function)
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

        private void WinterhillSource_OnSlidersChanged(string key, int value)
        {
            // TODO: pull in id and simplify
            switch(key)
            {
                case "volume_slider_0":
                    _settings.DefaultMuted[0] = muted[0] = false;
                    _media_player[0]?.SetVolume(value);
                    _settings.DefaultVolume[0] = (byte)value;
                    break;
                case "volume_slider_1":
                    _settings.DefaultMuted[1] = muted[1] = false;
                    _media_player[1]?.SetVolume(value);
                    _settings.DefaultVolume[1] = (byte)value;
                    break;
                case "volume_slider_2":
                    _settings.DefaultMuted[2] = muted[2] = false;
                    _media_player[2]?.SetVolume(value);
                    _settings.DefaultVolume[2] = (byte)value;
                    break;
                case "volume_slider_3":
                    _settings.DefaultMuted[3] = muted[3] = false;
                    _media_player[3]?.SetVolume(value);
                    _settings.DefaultVolume[3] = (byte)value;
                    break;
            }
        }

        private void UpdateMediaProperties(int player, MediaStatus media_status)
        {
            int tuner = player + 1;

            _tuner_properties[player].UpdateTitle("Tuner " + tuner.ToString() + " - " + _media_player[player].GetName());

            string video_res = media_status.VideoWidth.ToString() + " x " + media_status.VideoHeight.ToString();
            string audio_rate = media_status.AudioRate.ToString() + " Hz, " + media_status.AudioChannels.ToString() + " channels";

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

                if (_tuner_properties == null) return;

                for (int c = 0; c < mm.rx.Length - 1; c++)
                {
                    ReceiverMessage rx = mm.rx[c + 1];

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
                            Log.Information("Stopping " + c.ToString() + " - " + rx.scanstate.ToString());

                            VideoChangeCB?.Invoke(c + 1, false);
                            playing[c] = false;
                            _tuner_properties[c].UpdateColor("demodstate", Color.PaleVioletRed);
                        }

                        demodstate[c] = rx.scanstate;

                        float sent_freq = 0;

                        try
                        {
                            if (float.TryParse(rx.frequency, NumberStyles.Any, CultureInfo.InvariantCulture, out sent_freq))
                            {
                                _current_frequency[c] = Convert.ToInt32((sent_freq * 1000) - _settings.Offset[c]);
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

                    _tuner_properties[c].UpdateValue("demodstate", scanstate_lookup[rx.scanstate]);
                    _tuner_properties[c].UpdateValue("mer", rx.mer.ToString());
                    _tuner_properties[c].UpdateValue("frequency", GetFrequency(c, true).ToString());
                    _tuner_properties[c].UpdateValue("nim_frequency", GetFrequency(c, false).ToString());
                    _tuner_properties[c].UpdateValue("symbol_rate", rx.symbol_rate.ToString());
                    _tuner_properties[c].UpdateValue("modcod", rx.modcod.ToString());
                    _tuner_properties[c].UpdateValue("service_name", rx.service_name.ToString());
                    _tuner_properties[c].UpdateValue("service_name_provider", rx.service_provider_name.ToString());
                    _tuner_properties[c].UpdateValue("null_packets", rx.null_percentage.ToString());
                    _tuner_properties[c].UpdateValue("ts_addr", rx.ts_addr.ToString());

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
                    _tuner_properties[c].UpdateBigLabel(rx.dbmargin.ToString());

                    var data = _tuner_properties[c].GetAll();
                    OnSourceData?.Invoke(data, "Tuner " + c.ToString());
                }
            }
            catch ( Exception Ex)
            {
                Log.Warning(Ex, "Error");
            }
        }

        public override Dictionary<string, string> GetSignalData(int device)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("ServiceName", last_service_name[device]);
                data.Add("ServiceProvider", last_service_provider[device]);
                data.Add("dbMargin", last_dbm[device]);
                data.Add("Mer", last_mer[device]);
                data.Add("SR", _current_sr[device].ToString());
                data.Add("Frequency", _current_frequency[device].ToString());

            return data;
        }
    }
}

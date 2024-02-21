using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource
    {
        private Control _parent;

        private DynamicPropertyGroup[] _tuner_properties = new DynamicPropertyGroup[4];

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
                Console.WriteLine("Fatal Error: No Properties Panel");
                return false;
            }

            for (int c = 3; c >= 0; c--)
            {
                _tuner_properties[c] = new DynamicPropertyGroup("Tuner " +  (c+1).ToString(), _parent);
                _tuner_properties[c].setID(c);
                _tuner_properties[c].OnSlidersChanged += WinterhillSource_OnSlidersChanged;
                _tuner_properties[c].OnMediaButtonPressed += WinterhillSource_OnMediaButtonPressed;
                _tuner_properties[c].AddItem("demodstate", "Demod State");
                _tuner_properties[c].AddItem("mer", "Mer");
                _tuner_properties[c].AddItem("frequency", "Frequency");
                _tuner_properties[c].AddItem("nim_frequency", "Nim Frequency");
                _tuner_properties[c].AddItem("symbol_rate", "Symbol Rate");
                _tuner_properties[c].AddItem("modcod", "Modcod");
                _tuner_properties[c].AddItem("service_name", "Service Name");
                _tuner_properties[c].AddItem("service_name_provider", "Service Name Provider");
                _tuner_properties[c].AddItem("null_packets", "Null Packets");
                _tuner_properties[c].AddItem("ts_addr", "TS Address");
                _tuner_properties[c].AddItem("ts_ip", "TS IP");
                _tuner_properties[c].AddItem("video_codec", "Video Codec");
                _tuner_properties[c].AddItem("video_resolution", "Video Resolution");
                _tuner_properties[c].AddItem("audio_codec", "Audio Codec");
                _tuner_properties[c].AddItem("audio_rate", "Audio Rate");
                _tuner_properties[c].AddSlider("volume_slider_" + c.ToString(), "Volume", 0, 200);
                _tuner_properties[c].AddMediaControls("media_controls_" + c.ToString(), "Media Controls");
            }
            return true;
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
                        muted[tuner] = true;
                    }
                    else
                    {
                        _media_player[tuner].SetVolume(preMute[tuner]);
                        _tuner_properties[tuner].UpdateValue("volume_slider_" + tuner.ToString(), preMute[tuner].ToString());
                        muted[tuner] = false;
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
                            Console.WriteLine("Can't record, not locked to a signal");
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
                    }

                    _tuner_properties[tuner].UpdateValue("media_controls_" + tuner.ToString(), indicatorStatus[tuner].ToString());

                    break;


            }
        }

        private void WinterhillSource_OnMediaButtonPressed(string key, int function)
        {
            switch(key)
            {
                case "media_controls_0": MediaControlsHandler(0, function); break;
                case "media_controls_1": MediaControlsHandler(1, function); break;
                case "media_controls_2": MediaControlsHandler(2, function); break;
                case "media_controls_3": MediaControlsHandler(3, function); break;
            }
        }

        private bool[] muted = new bool[] {false, false, false, false};
        private int[] preMute = new int[] { 0, 0, 0, 0 };
        private int[] indicatorStatus = new int[] {0, 0, 0, 0};

        private void WinterhillSource_OnSlidersChanged(string key, int value)
        {
            // TODO: pull in id and simplify
            switch(key)
            {
                case "volume_slider_0":
                    muted[0] = false;
                    _media_player[0]?.SetVolume(value);
                    _settings.DefaultVolume[0] = (byte)value;
                    break;
                case "volume_slider_1":
                    muted[1] = false;
                    _media_player[1]?.SetVolume(value);
                    _settings.DefaultVolume[1] = (byte)value;
                    break;
                case "volume_slider_2":
                    muted[2] = false;
                    _media_player[2]?.SetVolume(value);
                    _settings.DefaultVolume[2] = (byte)value;
                    break;
                case "volume_slider_3":
                    muted[3] = false;
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

            // still setting up
            if (!_videoPlayersReady)
                return;

            if (_tuner_properties == null) return;

            for (int c = 0; c < mm.rx.Length-1;c++)
            {
                ReceiverMessage rx = mm.rx[c+1];

                if (demodstate[c] != rx.scanstate) 
                {
                    if (rx.scanstate == 2 || rx.scanstate == 3)
                    {
                        Console.WriteLine("Playing" + c.ToString());
                        VideoChangeCB?.Invoke(c+1, true);
                        playing[c] = true;
                    }
                    else
                    {
                        Console.WriteLine("Stopping " + c.ToString() + " - " + rx.scanstate.ToString());
                        
                        VideoChangeCB?.Invoke(c+1, false);
                        playing[c] = false;
                    }

                    demodstate[c] = rx.scanstate;

                    float sent_freq = 0;
                    if (float.TryParse(rx.frequency, out sent_freq))
                    {
                        _current_frequency[c] = Convert.ToInt32((sent_freq * 1000) - _settings.Offset[c]);
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
                _tuner_properties[c].UpdateValue("ts_ip", rx.ts_port.ToString());
                _tuner_properties[c].UpdateBigLabel(rx.dbmargin.ToString());

                var data = _tuner_properties[c].GetAll();
                OnSourceData?.Invoke(data, "Tuner " + c.ToString());
            }
        }
    }
}

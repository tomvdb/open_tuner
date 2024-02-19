using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FTD2XX_NET.FTDI;
using System.Drawing;
using opentuner.MediaSources.Minitiouner;
using FlyleafLib.MediaFramework.MediaFrame;
using LibVLCSharp.Shared;
using Vortice.MediaFoundation;

namespace opentuner.MediaSources.Longmynd
{
    public enum LongmyndPropertyCommands
    {
        SETFREQUENCY,
    }

    public partial class LongmyndSource
    {

        public override event SourceDataChange OnSourceData;

        // properties management
        Control _parent = null;

        private static DynamicPropertyGroup _tuner1_properties = null;
        private static DynamicPropertyGroup _tuner2_properties = null;
        private static DynamicPropertyGroup _source_properties = null;

        // context menu strip
        ContextMenuStrip _genericContextStrip;

        private bool BuildSourceProperties()
        {
            if (_parent == null)
            {
                Console.WriteLine("Fatal Error: No Properties Panel");
                return false;
            }

            _genericContextStrip = new ContextMenuStrip();
            _genericContextStrip.Opening += _genericContextStrip_Opening;


            _tuner1_properties = ConfigureTunerProperties(1);
            _tuner1_properties.UpdateValue("volume_slider_1", _settings.DefaultVolume1.ToString());

            // source properties
            _source_properties = new DynamicPropertyGroup("Longmynd Properties", _parent);
            _source_properties.AddItem("source_ip", "IP Address");
            _source_properties.AddItem("source_ts_ip", "TS IP", _genericContextStrip);

            return true;
        }

        private DynamicPropertyGroup ConfigureTunerProperties(int tuner)
        {
            DynamicPropertyGroup dynamicPropertyGroup = new DynamicPropertyGroup("Tuner " + tuner.ToString(), _parent);
            dynamicPropertyGroup.OnSlidersChanged += DynamicPropertyGroup_OnSliderChanged;
            dynamicPropertyGroup.OnMediaButtonPressed += DynamicPropertyGroup_OnMediaButtonPressed;

            dynamicPropertyGroup.AddItem("demodstate", "Demod State", Color.PaleVioletRed);
            dynamicPropertyGroup.AddItem("mer", "Mer",_genericContextStrip);
            //dynamicPropertyGroup.AddItem("db_margin", "db Margin");
            //dynamicPropertyGroup.AddItem("rf_input_level", "RF Input Level");
            dynamicPropertyGroup.AddItem("rf_input", "RF Input", _genericContextStrip);
            dynamicPropertyGroup.AddItem("requested_freq", "Requested Freq", _genericContextStrip);
            dynamicPropertyGroup.AddItem("symbol_rate", "Symbol Rate");
            dynamicPropertyGroup.AddItem("modcod", "Modcod");
            //dynamicPropertyGroup.AddItem("lna_gain", "LNA Gain");
            dynamicPropertyGroup.AddItem("ber", "Ber");
            //dynamicPropertyGroup.AddItem("freq_carrier_offset", "Freq Carrier Offset");
            dynamicPropertyGroup.AddItem("stream_format", "Stream Format");
            dynamicPropertyGroup.AddItem("service_name", "Service Name");
            dynamicPropertyGroup.AddItem("service_name_provider", "Service Name Provider");
            dynamicPropertyGroup.AddItem("null_packets", "Null Packets");
            dynamicPropertyGroup.AddItem("video_codec", "Video Codec");
            dynamicPropertyGroup.AddItem("video_resolution", "Video Resolution");
            dynamicPropertyGroup.AddItem("audio_codec", "Audio Codec");
            dynamicPropertyGroup.AddItem("audio_rate", "Audio Rate");
            dynamicPropertyGroup.AddSlider("volume_slider_" + tuner.ToString(), "Volume", 0, 200);
            dynamicPropertyGroup.AddMediaControls("media_controls", "Media Controls");
            return dynamicPropertyGroup;
        }


        int preMute1 = 0;
        bool muted1 = false;
        int indicatorStatus = 0;


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
            switch (function)
            {
                case 0: // mute
                        if (!muted1)
                        {
                            preMute1 = _media_player.GetVolume();
                            _media_player.SetVolume(0);
                            _tuner1_properties.UpdateValue("volume_slider_1", "0");
                            muted1 = true;
                        }
                        else
                        {
                            _media_player.SetVolume(preMute1);
                            _tuner1_properties.UpdateValue("volume_slider_1", preMute1.ToString());
                            muted1 = false;
                        }
                    break;
                case 1: // snapshot
                    Console.WriteLine("Snapshot");
                    _media_player.SnapShot(_mediaPath + CommonFunctions.GenerateTimestampFilename() + ".png");

                    break;
                case 2: // record
                    Console.WriteLine("Record");

                    if (_recorder.record)
                    {
                        _recorder.record = false;    // stop recording
                        ClearIndicator(ref indicatorStatus, PropertyIndicators.RecordingIndicator);
                    }
                    else
                    {
                        // are we locked onto a signal ?
                        if (demodState >= 3)
                        {
                            _recorder.record = true;     // start recording
                            SetIndicator(ref indicatorStatus, PropertyIndicators.RecordingIndicator);
                        }
                        Console.WriteLine("Can't record, not locked to a signal");
                    }

                    _tuner1_properties.UpdateValue("media_controls", indicatorStatus.ToString());

                    break;

                case 3: // stream
                    Console.WriteLine("UDP Stream");

                    
                    if ( _streamer.stream)
                    {
                        _streamer.stream = false;   
                        ClearIndicator(ref indicatorStatus, PropertyIndicators.StreamingIndicator);
                    }
                    else
                    {
                        if (demodState >= 3)
                        {
                            _streamer.stream = true;
                            SetIndicator(ref indicatorStatus, PropertyIndicators.StreamingIndicator);
                        }
                    }

                    _tuner1_properties.UpdateValue("media_controls", indicatorStatus.ToString());

                    break;

            }
        }

        private void DynamicPropertyGroup_OnSliderChanged(string key, int value)
        {
            switch (key)
            {
                case "volume_slider_1":
                      muted1 = false;
                      _media_player?.SetVolume(value);
                      _settings.DefaultVolume1 = (byte)value;
                    break;
            }
        }

        private void UpdateMediaProperties(int player, MediaStatus media_status)
        {
            int tuner = player + 1;

            DynamicPropertyGroup _tuner = _tuner1_properties;

            _tuner.UpdateTitle("Tuner " + tuner.ToString() + " - " + _media_player.GetName());

            string video_res = media_status.VideoWidth.ToString() + " x " + media_status.VideoHeight.ToString();
            string audio_rate = media_status.AudioRate.ToString() + " Hz, " + media_status.AudioChannels.ToString() + " channels";

            _tuner.UpdateValue("video_codec", media_status.VideoCodec);
            _tuner.UpdateValue("video_resolution", video_res);
            _tuner.UpdateValue("audio_codec", media_status.AudioCodec);
            _tuner.UpdateValue("audio_rate", audio_rate);
        }




        private void _genericContextStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            Console.WriteLine("Opening Context Menu :" + contextMenuStrip.SourceControl.Name);

            contextMenuStrip.Items.Clear();

            switch (contextMenuStrip.SourceControl.Name)
            {
                // change frequency
                case "requested_freq":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Change Frequency", LongmyndPropertyCommands.SETFREQUENCY, 0));
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
            Console.WriteLine("Config Change: " + command.ToString() + " - " + option.ToString());

            switch (command)
            {
                case LongmyndPropertyCommands.SETFREQUENCY:
                    MessageBox.Show("Change Frequency");
                    break;

                default:
                    Console.WriteLine("Unconfigured Command Change - " + command.ToString());
                    break;
            }
        }


    }
}

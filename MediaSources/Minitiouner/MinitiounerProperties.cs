using LibVLCSharp.Shared;
using Newtonsoft.Json.Linq;
using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Minitiouner
{
    public partial class MinitiounerSource
    {
        // properties management
        Control _parent = null;

        private static DynamicPropertyGroup _tuner1_properties = null;
        private static DynamicPropertyGroup _tuner2_properties = null;
        private static DynamicPropertyGroup _source_properties = null;

        private bool BuildSourceProperties()
        {
            if (_parent == null)
            {
                Console.WriteLine("Fatal Error: No Properties Panel");
                return false;
            }

            if (ts_devices == 2)
            {
                _tuner2_properties = ConfigureTunerProperties(2);
            }

            _tuner1_properties = ConfigureTunerProperties(1);

            // source properties
            _source_properties = new DynamicPropertyGroup("Minitiouner Properties", _parent);
            _source_properties.AddItem("source_hw_interface", "Hardware Interface");
            _source_properties.AddItem("source_hw_ldpc_errors", "LPDC Errors");

            return true;
        }

        private DynamicPropertyGroup ConfigureTunerProperties(int tuner)
        {
            DynamicPropertyGroup dynamicPropertyGroup = new DynamicPropertyGroup("Tuner " +  tuner.ToString(), _parent);
            dynamicPropertyGroup.OnSlidersChanged += DynamicPropertyGroup_OnSliderChanged;

            dynamicPropertyGroup.AddItem("demodstate", "Demod State");
            dynamicPropertyGroup.AddItem("mer", "Mer");
            dynamicPropertyGroup.AddItem("db_margin", "db Margin");
            dynamicPropertyGroup.AddItem("rf_input_level", "RF Input Level");
            dynamicPropertyGroup.AddItem("rf_input", "RF Input");
            dynamicPropertyGroup.AddItem("requested_freq", "Requested Freq");
            dynamicPropertyGroup.AddItem("symbol_rate", "Symbol Rate");
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
            return dynamicPropertyGroup;
        }

        private void DynamicPropertyGroup_OnSliderChanged(string key, int value)
        {
            switch (key)
            {
                case "volume_slider_1":
                    if (_media_players.Count > 0)
                    {
                        _media_players[0].SetVolume(value); 
                    }
                    break;
                case "volume_slider_2":
                    if (_media_players.Count > 0)
                    {
                        _media_players[1].SetVolume(value);
                    }
                    break;
            }
        }

        private void UpdateTSProperties(int tuner, TSStatus ts_status)
        {
            DynamicPropertyGroup _tuner = (tuner == 1 ? _tuner1_properties : _tuner2_properties);

            _tuner.UpdateValue("service_name", ts_status.ServiceName);
            _tuner.UpdateValue("service_name_provider", ts_status.ServiceProvider);
            _tuner.UpdateValue("null_packets", ts_status.NullPacketsPerc.ToString() + "%");

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

            // tuner 1 properties  *************
            _tuner1_properties.UpdateValue("demodstate", lookups.demod_state_lookup[new_status.T1P2_demod_status]);
            _tuner1_properties.UpdateValue("mer", mer.ToString() + " dB");
            _tuner1_properties.UpdateValue("lna_gain", new_status.T1P2_lna_gain.ToString());
            _tuner1_properties.UpdateValue("rf_input_level", new_status.T1P2_input_power_level.ToString() + " dB");
            _tuner1_properties.UpdateValue("symbol_rate", (new_status.T1P2_symbol_rate / 1000).ToString());
            _tuner1_properties.UpdateValue("ber", new_status.T1P2_ber.ToString());
            _tuner1_properties.UpdateValue("freq_carrier_offset", new_status.T1P2_frequency_carrier_offset.ToString());
            _tuner1_properties.UpdateValue("requested_freq", GetCurrentFrequency(0, true).ToString("N0"));
            _tuner1_properties.UpdateValue("rf_input", (new_status.T1P2_rf_input == 1 ? "A" : "B"));
            _tuner1_properties.UpdateValue("stream_format", lookups.stream_format_lookups[Convert.ToInt32(new_status.T1P2_stream_format)].ToString());

            // clear ts data if not locked onto signal
            if (new_status.T1P2_demod_status < 2)
            {
                _tuner1_properties.UpdateValue("service_provider_name", "");
                _tuner1_properties.UpdateValue("service_name", "");
                _tuner1_properties.UpdateValue("service_provider_name", "");
                _tuner1_properties.UpdateValue("stream_format", "");
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

            _tuner1_properties.UpdateValue("db_margin", db_margin_text);
            _tuner1_properties.UpdateValue("modcod", modcod_text);


            if (ts_devices == 2 && _tuner2_properties != null)
            {
                // tuner 1 properties  *************
                _tuner2_properties.UpdateValue("demodstate", lookups.demod_state_lookup[new_status.T2P1_demod_status]);
                _tuner2_properties.UpdateValue("mer", mer.ToString() + " dB");
                _tuner2_properties.UpdateValue("lna_gain", new_status.T2P1_lna_gain.ToString());
                _tuner2_properties.UpdateValue("rf_input_level", new_status.T2P1_input_power_level.ToString() + " dB");
                _tuner2_properties.UpdateValue("symbol_rate", (new_status.T2P1_symbol_rate / 1000).ToString());
                _tuner2_properties.UpdateValue("ber", new_status.T2P1_ber.ToString());
                _tuner2_properties.UpdateValue("freq_carrier_offset", new_status.T2P1_frequency_carrier_offset.ToString());
                _tuner2_properties.UpdateValue("requested_freq", GetCurrentFrequency(0, true).ToString("N0"));
                _tuner2_properties.UpdateValue("rf_input", (new_status.T2P1_rf_input == 1 ? "A" : "B"));
                _tuner2_properties.UpdateValue("stream_format", lookups.stream_format_lookups[Convert.ToInt32(new_status.T2P1_stream_format)].ToString());

                // clear ts data if not locked onto signal
                if (new_status.T2P1_demod_status < 2)
                {
                    _tuner2_properties.UpdateValue("service_provider_name", "");
                    _tuner2_properties.UpdateValue("service_name", "");
                    _tuner2_properties.UpdateValue("service_provider_name", "");
                    _tuner2_properties.UpdateValue("stream_format", "");
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
                            dbmargin = (mer - lookups.modcod_lookup_dvbs2_threshold[new_status.T2P1_modcode]);
                            db_margin_text = "D" + dbmargin.ToString("N1");
                            break;
                        case 3:
                            modcod_text = lookups.modcod_lookup_dvbs[new_status.T2P1_modcode];
                            dbmargin = (mer - lookups.modcod_lookup_dvbs_threshold[new_status.T2P1_modcode]);
                            db_margin_text = "D" + dbmargin.ToString("N1");
                            break;
                    }
                }
                catch (Exception Ex)
                {
                }

                _tuner2_properties.UpdateValue("db_margin", db_margin_text);
                _tuner2_properties.UpdateValue("modcod", modcod_text);
            }
        }

    }
}

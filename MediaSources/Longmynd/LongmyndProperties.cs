using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Longmynd
{
    public partial class LongmyndSource
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

            _tuner1_properties = ConfigureTunerProperties(1);
            _tuner1_properties.UpdateValue("volume_slider_1", _settings.DefaultVolume.ToString());

            // source properties
            _source_properties = new DynamicPropertyGroup("Longmynd Properties", _parent);
            _source_properties.AddItem("source_ip", "IP Address");
            _source_properties.AddItem("source_ts_ip", "TS IP");

            return true;
        }

        private DynamicPropertyGroup ConfigureTunerProperties(int tuner)
        {
            DynamicPropertyGroup dynamicPropertyGroup = new DynamicPropertyGroup("Tuner " + tuner.ToString(), _parent);
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
                      _media_player?.SetVolume(value);
                      _settings.DefaultVolume = (byte)value;
                    break;
            }
        }

    }
}

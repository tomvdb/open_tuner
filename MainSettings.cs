using opentuner.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public class MainSettings : GenericSettings
    {
        [Group("Settings 1")]
        [FriendlyName("Media Path")]
        public string media_path = "";
        public string media_video_path = "";

        [Group("Settings 2")]
        public bool enable_spectrum_checkbox = true;
        public bool enable_chatform_checkbox = true;
        public bool enable_mqtt_checkbox = true;
        public bool enable_quicktune_checkbox = true;
        public bool enable_datvreporter_checkbox = false;

        // future
        public bool enable_pluto_checkbox = false;

        public int default_source = 0;
        public bool mute_at_startup = true;

        public bool auto_connect = false;

        public bool hide_properties = false; // can also be toggled with CTRL-P
        public bool hide_ExtraTool = false;  // can also be toggled with CTRL-E
        public bool[] show_video_info = { true, true, true, true };

        public int[] mediaplayer_preferences = { 0, 1, 1, 1 };
        public bool[] mediaplayer_windowed = { false, false, false, false };
        public string[] streamer_udp_hosts = { "127.0.0.1", "127.0.0.1", "127.0.0.1", "127.0.0.1" };
        public int[] streamer_udp_ports = { 5000, 5001, 5002, 5003 };

        // loaded on startup and updated on exit
        public int gui_window_width = -1;
        public int gui_window_height = -1;
        public int gui_window_x = -1;
        public int gui_window_y = -1;
        public int gui_window_state = 0;
        public int gui_main_splitter_position = 436;
    }
}

using opentuner.MediaSources;
using opentuner.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace opentuner.ExtraFeatures.QuickTuneControl
{
    public class QuickTuneControl
    {
        QuickTuneControlSettings _settings;
        SettingsManager<QuickTuneControlSettings> _settingsManager;

        UdpListener[] _udpListeners;
        OTSource _videoSource;

        public QuickTuneControl(OTSource VideoSource) 
        {
            _videoSource = VideoSource;
            _settings = new QuickTuneControlSettings();
            _settingsManager = new SettingsManager<QuickTuneControlSettings>("quicktune_settings");

            _settings = _settingsManager.LoadSettings(_settings);

            _udpListeners = new UdpListener[VideoSource.GetVideoSourceCount()];

            // udp listener
            for (int c = 0; c < _udpListeners.Length; c++) 
            {
                _udpListeners[c] = new UdpListener(_settings.UDPListenPorts[c], c);
                _udpListeners[c].DataReceived += QuickTuneControl_DataReceived;
                _udpListeners[c].StartListening();
            }

        }

        private void QuickTuneControl_DataReceived(object sender, DataReceivedEventArgs e)
        {
            UdpListener udpListener = (UdpListener)sender;

            try
            {
                Log.Information("UDP Received (" + udpListener.ID.ToString() + "): " + e.Message);

                string[] properties = e.Message.Split(',');

                uint freq = 0;
                uint offset = 0;
                uint sr = 0;

                uint.TryParse(properties[1].Substring(5), out freq);
                uint.TryParse(properties[2].Substring(7), out offset);
                uint.TryParse(properties[4].Substring(6), out sr);

                Log.Information("New Freq Request (" + udpListener.ID.ToString() + ") = " + (freq - offset).ToString() + "," + sr.ToString() + " ks");
                _videoSource.SetFrequency(udpListener.ID, freq-offset, sr, false);
                
            }
            catch (Exception Ex)
            {

            }

        }

        public void Close()
        {
            for (int c = 0; c < _udpListeners.Length; c++)
            {
                _udpListeners[c].StopListening();
            }
        }
    }
}

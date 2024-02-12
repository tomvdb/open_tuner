using CoreAudio;
using opentuner.ExtraFeatures.MqttClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.Transmit
{
    public class F5OEOEPlutoControl
    {
        private OTMqttClient _mqtt_client;

        private string _detected_callsign = "";

        public bool _callsign_configured = false;

        public F5OEOEPlutoControl(OTMqttClient MqttClient)
        {
            _mqtt_client = MqttClient;
            _mqtt_client.OnMqttMessageReceived += _mqtt_client_OnMqttMessageReceived;
        }

        private void _mqtt_client_OnMqttMessageReceived(MqttMessage Message)
        {

            if (Message.Topic.Contains("dt/pluto"))
            {
                string[] parts = Message.Topic.Split('/');

                if (!_callsign_configured)
                {
                    if (parts[2].ToUpper() != "NOCALL")
                    {
                        _callsign_configured = true;
                    }
                }
            }
        }

        // doesnt require a reboot command - will reboot automatically
        // TODO: mqtt needs to automatically reconnect
        public void ConfigureCallsignAndReboot(string Callsign)
        {
            Console.WriteLine("Configure Callsign");

            Task.Run(async () =>
            {
                await _mqtt_client.SendMqttCommand("cmd/pluto/call", Callsign);
            });

        }

        // doesn't seem to work
        public void Reboot()
        {
            Console.WriteLine("Reboot");

            Task.Run(async () =>
            {
                await _mqtt_client.SendMqttCommand("system/reboot", "reboot");
            });
        }

        
    }
}

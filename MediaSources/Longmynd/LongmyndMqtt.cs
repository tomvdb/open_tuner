using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Vortice.XAudio2;
using System.Drawing;

namespace opentuner.MediaSources.Longmynd
{

    public partial class LongmyndSource
    {
        private IMqttClient _mqtt_client;

        public void MqttSetFrequency(uint frequency, uint symbol_rate) 
        {
        }

        public void ConnectMqtt()
        {
            string _broker = _settings.LongmyndMqttHost;
            int _broker_port = _settings.LongmyndMqttPort;
            string _clientid = "OTLM" + Guid.NewGuid().ToString();

            // client factory
            var factory = new MqttFactory();

            // client instance
            _mqtt_client = factory.CreateMqttClient();

            // client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_broker, _broker_port)
                .WithClientId(_clientid)
                .WithCleanSession()
                .Build();

            _mqtt_client.ConnectedAsync += _mqtt_client_ConnectedAsync;
            _mqtt_client.DisconnectedAsync += _mqtt_client_DisconnectedAsync; ;
            _mqtt_client.ApplicationMessageReceivedAsync += _mqtt_client_ApplicationMessageReceivedAsync;

            var connectResult = _mqtt_client.ConnectAsync(options);
        }

        private Task _mqtt_client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            HandleMqttMessage(arg.ApplicationMessage.Topic, arg.ApplicationMessage.ConvertPayloadToString());           

            return Task.CompletedTask;
        }



        double mqtt_mer = 0.0;

        private void HandleMqttMessage(string Topic, string Message)
        {
            int new_demodstate = demodState;

            //Console.WriteLine(Topic);
            switch(Topic)
            {
                case "dt/longmynd/rx_state":

                    switch (Message)
                    {
                        case "Init":
                            new_demodstate = 0;
                            break;
                        case "Hunting":
                            new_demodstate = 1;
                            break;
                        case "found header":
                            new_demodstate = 2;
                            break;
                        case "demod_s":
                            new_demodstate = 3;
                            break;
                        case "demod_s2":
                            new_demodstate = 4;
                            break;
                    }
                    break;

                case "dt/longmynd/symbolrate":
                    _tuner1_properties.UpdateValue("symbol_rate", Message);
                    break;
                case "dt/longmynd/ber":
                    _tuner1_properties.UpdateValue("ber", Message);
                    break;
                case "dt/longmynd/mer":
                    _tuner1_properties.UpdateValue("mer", Message);
                    double.TryParse(Message, out mqtt_mer);
                    break;
            }

            if (new_demodstate != demodState)
            {
                if (new_demodstate < 3)
                {
                    Console.WriteLine("Stopping");
                    VideoChangeCB?.Invoke(1, false);
                    playing = false;
                }
                else
                {
                    Console.WriteLine("Playing");
                    VideoChangeCB?.Invoke(1, true);
                    playing = true;
                }

                demodState = new_demodstate;
                _tuner1_properties.UpdateValue("demodstate", demod_state_lookup[demodState]);

                if (demodState < 3)
                    _tuner1_properties.UpdateColor("demodstate", Color.PaleVioletRed);
                else
                    _tuner1_properties.UpdateColor("demodstate", Color.PaleGreen);


            }


        }

        private Task _mqtt_client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("Longmynd mqtt disconnected");

            return Task.CompletedTask;
        }

        private async Task _mqtt_client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("Longmynd mqtt connected");

            await _mqtt_client.SubscribeAsync("dt/longmynd/#");

            return;
        }
    }
}

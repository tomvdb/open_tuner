using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Vortice.XAudio2;
using System.Drawing;
using System.Windows.Media.Animation;

namespace opentuner.MediaSources.Longmynd
{

    public partial class LongmyndSource
    {
        private IMqttClient _mqtt_client;

       

        public void SendMqttStatus(string topic, string value)
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(value)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

            Task.Run(async () =>
            {
                await _mqtt_client.PublishAsync(message);
            });

        }

        public void MqttSetTS(string ip, int port)
        {
            Console.WriteLine(ip.ToString() + " - " + port.ToString());
            Console.WriteLine("ip set: " + _settings.CmdTopic + "ip");
            Console.WriteLine("port set: " + _settings.CmdTopic + "tsip");
            Console.WriteLine("port is ignored, should be 1234 on mqtt version, unless you have a custom setup");
            SendMqttStatus(_settings.CmdTopic + "tsip", ip);
        }

        public void MqttSetFrequency(uint frequency, uint symbol_rate) 
        {
            Console.WriteLine(frequency.ToString() + " - " + symbol_rate.ToString());
            Console.WriteLine("freq set: " + _settings.CmdTopic + "frequency");
            Console.WriteLine("sr set: " + _settings.CmdTopic + "sr");
            SendMqttStatus(_settings.CmdTopic + "frequency", (frequency - _settings.Offset1).ToString());
            SendMqttStatus(_settings.CmdTopic + "sr", (symbol_rate).ToString());
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
        string mod = "";
        string fec = "";

        private void HandleMqttMessage(string Topic, string Message)
        {
            int new_demodstate = demodState;

            if (Message == null)
            {
                //Console.WriteLine("null message value:" + Topic);
                return;
            }

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
                case "dt/longmynd/service_name":
                    _tuner1_properties.UpdateValue("service_name", Message);
                    break;
                case "dt/longmynd/provider_name":
                    _tuner1_properties.UpdateValue("service_name_provider", Message);
                    break;
                case "dt/longmynd/ts_null":
                    _tuner1_properties.UpdateValue("null_packets", Message);
                    break;
                case "dt/longmynd/set/swport":
                    _tuner1_properties.UpdateValue("rf_input", ( Message == "0" ? "A" : "B"));
                    break;
                case "dt/longmynd/set/tsip":
                    _source_properties.UpdateValue("source_ts_ip", Message);

                    if (_LocalIp != Message)
                    {
                        _source_properties.UpdateColor("source_ts_ip", Color.PaleVioletRed);
                    }
                    else
                    {
                        _source_properties.UpdateColor("source_ts_ip", Color.Bisque);
                    }
                    break;
                case "dt/longmynd/matype1":
                    _tuner1_properties.UpdateValue("stream_format", Message);
                    break;
                case "dt/longmynd/modulation":
                    mod = Message;
                    _tuner1_properties.UpdateValue("modcod", mod + " " + fec);
                    break;
                case "dt/longmynd/fec":
                    fec = Message;
                    _tuner1_properties.UpdateValue("modcod", mod + " " + fec);
                    break;
                case "dt/longmynd/carrier_frequency":
                    if ( uint.TryParse(Message, out current_frequency_1))
                    {
                        _tuner1_properties.UpdateValue("requested_freq", "(" + GetFrequency(0, true).ToString("N0") + ") (" + GetFrequency(0, false).ToString("N0") + ")");
                    }
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

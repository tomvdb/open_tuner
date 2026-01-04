using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using opentuner.MediaSources;
using opentuner.Utilities;
using Serilog;

namespace opentuner.ExtraFeatures.MqttClient
{
    public delegate void NewMqttMessage(MqttMessage Message);
    
    public class MqttManager
    {
        private string _broker;
        private int _broker_port;
        private string _clientid;
        private string _maintopic = "dt/opentuner/";
        private string _cmdtopic = "cmd/opentuner/";

        private IMqttClient _mqtt_client;

        public event NewMqttMessage OnMqttMessageReceived;

        private MqttManagerSettings _settings;
        private SettingsManager<MqttManagerSettings> _settingsManager;

        public MqttManager() 
        {
            _settings = new MqttManagerSettings();
            _settingsManager = new SettingsManager<MqttManagerSettings>("mqttclient_settings");

            _broker = _settings.MqttBroker;
            _broker_port = _settings.MqttPort;

            _clientid = "OT" + Guid.NewGuid().ToString();

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
            _mqtt_client.DisconnectedAsync += _mqtt_client_DisconnectedAsync;
            _mqtt_client.ApplicationMessageReceivedAsync += _mqtt_client_ApplicationMessageReceivedAsync;

            var connectResult =  _mqtt_client.ConnectAsync(options);
        }

        public void Disconnect()
        {
            try
            {
                _mqtt_client.DisconnectAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public void SendProperties(OTSourceData properties, string ChildTopic)
        {
            SendMqttStatus(ChildTopic + "/demod_locked", properties.demod_locked.ToString());
            SendMqttStatus(ChildTopic + "/frequency", properties.frequency.ToString());
            SendMqttStatus(ChildTopic + "/symbol_rate", properties.symbol_rate.ToString());
            SendMqttStatus(ChildTopic + "/service_name", properties.service_name.ToString());
            SendMqttStatus(ChildTopic + "/mer", properties.mer.ToString());
            SendMqttStatus(ChildTopic + "/db_margin", properties.db_margin.ToString());
        }

        public void SendMqttStatus(string topic, string value)
        {
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(_maintopic + topic)
                .WithPayload(value)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

                Task.Run(async () =>
                {
                    await _mqtt_client.PublishAsync(message);
                });
        }

        // this requires a full topic - currently only used for pluto commands
        public async Task SendMqttCommand(string topic, string value)
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(value)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

            await _mqtt_client.PublishAsync(message);

            return;
        }


        private Task _mqtt_client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            OnMqttMessageReceived?.Invoke(new MqttMessage(arg.ApplicationMessage.Topic, arg.ApplicationMessage.ConvertPayloadToString()));
            return Task.CompletedTask;
        }

        private Task _mqtt_client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Log.Information("Mqtt Disconnected");
            return Task.CompletedTask;
        }

        private async Task _mqtt_client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Log.Information("Mqtt Connected");

            // subscribe to mqtt commands
            await _mqtt_client.SubscribeAsync(_cmdtopic + "tuner1/#");
            await _mqtt_client.SubscribeAsync(_cmdtopic + "tuner2/#");

            // subscribe to f5oeoe firmware topics (if available)
            // await _mqtt_client.SubscribeAsync("dt/pluto/#");

            return;
        }
    }
}

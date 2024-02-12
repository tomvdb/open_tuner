using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace opentuner.ExtraFeatures.MqttClient
{
    public delegate void NewMqttMessage(MqttMessage Message);
    
    public class OTMqttClient
    {
        private string _broker;
        private int _broker_port;
        private string _clientid;
        private string _maintopic = "dt/opentuner/";
        private string _cmdtopic = "cmd/opentuner/";

        private IMqttClient _mqtt_client;

        public event NewMqttMessage OnMqttMessageReceived;

        public OTMqttClient(string BrokerHost, int BrokerPort, string ParentTopic) 
        { 
            _broker = BrokerHost;
            _broker_port = BrokerPort;
            _maintopic = ParentTopic;

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
            catch (Exception ex) { }
        }

        public async Task UpdateTunerStatus(TunerStatus tunerStatus)
        {
            await SendMqttStatus("ldpc_errors", tunerStatus.errors_ldpc_count.ToString());

            await SendMqttStatus("tuner1/rx_state", lookups.demod_state_lookup[tunerStatus.T1P2_demod_status]);
            await SendMqttStatus("tuner2/rx_state", lookups.demod_state_lookup[tunerStatus.T2P1_demod_status]);
            await SendMqttStatus("tuner1/lna_gain", tunerStatus.T1P2_lna_gain.ToString());
            await SendMqttStatus("tuner2/lna_gain", tunerStatus.T2P1_lna_gain.ToString());
            await SendMqttStatus("tuner1/agc1", tunerStatus.T1P2_agc1_gain.ToString());
            await SendMqttStatus("tuner1/agc2", tunerStatus.T1P2_agc2_gain.ToString());
            await SendMqttStatus("tuner2/agc1", tunerStatus.T1P2_agc1_gain.ToString());
            await SendMqttStatus("tuner2/agc2", tunerStatus.T1P2_agc2_gain.ToString());
            await SendMqttStatus("tuner1/poweri", tunerStatus.T1P2_power_i.ToString());
            await SendMqttStatus("tuner1/powerq", tunerStatus.T1P2_power_q.ToString());
            await SendMqttStatus("tuner2/poweri", tunerStatus.T2P1_power_i.ToString());
            await SendMqttStatus("tuner2/powerq", tunerStatus.T2P1_power_q.ToString());
            await SendMqttStatus("tuner1/puncrate", tunerStatus.T1P2_puncture_rate.ToString());
            await SendMqttStatus("tuner2/puncrate", tunerStatus.T2P1_puncture_rate.ToString());

            await SendMqttStatus("tuner1/symbol_rate", tunerStatus.T1P2_symbol_rate.ToString());
            await SendMqttStatus("tuner2/symbol_rate", tunerStatus.T2P1_symbol_rate.ToString());
            await SendMqttStatus("tuner1/carrier_frequency", (tunerStatus.T1P2_requested_frequency + tunerStatus.T1P2_frequency_carrier_offset).ToString());
            await SendMqttStatus("tuner2/carrier_frequency", (tunerStatus.T2P1_requested_frequency + tunerStatus.T2P1_frequency_carrier_offset).ToString());

            await SendMqttStatus("tuner1/viterbi_error", tunerStatus.T1P2_viterbi_error_rate.ToString());
            await SendMqttStatus("tuner2/viterbi_error", tunerStatus.T2P1_viterbi_error_rate.ToString());
            await SendMqttStatus("tuner1/ber", tunerStatus.T1P2_ber.ToString());
            await SendMqttStatus("tuner2/ber", tunerStatus.T2P1_ber.ToString());
            await SendMqttStatus("tuner1/mer", ((double)(tunerStatus.T1P2_mer) / 10).ToString());
            await SendMqttStatus("tuner2/mer", ((double)(tunerStatus.T2P1_mer) / 10).ToString());
            await SendMqttStatus("tuner1/bch_uncorrected", (tunerStatus.T1P2_errors_bch_uncorrected ? 1 : 0).ToString());
            await SendMqttStatus("tuner2/bch_uncorrected", tunerStatus.T2P1_errors_bch_uncorrected.ToString());
            await SendMqttStatus("tuner1/bch_errors", tunerStatus.T1P2_errors_bch_count.ToString());
            await SendMqttStatus("tuner2/bch_errors", tunerStatus.T2P1_errors_bch_count.ToString());

            await SendMqttStatus("tuner1/matype1", lookups.stream_format_lookups[(int)tunerStatus.T1P2_stream_format].ToString());
            await SendMqttStatus("tuner2/matype2", lookups.stream_format_lookups[(int)tunerStatus.T2P1_stream_format].ToString());

            double dbmargin1 = 0;
            string modcod1 = "";

            double dbmargin2 = 0;
            string modcod2 = "";

            try
            {
                switch (tunerStatus.T1P2_demod_status)
                {
                    case 2:
                        modcod1 = lookups.modcod_lookup_dvbs2[tunerStatus.T1P2_modcode];
                        dbmargin1 = (((double)(tunerStatus.T1P2_mer) / 10) - lookups.modcod_lookup_dvbs2_threshold[tunerStatus.T1P2_modcode]);
                        break;
                    case 3:
                        modcod1 = lookups.modcod_lookup_dvbs[tunerStatus.T1P2_modcode];
                        dbmargin1 = (((double)(tunerStatus.T1P2_mer) / 10) - lookups.modcod_lookup_dvbs_threshold[tunerStatus.T1P2_modcode]);
                        break;
                    default:
                        dbmargin1 = 0;
                        modcod1 = "unknown unknown";
                        break;
                }
            }
            catch (Exception Ex)
            {
                dbmargin1 = 0;
                modcod1 = "unknown unknown";
            }

            try
            {
                switch (tunerStatus.T2P1_demod_status)
                {
                    case 2:
                        modcod2 = lookups.modcod_lookup_dvbs2[tunerStatus.T2P1_modcode];
                        dbmargin2 = (((double)(tunerStatus.T2P1_mer) / 10) - lookups.modcod_lookup_dvbs2_threshold[tunerStatus.T2P1_modcode]);
                        break;
                    case 3:
                        modcod2 = lookups.modcod_lookup_dvbs[tunerStatus.T2P1_modcode];
                        dbmargin2 = (((double)(tunerStatus.T2P1_mer) / 10) - lookups.modcod_lookup_dvbs_threshold[tunerStatus.T2P1_modcode]);
                        break;
                    default:
                        dbmargin2 = 0;
                        modcod2 = "unknown unknown";
                        break;
                }
            }
            catch (Exception Ex)
            {
                dbmargin2 = 0;
                modcod2 = "unknown unknown";
            }

            await SendMqttStatus("tuner1/margin_db", dbmargin1.ToString());
            await SendMqttStatus("tuner2/margin_db", dbmargin2.ToString());
            await SendMqttStatus("tuner1/modulation", modcod1.Split(' ')[0].ToString());
            await SendMqttStatus("tuner2/modulation", modcod2.Split(' ')[0].ToString());
            await SendMqttStatus("tuner1/fec", modcod1.Split(' ')[1].ToString());
            await SendMqttStatus("tuner2/fec", modcod2.Split(' ')[1].ToString());

            await SendMqttStatus("tuner1/rolloff", lookups.rolloff_lookups[tunerStatus.T1P2_rolloff].ToString());
            await SendMqttStatus("tuner2/rolloff", lookups.rolloff_lookups[tunerStatus.T2P1_rolloff].ToString());

            await SendMqttStatus("tuner1/pilots", (tunerStatus.T1P2_pilots ? 1 : 0).ToString());
            await SendMqttStatus("tuner2/pilots", (tunerStatus.T2P1_pilots ? 1 : 0).ToString());

            await SendMqttStatus("tuner1/short_frame", (tunerStatus.T1P2_short_frame ? 1 : 0).ToString());
            await SendMqttStatus("tuner2/short_frame", (tunerStatus.T2P1_short_frame ? 1 : 0).ToString());

            return;
        }

        public async Task UpdateTSStatus(int device, TSStatus tsStatus)
        {
            string tuner = "tuner" + device.ToString() + "/";

            await SendMqttStatus(tuner + "service_name", tsStatus.ServiceName);
            await SendMqttStatus(tuner + "provider_name", tsStatus.ServiceProvider);
            await SendMqttStatus(tuner + "ts_null", tsStatus.NullPacketsPerc.ToString());

            return;
        }

        public async Task SendMqttStatus(string topic, string value)
        {
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(_maintopic + topic)
                .WithPayload(value)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

                await _mqtt_client.PublishAsync(message);

            return;
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
            Console.WriteLine("Mqtt Disconnected");
            return Task.CompletedTask;
        }

        private async Task _mqtt_client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("Mqtt Connected");

            // subscribe to mqtt commands
            await _mqtt_client.SubscribeAsync(_cmdtopic + "tuner1/#");
            await _mqtt_client.SubscribeAsync(_cmdtopic + "tuner2/#");

            // subscribe to f5oeoe firmware topics (if available)
            await _mqtt_client.SubscribeAsync("dt/pluto/#");

            return;
        }
    }
}

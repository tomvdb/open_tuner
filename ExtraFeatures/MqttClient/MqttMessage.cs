using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.MqttClient
{
    // simple class for mqttmessages
    public class MqttMessage
    {
        public string Topic { get; set; }
        public string Message { get; set; }

        public MqttMessage(string Topic, string Message) 
        { 
            this.Topic = Topic;
            this.Message = Message;
        }

        public override string ToString() 
        {
            return "Mqtt: " + Topic + " = " + Message;
        }
    }
}

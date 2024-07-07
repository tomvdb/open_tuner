using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.MediaFoundation;
using WebSocketSharp;
using System.Timers;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource
    {
        // ws interface
        private WebSocket controlWS;        // longmynd control ws websocket
        private WebSocket monitorWS;        // longmynd monitor ws websocket

        private void WSSetFrequency(int device, int freq, int sr)
        {           
            controlWS.Send("F" + (device + 1).ToString() + "," + freq + "," + sr.ToString() + "," + _current_offset[device]);
        }

        private void WSSetTS(int device)
        {
            string wh_command = ("U" + (device + 1).ToString() + "," + _LocalIp.ToString());
            Log.Debug(wh_command);
            controlWS.Send(wh_command);
        }

        private void connectWebsockets()
        {

            string url = "ws://" + _settings.WinterhillWSHost + ":" + _settings.WinterhillWSPort.ToString() + "/ ";

            monitorWS = new WebSocket(url, "monitor");
            monitorWS.OnOpen += Monitorws_OnOpen;
            monitorWS.OnMessage += Monitorws_OnMessage;
            monitorWS.OnClose += Monitorws_OnClose;
            monitorWS.OnError += MonitorWS_OnError;
            monitorWS.ConnectAsync();

            controlWS = new WebSocket(url, "control");
            controlWS.OnClose += Controlws_OnClose;
            controlWS.OnMessage += Controlws_OnMessage;
            controlWS.OnOpen += Controlws_OnOpen;
            controlWS.OnError += ControlWS_OnError;
            controlWS.ConnectAsync();

        }

        private void ControlWS_OnError(object sender, ErrorEventArgs e)
        {
            debug("ControlWS_OnError: " + e.ToString());
        }

        private void MonitorWS_OnError(object sender, ErrorEventArgs e)
        {
            debug("MonitorWS_OnError: " + e.ToString());
        }

        private void OnTimeoutEvent(object sender, ElapsedEventArgs e)
        {
            debug("Monitor WS Timeout");
            WebSocketTimeout();
            if (monitorWS.IsAlive)
            {
                debug("Closing Monitor WS");
                monitorWS.Close();
            }
            _connected = false;
            if (controlWS.IsAlive)
            {
                debug("Closing Control WS");
                controlWS.Close();
            }
        }

        private void Monitorws_OnOpen(object sender, EventArgs e)
        {
            debug("Success: Monitor WS Open");
            _connected = true;
        }



        public void debug(string msg)
        {
            Log.Information(msg);
        }

        private void Controlws_OnOpen(object sender, EventArgs e)
        {
            debug("Success: Control WS Open");
        }


        private void Controlws_OnMessage(object sender, MessageEventArgs e)
        {
        }

        private void Controlws_OnClose(object sender, CloseEventArgs e)
        {
            debug("Error: Control WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            controlWS.ConnectAsync();
        }

        private void Monitorws_OnClose(object sender, CloseEventArgs e)
        {
            debug("Error: Monitor WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            monitorWS.ConnectAsync();
        }

        private void Monitorws_OnMessage(object sender, MessageEventArgs e)
        {
            monitorMessage mm = JsonConvert.DeserializeObject<monitorMessage>(e.Data);
            UpdateInfo(mm);
        }

        public void DisconnectWebsockets()
        {
            _connected = false;
            if (monitorWS.IsAlive)
                monitorWS.Close();
            if (controlWS.IsAlive)
                controlWS.Close();
        }
    }

    [Serializable]
    public class ReceiverMessage
    {
        public int rx;
        public int scanstate;
        public string service_name;
        public string service_provider_name;
        public string mer;
        public string dbmargin;
        public string frequency;
        public string symbol_rate;
        public string modcod;
        public string null_percentage;
        public string video_type;
        public string audio_type;
        public string ts_addr;
        public string ts_port;
        public int rfport;
    }


    [Serializable]
    public class monitorMessage
    {
        public string type;
        public double timestamp;
        public ReceiverMessage[] rx;
    }
}

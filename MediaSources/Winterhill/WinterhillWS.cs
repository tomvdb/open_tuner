using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.MediaFoundation;
using WebSocketSharp;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource
    {
        // ws interface
        private WebSocket controlWS;        // longmynd control ws websocket
        private WebSocket monitorWS;        // longmynd monitor ws websocket

        private bool _closing = false;      // stop retrying connect

        /*
        private void WSSetFrequency(uint frequency, uint symbol_rate)
        {
            controlWS.Send("C" + (frequency - _settings.Offset1).ToString() + "," + symbol_rate.ToString());
        }
        */

        private void WSSetFrequency(int device, int freq, int sr)
        {           
            controlWS.Send("F" + (device + 1).ToString() + "," + freq + "," + sr.ToString() + "," + _settings.Offset[device]);
        }

        private void connectWebsockets()
        {

            string url = "ws://" + _settings.WinterhillWSHost + ":" + _settings.WinterhillWSPort.ToString() + "/ ";

            monitorWS = new WebSocket(url, "monitor");
            monitorWS.OnOpen += Monitorws_OnOpen;
            monitorWS.OnMessage += Monitorws_OnMessage;
            monitorWS.OnClose += Monitorws_OnClose;
            monitorWS.ConnectAsync();

            controlWS = new WebSocket(url, "control");
            controlWS.OnClose += Controlws_OnClose;
            controlWS.OnMessage += Controlws_OnMessage;
            controlWS.OnOpen += Controlws_OnOpen;
            controlWS.ConnectAsync();
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
            if (_closing == true)
                return;
            debug("Error: Control WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            controlWS.ConnectAsync();
        }

        private void Monitorws_OnClose(object sender, CloseEventArgs e)
        {
            if (_closing == true)
                return;
            debug("Error: Monitor WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            monitorWS.ConnectAsync();
        }

        private void Monitorws_OnMessage(object sender, MessageEventArgs e)
        {
            monitorMessage mm = JsonConvert.DeserializeObject<monitorMessage>(e.Data);
            UpdateInfo(mm);
        }

        public void Disconnect()
        {
            _connected = false;
            _closing = true;
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
    }


    [Serializable]
    public class monitorMessage
    {
        public string type;
        public double timestamp;
        public ReceiverMessage[] rx;
    }
}

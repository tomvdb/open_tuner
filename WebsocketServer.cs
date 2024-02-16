using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace opentuner
{
    public class WSClientConnection : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //Send(e.Data);

        }

        protected override void OnClose(CloseEventArgs e)
        {
            //base.OnClose(e);
            Console.WriteLine("Ws closed: " + e.ToString());
        }

        protected override void OnOpen()
        {
            //base.OnOpen();
            Console.WriteLine("Ws open: " + this.ID);
        }

    }
    public class OTWebsocketServer
    {
        WebSocketServer ws_server;

        public OTWebsocketServer(int port) 
        { 
            ws_server = new WebSocketServer(port);
            ws_server.AddWebSocketService<WSClientConnection>("/");
            ws_server.Start();
        }

        public void Broadcast(string msg)
        {
            if (ws_server != null)
            {
                ws_server.WebSocketServices.Broadcast(msg);
            }
        }
    }

    [Serializable]
    public class monitorMessage_packet
    {
        public monitorMessage_packet_rx rx;
        public monitorMessage_packet_ts ts;
    }


    [Serializable]
    public class monitorMessage_packet_rx
    {
        public int rfport;
        public int demod_state;
        public int ber;
        public int frequency;
        public int symbolrate;
        public int mer;
        public int modcod;
        public string ts_ip_addr;
        public int ts_ip_port;
    }

    [Serializable]
    public class monitorMessage_packet_ts
    {
        public string service_name;
        public string service_provider_name;
        public int null_ratio;
        public int[][] PIDs;
    }

    [Serializable]
    public class monitorMessage
    {
        public string type;
        public double timestamp;
        public monitorMessage_packet packet;
    }
}

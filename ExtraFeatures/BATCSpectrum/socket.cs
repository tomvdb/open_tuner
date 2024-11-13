// From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace opentuner
{
    class socket
    {
        private const int padding = 8;  // useless bytes to remove at the end of the socket data package

        public Action<ushort[]> callback;

        private WebSocket ws;       //websocket client

        private ushort[] fft_data;

        public bool connected;

        public DateTime lastdata;

        public event EventHandler<bool> ConnectionStatusChanged;

        public socket()
        {
            connected = false;
        }

        public void start()
        {
            if (!connected)
            {
                Log.Information("Websocket: QO_Spectrum: Try connect..\n");

                ws = new WebSocket("wss://eshail.batc.org.uk/wb/fft", "fft_m0dtslivetune");

                ws.OnMessage += (ss, ee) => NewData(ee.RawData);
                ws.OnOpen += Ws_OnOpen;
                ws.OnClose += Ws_OnClose; 
                ws.OnError += Ws_OnError;

                ws.ConnectAsync();
            }
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            connected = false;
            Log.Information("Websocket: QO_Spectrum: Connection Closed");

            ConnectionStatusChanged?.Invoke(this, connected);
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            connected = true; 
            Log.Information("Websocket: QO_Spectrum: Connected.\n");

            ConnectionStatusChanged?.Invoke(this, connected);
            lastdata = DateTime.Now;

        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Log.Information("Websocket: QO_Spectrum: Error" + e.ToString());
        }

        public void stop()
        {
            if (connected)
            {
                ws?.Close();
                connected = false;
            }

        }

        private void NewData(byte[] data)
        {
            lastdata = DateTime.Now;

            int data_length = data.Length - padding;    // data length to process
            fft_data = new UInt16[data_length / 2];

            //unpack bytes to unsigned short int values
            int n = 0;
            byte[] buf = new byte[2];

            for (int i = 0; i < data_length; i += 2)
            {
                buf[0] = data[i];
                buf[1] = data[i + 1];
                fft_data[n] = BitConverter.ToUInt16(buf, 0);
                n++;
            }
            callback(fft_data);
            //Log.Information(".");

        }

    }
}

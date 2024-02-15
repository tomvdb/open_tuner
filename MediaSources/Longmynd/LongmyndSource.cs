using Newtonsoft.Json;
using opentuner.MediaPlayers;
using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;
using WebSocketSharp;

namespace opentuner.MediaSources.Longmynd
{
    public partial class LongmyndSource : OTSource
    {
        private bool _connected = false;

        private WebSocket controlWS;        // longmynd control ws websocket
        private WebSocket monitorWS;        // longmynd monitor ws websocket

        private System.Timers.Timer sessionTimer;

        private LongmyndSettings _settings = new LongmyndSettings();

        private int demodState = -1;

        private UDPClient udp_client;

        private CircularBuffer udp_buffer = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        bool playing = false;

        private VideoChangeCallback VideoChangeCB;

        Thread ts_thread_t = null;
        TSThread ts_thread;

        public CircularBuffer ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        private OTMediaPlayer _media_player;

        public override bool DeviceConnected => _connected;

        public override void Close()
        {
            
        }

        public override void ConfigureVideoPlayers(List<OTMediaPlayer> MediaPlayers)
        {
            _media_player = MediaPlayers[0];
        }

        public override string GetDescription()
        {
            return "Longmynd Client";
        }

        public override string GetDeviceName()
        {
            return "Longmynd Client Device";
        }

        public override long GetFrequency(int device, bool offset_included)
        {
            return 0;
        }

        public override string GetName()
        {
            return "Longmynd";
        }

        public override CircularBuffer GetVideoDataQueue(int device)
        {
            return ts_data_queue;
        }

        public override int GetVideoSourceCount()
        {
            return 1;
        }

        private void Udp_client_DataReceived(object sender, byte[] e)
        {
            if (!playing) { return; }

            for (int c = 0; c < e.Length; c++)
            {
                udp_buffer.Enqueue(e[c]);
            }
        }

        private void Udp_client_ConnectionStatusChanged(object sender, string e)
        {
            debug("udp: Connection status changed: " + e);
        }


        private void connectWebsockets()
        {            

            string url = "ws://" + _settings.LongmyndHost + ":" + _settings.LongmyndWS.ToString() + "/ ";

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
        }


        private void Controlws_OnOpen(object sender, EventArgs e)
        {
            debug("Success: Control WS Open");

            /*
            debug("Attempt to change longmynd TS to " + localip + ":" + longmyndTSPort.ToString());

            if (configureIP)
            {
                debug("Configuring IP:");

                if (configureIP_ip.Length > 0)
                {
                    debug("Configuring as " + configureIP_ip + ":" + longmyndTSPort.ToString());
                    controlWS.Send("U" + configureIP_ip + ":" + longmyndTSPort.ToString());
                }
                else
                {
                    debug("Configuring as " + localip + ":" + longmyndTSPort.ToString());
                    controlWS.Send("U" + localip + ":" + longmyndTSPort.ToString());
                    configureIP_ip = localip;
                }
            }

            checkPowerLNB.Checked = lnbPowerEnabled;
            checkHoriz.Checked = lnbHorizPol;

            setRFPort(defaultTuner);

            if (defaultTuner == 0)
                radioTunerPortA.Checked = true;
            else
                radioTunerPortB.Checked = true;
            */
        }

        public void debug(string msg)
        {
            Console.WriteLine(msg);
        }

        private void Controlws_OnMessage(object sender, MessageEventArgs e)
        {
            //debug("Control WS Message" + e.Data);
        }

        private void Controlws_OnClose(object sender, CloseEventArgs e)
        {
            debug("Error: Control WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            controlWS.Connect();
        }

        private void Monitorws_OnClose(object sender, CloseEventArgs e)
        {
            debug("Error: Monitor WS Closed - Check WS IP");
            debug("Attempting to reconnect...");
            monitorWS.Connect();
        }

        private void Monitorws_OnMessage(object sender, MessageEventArgs e)
        {
            monitorMessage mm = JsonConvert.DeserializeObject<monitorMessage>(e.Data);


            if (demodState != mm.packet.rx.demod_state)
            {
                if (mm.packet.rx.demod_state < 3)
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

                demodState = mm.packet.rx.demod_state;
            }

        }

        public override int Initialize(VideoChangeCallback VideoChangeCB, Control Parent)
        {
            _parent = Parent;

            // connect websockets
            connectWebsockets();

            // open udp port
            udp_client = new UDPClient(4003);
            udp_client.ConnectionStatusChanged += Udp_client_ConnectionStatusChanged;
            udp_client.DataReceived += Udp_client_DataReceived;
            udp_client.Connect();

            ts_thread = new TSThread(ts_data_queue, FlushTS2, ReadTS2, "LM TS");
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            BuildSourceProperties();

            this.VideoChangeCB = VideoChangeCB;

            return 1;
        }

        void FlushTS2()
        {
            udp_buffer.Clear();
        }

        byte ReadTS2(ref byte[] data, ref uint dataRead)
        {
            int read = udp_buffer.Count;
            uint written = 0;

            if (udp_buffer.Count > 4000) 
            {
                read = 4000;
            }

            for (int c = 0; c < read; c++)
            {
                data[c] = udp_buffer.Dequeue();
                written += 1;
            }

            dataRead = written;

            return 0;
        }


        public override void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue)
        {
        }


        public override void SetFrequency(int device, uint frequency, uint symbol_rate, bool offset_included)
        {

            controlWS.Send("C" + (frequency - _settings.Offset).ToString() + "," + symbol_rate.ToString());
        }

        public override void ShowSettings()
        {
            
        }

        public override void StartStreaming(int device)
        {
            if (ts_thread != null)
                ts_thread.start_ts();
        }

        public override void StopStreaming(int device)
        {
            if (ts_thread != null)
                ts_thread.stop_ts();
        }
    }
}

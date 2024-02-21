using opentuner.MediaPlayers;
using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource : OTSource
    {
        private WinterhillSettings _settings;
        private SettingsManager<WinterhillSettings> _settingsManager;

        public override event SourceDataChange OnSourceData;

        private bool _connected = false;
        public override bool DeviceConnected => _connected;

        private VideoChangeCallback VideoChangeCB;

        Thread[] ts_thread_t = null;
        TSThread[] ts_threads;

        // todo: fix double buffer read
        private CircularBuffer[] udp_buffer; // = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);
        public CircularBuffer[] ts_data_queue; // = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        private OTMediaPlayer[] _media_player = new OTMediaPlayer[4];
        private TSRecorder[] _recorders = new TSRecorder[4];
        private TSUdpStreamer[] _streamer = new TSUdpStreamer[4];

        private int ts_devices = 4;

        UDPClient[] udp_clients;

        private string _MediaPath;

        private int[] _current_frequency = new int[4] {0, 0, 0, 0};

        private int[] demodstate = new int[4] {0, 0, 0, 0};

        private bool[] playing = new bool[4] {false, false, false, false};

        bool _videoPlayersReady = false;

        public WinterhillSource()
        {
            // settings
            _settings = new WinterhillSettings();
            _settingsManager = new SettingsManager<WinterhillSettings>("winterhill_settings");
        }

        public override int Initialize(VideoChangeCallback VideoChangeCB, Control Parent)
        {
            _parent = Parent;

            // connect interface
            switch(_settings.DefaultInterface)
            {
                case 1: // websockets
                    connectWebsockets();
                    break;
                case 2: // udp
                    break;
            }

            // open udp ts ports
            udp_clients = new UDPClient[4];
            ts_threads = new TSThread[4];
            ts_thread_t = new Thread[4];
            ts_data_queue = new CircularBuffer[4];
            udp_buffer = new CircularBuffer[4];

            for (int c = 0; c < ts_devices; c++)
            {
                int port = _settings.WinterhillUdpBasePort + 41 + c;

                Console.WriteLine("UDP Port: " + port.ToString());

                udp_buffer[c] = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);
                ts_data_queue[c] = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

                udp_clients[c] = new UDPClient(port, c);
                udp_clients[c].ConnectionStatusChanged += WinterhillSource_ConnectionStatusChanged;
                udp_clients[c].DataReceived += WinterhillSource_DataReceived;
                udp_clients[c].Connect();

                FlushTS flush_ts = null;
                ReadTS read_ts = null;

                switch(c)
                {
                    case 0: flush_ts = FlushTS0; read_ts = ReadTS0; break;
                    case 1: flush_ts = FlushTS1; read_ts = ReadTS1; break;
                    case 2: flush_ts = FlushTS2; read_ts = ReadTS2; break;
                    case 3: flush_ts = FlushTS3; read_ts = ReadTS3; break;
                }


                ts_threads[c] = new TSThread(ts_data_queue[c], flush_ts, read_ts, "WS TS" + c.ToString());
                ts_thread_t[c] = new Thread(ts_threads[c].worker_thread);
                ts_thread_t[c].Start();

            }

            BuildSourceProperties();

            this.VideoChangeCB = VideoChangeCB;

            return ts_devices;
        }

        public void FlushTS0()
        {
            udp_buffer[0].Clear();
        }
        public void FlushTS1()
        {
            udp_buffer[1].Clear();
        }
        public void FlushTS2()
        {
            udp_buffer[2].Clear();
        }
        public void FlushTS3()
        {
            udp_buffer[3].Clear();
        }

        byte ReadTSGeneric(int id, ref byte[] data, ref uint dataRead)
        {
            int read = udp_buffer[id].Count;
            uint written = 0;

            if (udp_buffer[id].Count > 4000)
            {
                read = 4000;
            }

            for (int c = 0; c < read; c++)
            {
                data[c] = udp_buffer[id].Dequeue();
                written += 1;
            }

            dataRead = written;

            return 0;
        }

        byte ReadTS0(ref byte[] data, ref uint dataRead)
        {
            return ReadTSGeneric(0, ref data, ref dataRead);
        }

        byte ReadTS1(ref byte[] data, ref uint dataRead)
        {
            return ReadTSGeneric(1, ref data, ref dataRead);
        }

        byte ReadTS2(ref byte[] data, ref uint dataRead)
        {
            return ReadTSGeneric(2, ref data, ref dataRead);
        }

        byte ReadTS3(ref byte[] data, ref uint dataRead)
        {
            return ReadTSGeneric(3, ref data, ref dataRead);
        }


        private void WinterhillSource_DataReceived(object sender, byte[] e)
        {
            int device = ((UDPClient)sender).getID();

            if (!playing[device]) return;

            for (int c = 0; c < e.Length; c++)
            {
                udp_buffer[device].Enqueue(e[c]);
            }
        }

        private void WinterhillSource_ConnectionStatusChanged(object sender, string e)
        {
            Console.WriteLine("Connection Status " + ((UDPClient)sender).getID() + " : " + e);
        }

        public override void Close()
        {
            // todo: close various objects
        }

        public override void ConfigureMediaPath(string MediaPath)
        {
            _MediaPath = MediaPath;
        }

        public override void ConfigureTSRecorders(List<TSRecorder> TSRecorders)
        {
            for (int c = 0; c  < TSRecorders.Count; c++)
            {
                _recorders[c] = TSRecorders[c];
            }
        }

        public override void ConfigureTSStreamers(List<TSUdpStreamer> TSStreamers)
        {
            for (int c = 0; c < TSStreamers.Count; c++)
            {
                _streamer[c] = TSStreamers[c];
            }
        }

        public override void ConfigureVideoPlayers(List<OTMediaPlayer> MediaPlayers)
        {
            for (int c = 0; c < 4; c++) {
                _media_player[c] = MediaPlayers[c];
                _media_player[c].onVideoOut += WinterhillSource_onVideoOut;
            }

            _videoPlayersReady = true;
        }

        private void WinterhillSource_onVideoOut(object sender, MediaStatus e)
        {
            int video_id = ((OTMediaPlayer)sender).getID();
            // need id's here
            //Console.WriteLine("OnVideoOut: " +  video_id.ToString());

            _media_player[video_id].SetVolume((int)_settings.DefaultVolume[video_id]);

            UpdateMediaProperties(video_id, e);
        }

        public override string GetDescription()
        {
            return "Winterhill";
        }

        public override string GetDeviceName()
        {
            return "Winterhill";
        }

        public override long GetFrequency(int device, bool offset_included)
        {
            return _current_frequency[device] + (offset_included ? _settings.Offset[device] : 0);
        }

        public override string GetName()
        {
            return "Winterhill";
        }

        public override CircularBuffer GetVideoDataQueue(int device)
        {
            return ts_data_queue[device];
        }

        public override int GetVideoSourceCount()
        {
            return ts_devices;
        }

        public override void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue)
        {
            ts_threads[device].RegisterTSConsumer(ts_buffer_queue);
        }

        public override void SetFrequency(int device, uint frequency, uint symbol_rate, bool offset_included)
        {
            WSSetFrequency(device, (int)frequency, (int)symbol_rate);
        }

        public override void ShowSettings()
        {
            WinterhillSettingsForm settingsForm = new WinterhillSettingsForm(_settings);
        }

        public override void StartStreaming(int device)
        {
            if (ts_threads != null)
            {
                if (ts_threads[device] != null)
                {
                    ts_threads[device].start_ts();
                }
            }
        }

        public override void StopStreaming(int device)
        {
            if (ts_threads != null)
            {
                if (ts_threads[device] != null)
                {
                    ts_threads[device].stop_ts();
                }
            }
        }
    }
}

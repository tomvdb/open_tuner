using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.CodeDom;
using opentuner.MediaSources;
using Serilog;

namespace opentuner
{
    public class TSUdpStreamer
    {
        public CircularBuffer ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        object locker = new object();

        protected bool _stream;
        public bool stream
        {
            get
            {
                lock (locker)
                    return _stream;
            }
            set
            {
                lock (locker)
                    _stream = value;
            }
        }

        bool streaming = false;

        string udp_address = "";
        int udp_port = 0;

        private bool _running = false;
        private Thread _StreamThread = null;

        public event EventHandler<bool> onStreamStatusChange;

        public int ID { get { return _id; } }
        private int _id = 0;

        public TSUdpStreamer(string udp_address, int udp_port, int Id, OTSource TSSource)
        {
            this.udp_address = udp_address;
            this.udp_port = udp_port;
            this._id = Id;

            // register for TS Stream
            TSSource.RegisterTSConsumer(Id, ts_data_queue);

            streaming = false;

            _StreamThread = new Thread(worker_thread);
            _StreamThread.Start();
        }

        public void Close()
        {
            _running = false;
            _StreamThread?.Abort();
        }

        public void worker_thread()
        {
            byte data;
            _running = true;
            // Create a UDP client to send data to VLC
            UdpClient udpClient = new UdpClient();

            // Set the destination IP address and port of VLC
            IPAddress vlcIpAddress = IPAddress.Parse(udp_address); // replace with the actual IP address of VLC
            int vlcPort = udp_port;

            bool ts_sync = false;

            try
            {
                while (_running)
                {

                    if (streaming == false && stream == true)
                    {
                        streaming = true;
                        ts_sync = false;

                        onStreamStatusChange?.Invoke(this, true);
                    }
                    else
                    {
                        if (streaming == true && stream == false)
                        {
                            streaming = false;
                            onStreamStatusChange?.Invoke(this, false);
                        }
                    }

                    // if we are streaming, throw away data until synced
                    if (streaming == true && ts_sync == false) 
                    {
                        if (ts_data_queue.Count > 0)
                        {
                            if (ts_data_queue.TryPeek() == 0x47)
                            {
                                Log.Information("TS Synced");
                                ts_sync = true;
                            }
                            else
                            {
                                data = ts_data_queue.Dequeue();
                            }
                        }
                    }

                    // we are streaming and in sync
                    if (streaming && ts_sync)
                    {
                        if (ts_data_queue.Count >= 188)
                        {

                            if (ts_data_queue.TryPeek() != 0x47)
                            {
                                Log.Information("TS Sync Lost");
                                ts_sync = false;
                                continue;
                            }

                            byte[] dt = new byte[188];
                            int count = 0;

                            while (count < 188)
                            {
                                if (ts_data_queue.Count > 0)
                                {
                                    data = ts_data_queue.Dequeue();
                                    dt[count++] = data;
                                }
                                else
                                {
                                    Log.Information("Warning: Trying to dequeue, but no bytes : TSUdpThread");
                                }
                            }

                            udpClient.Send(dt, count, new IPEndPoint(vlcIpAddress, vlcPort));
                        }
                        else  // streaming but not enough data yet
                        {
                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        // throw away data
                        if (ts_data_queue.Count > 0)
                            ts_data_queue.Clear();
                        Thread.Sleep(100);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Log.Information("TS UDP Thread Closed");
                Thread.ResetAbort();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace opentuner
{
    public class TSUDPThread
    {
        ConcurrentQueue<byte> _ts_data_queue = new ConcurrentQueue<byte>();

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

        public TSUDPThread(TSThread _ts_thread, string udp_address, int udp_port)
        {
            _ts_thread.RegisterTSConsumer(_ts_data_queue);
            this.udp_address = udp_address;
            this.udp_port = udp_port;
        }

        public void worker_thread()
        {
            byte data;

            // Create a UDP client to send data to VLC
            UdpClient udpClient = new UdpClient();

            // Set the destination IP address and port of VLC
            IPAddress vlcIpAddress = IPAddress.Parse(udp_address); // replace with the actual IP address of VLC
            int vlcPort = udp_port; 

            try
            {
                while (true)
                {

                    if (streaming == false && stream == true)
                    {
                        streaming = true;
                    }
                    else
                    {
                        if (streaming == true && stream == false)
                        {
                            streaming = false;
                        }
                    }

                    int ts_data_count = _ts_data_queue.Count();

                    if (ts_data_count >= 188)
                    {
                        byte[] dt = new byte[188];
                        int count = 0;

                        while (count < 188)
                        {
                            if (_ts_data_queue.TryDequeue(out data))
                                dt[count++] = data;
                        }

                        if (streaming)
                        {
                            udpClient.Send(dt, count, new IPEndPoint(vlcIpAddress, vlcPort));
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("TS UDP Thread: Closing ");
            }
            finally
            {
                Console.WriteLine("Closing TS UDP");
            }
        }
    }
}

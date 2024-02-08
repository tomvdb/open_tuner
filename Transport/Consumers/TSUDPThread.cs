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

namespace opentuner
{
    public class TSUDPThread
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

        public TSUDPThread(string udp_address, int udp_port)
        {
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

            bool ts_sync = false;

            try
            {
                while (true)
                {

                    if (streaming == false && stream == true)
                    {
                        streaming = true;
                        ts_sync = false;
                    }
                    else
                    {
                        if (streaming == true && stream == false)
                        {
                            streaming = false;
                        }
                    }

                    // if we are streaming, throw away data until synced
                    if (streaming == true && ts_sync == false) 
                    {
                        if (ts_data_queue.Count > 0)
                        {
                            if (ts_data_queue.TryPeek() == 0x47)
                            {
                                Console.WriteLine("TS Synced");
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
                                Console.WriteLine("TS Sync Lost");
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
                                    Console.WriteLine("Warning: Trying to dequeue, but no bytes : TSUdpThread");
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
                Console.WriteLine("TS UDP Thread: Closing ");
            }
            finally
            {
                Console.WriteLine("Closing TS UDP");
            }
        }
    }
}

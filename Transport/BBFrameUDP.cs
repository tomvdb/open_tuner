using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using opentuner.Classes;

namespace opentuner.Transport
{
    public class BBFrameUDP
    {
        CircularBuffer _ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

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

        public BBFrameUDP(TSThread _ts_thread, string udp_address, int udp_port)
        {
            _ts_thread.RegisterTSConsumer(_ts_data_queue);
            this.udp_address = udp_address;
            this.udp_port = udp_port;
        }

        public void worker_thread()
        {
            byte data;

            // Create a UDP client 
            UdpClient udpClient = new UdpClient();

            IPAddress vlcIpAddress = IPAddress.Parse(udp_address); 
            int vlcPort = udp_port;

            bool header_sync = false;

            BinaryWriter binWriter = null;

            try
            {
                while (true)
                {
                    if (streaming == false && stream == true)
                    {
                        string fileName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" +  ".bin";
                        Console.WriteLine("New BBFrame File: " + fileName);
                        binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
                        streaming = true;
                        header_sync = false;
                    }
                    else
                    {
                        if (streaming == true && stream == false)
                        {
                            streaming = false;
                            if (binWriter != null)
                            {
                                binWriter.Close();
                            }
                        }
                    }

                    /*
                    // if we are streaming, throw away data until synced
                    if (streaming == true && header_sync == false)
                    {
                        if (_ts_data_queue.Count > 0)
                        {
                            if (_ts_data_queue.TryPeek() == 0x47)
                            {
                                Console.WriteLine("TS Synced");
                                ts_sync = true;
                            }
                            else
                            {
                                data = _ts_data_queue.Dequeue();
                            }
                        }
                    }
                    */

                    // we are streaming and in sync
                    if (streaming )
                    {
                        if (_ts_data_queue.Count > 0)
                        {
                            data = _ts_data_queue.Dequeue();

                            binWriter.Write(data);

                            //udpClient.Send(dt, count, new IPEndPoint(vlcIpAddress, vlcPort));
                        }
                        else  // streaming but not enough data yet
                        {
                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        // throw away data
                        if (_ts_data_queue.Count > 0)
                            _ts_data_queue.Clear();
                        Thread.Sleep(100);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("BBFrame Thread: Closing ");
            }
            finally
            {
                Console.WriteLine("Closing BBFrame UDP");
                if (binWriter != null)
                {
                    binWriter.Close();
                }

            }
        }
    }
}

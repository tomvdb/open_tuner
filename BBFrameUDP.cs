using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms.VisualStyles;
using Serilog;

namespace opentuner
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

            BinaryWriter binWriter = null;

            try
            {
                while (true)
                {
                    if (streaming == false && stream == true)
                    {
                        string fileName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" +  ".bin";
                        Log.Information("New BBFrame File: " + fileName);
                        binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
                        streaming = true;
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

                    // we are streaming
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
                Log.Information("BBFrame Thread: Closing ");
            }
            finally
            {
                Log.Information("Closing BBFrame UDP");
                if (binWriter != null)
                {
                    binWriter.Close();
                }
            }
        }
    }
}

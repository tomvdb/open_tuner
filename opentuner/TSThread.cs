using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;

namespace opentuner
{
    public class TSThread
    {
        ftdi hardware;
        ConcurrentQueue<NimStatus> status_queue;

        public TSThread(ftdi _hardware, ConcurrentQueue<NimStatus> _status_queue)
        {
            hardware = _hardware;
            status_queue = _status_queue;
        }

        public void worker_thread()
        {
            BinaryWriter binWriter = null;

            try
            {
                Console.WriteLine("TS Thread: Starting...");

                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
                UdpClient newsock = new UdpClient(ipep);

                IPEndPoint dest = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

                NimStatus nim_status = null;

                byte[] data = new byte[20*512];

                while (true)
                {
                    if (status_queue.Count() > 0 )
                    {
                        while (status_queue.TryDequeue(out nim_status))
                        {
                            if (nim_status.reset)
                            {
                                Console.WriteLine("Reset TS Queue");
                                uint len = 1;

                                while (len > 0)
                                {
                                    hardware.ftdi_ts_read(ref data, ref len);
                                }

                                if (binWriter != null)
                                {
                                    binWriter.Close();
                                }

                                Console.WriteLine("New File");
                                binWriter = new BinaryWriter(File.Open("c:\\temp\\test.ts", FileMode.Create));
                            }
                        }
                    }
                           
                    uint dataRead = 0;
                    if (hardware.ftdi_ts_read(ref data, ref dataRead) != 0)
                        Console.WriteLine("Read Error");

                    if (dataRead > 0)
                    {
                        if (dataRead != data.Length)
                            Console.WriteLine("Not full Packet *********** : " + dataRead.ToString());

                        newsock.Send(data, Convert.ToInt32(dataRead), dest);

                        if ( binWriter != null)
                            binWriter.Write(data,0,Convert.ToInt32(dataRead)); 
                    }

                    }
                }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine("TS Thread: Closing");
            }
            finally
            {
                Console.WriteLine("Closing TS");
                binWriter.Close();
            }

        }

    }
}

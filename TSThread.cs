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

    public delegate void TSDataCallback(TSStatus ts_status);

    public class TSThread
    {
        ftdi hardware;
        ConcurrentQueue<NimStatus> status_queue;

        ConcurrentQueue<byte> raw_ts_data_queue = null;
        ConcurrentQueue<byte> parser_ts_data_queue = null;

        public TSThread(ftdi _hardware, ConcurrentQueue<NimStatus> _status_queue,  ConcurrentQueue<byte> _raw_ts_data_queue, ConcurrentQueue<byte> _parser_ts_data_queue)
        {
            hardware = _hardware;
            status_queue = _status_queue;
            raw_ts_data_queue = _raw_ts_data_queue;
            parser_ts_data_queue = _parser_ts_data_queue;
        }

        public void worker_thread()
        {
            //BinaryWriter binWriter = null;
            bool bufferingData = false;

            try
            {
                Console.WriteLine("TS Thread: Starting...");

                /*
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
                UdpClient newsock = new UdpClient(ipep);

                IPEndPoint dest = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
                */

                NimStatus nim_status = null;
                //RawTSData raw_data = null;
                byte raw_data = 0;

                //byte[] data = new byte[8*512];
                //byte[] data = new byte[1];
                byte[] data = new byte[4096];

                while (true)
                {
                    if (status_queue.Count() > 0 )
                    {
                        while (status_queue.TryDequeue(out nim_status))
                        {
                            // no demod active, throw away ts data received, if any
                            if (nim_status.build_queue == false)
                            {
                                bufferingData = false;
                            }

                            if (nim_status.build_queue == true && bufferingData == false)   // if we are locked
                            {
                                // clear out the queue first
                                Console.WriteLine("Clearing out TS Queue");

                                while (raw_ts_data_queue.Count() > 0)
                                {
                                    raw_ts_data_queue.TryDequeue(out raw_data);
                                }

                                Console.WriteLine("Done... starting buffering");

                                bufferingData = true;
                            }


                        }
                    }
                           
                    uint dataRead = 0;
                    //Console.WriteLine("TSThread: Reading TS Data");

                    if (hardware.ftdi_ts_read(ref data, ref dataRead) != 0)
                        Console.WriteLine("Read Error");

                    if (dataRead > 0)
                    {
                        for ( int c = 0; c < dataRead; c++)
                        {
                            //RawTSData raw_ts_data = new RawTSData();
                            //raw_ts_data.rawTSData = new byte[1];
                            //raw_ts_data.rawTSData[0] = data[c];
                            //raw_ts_data.datalen = 1;
                            raw_ts_data_queue.Enqueue(data[c]);
                            parser_ts_data_queue.Enqueue(data[c]);
                        }
                    }

                    /*
                    if (dataRead > 0)
                    {
                        //Console.WriteLine("TSThread: " + dataRead.ToString() + " bytes - " + bufferingData.ToString());

                        if (bufferingData)
                        {
                            RawTSData raw_ts_data = new RawTSData();
                            raw_ts_data.rawTSData = data.ToArray();
                            raw_ts_data.datalen = Convert.ToInt32(dataRead);
                            raw_ts_data_queue.Enqueue(raw_ts_data);
                        }

                        // send ts data callback

                        //ts_data_callback?.Invoke(raw_ts_data);

                        //newsock.Send(data, Convert.ToInt32(dataRead), dest);

                        //if ( binWriter != null)
                        //    binWriter.Write(data,0,Convert.ToInt32(dataRead)); 
                    }
                    */

                    }
                }
            catch (ThreadAbortException)
            {
                Console.WriteLine("TS Thread: Closing ");
            }
            finally
            {
            }
        }
    }
}

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

        //ConcurrentQueue<byte> raw_ts_data_queue = null;
        //ConcurrentQueue<byte> parser_ts_data_queue = null;

        NimThread nim_thread = null;

        bool prev_locked_status = false;
        bool locked = false;
        bool ts_build_queue = false;

        private List<ConcurrentQueue<byte>> registered_consumers = new List<ConcurrentQueue<byte>>();

        public byte ts_device = ftdi.TS2;

        public TSThread(ftdi _hardware, ConcurrentQueue<byte> _raw_ts_data_queue, NimThread _nim_thread, byte _ts_device)
        {
            Console.WriteLine(" >> Starting TS Thread <<");

            hardware = _hardware;
            //raw_ts_data_queue = _raw_ts_data_queue;
            //parser_ts_data_queue = _parser_ts_data_queue;
            nim_thread = _nim_thread;

            //nim_thread.onNewStatus += Nim_thread_onNewStatus;
            NimStatusCallback status_callback = new NimStatusCallback(Nim_thread_onNewStatus);

            //nim_thread.register_callback(status_callback);

            Console.WriteLine(" >> Registering Raw TS Queue << ");
            registered_consumers.Add(_raw_ts_data_queue);

            ts_device = _ts_device;
        }

        public void RegisterTSConsumer(ConcurrentQueue<byte> raw_ts_data_queue)
        {
            Console.WriteLine(" >> Registering New Queue << ");
            registered_consumers.Add(raw_ts_data_queue);
        }

        //private void Nim_thread_onNewStatus(object sender, StatusEvent e)
        private void Nim_thread_onNewStatus(NimStatus nim_status)
        {
            /*
            //Console.WriteLine("new Nim status - " + ts_device.ToString());
            Console.WriteLine(locked.ToString() + " - " + prev_locked_status.ToString());

            byte raw_data = 0;

            if (prev_locked_status != locked)
            {
                Console.WriteLine( ts_device.ToString() + " - "  + locked.ToString() + " - " + prev_locked_status.ToString());
            }

            if (ts_device == ftdi.TS2)
            {

                if (nim_status.T1P2_demod_status >= 2) locked = true;

                if (prev_locked_status != locked)
                {
                    Console.WriteLine("New Nim Status : T1P2");
                    Console.WriteLine(nim_status.T1P2_demod_status.ToString());

                    if (nim_status.T1P2_demod_status >= 2)
                    {
                        ts_build_queue = true;
                    }
                    else
                    {
                        ts_build_queue = false;
                    }
                }

            }
            else
            {

                if (nim_status.T2P1_demod_status >= 2) locked = true;

                if (prev_locked_status != locked)
                {
                    Console.WriteLine("New Nim Status : T2P1");
                    Console.WriteLine(nim_status.T2P1_demod_status.ToString());

                    if (nim_status.T2P1_demod_status >= 2)
                    {
                        ts_build_queue = true;
                    }
                    else
                    {
                        ts_build_queue = false;
                    }
                }


            }

            prev_locked_status = locked;
            */

        }

        public void stop_ts()
        {
            Console.WriteLine("Stop TS" + ts_device.ToString());
            ts_build_queue = false;
        }

        public void start_ts()
        {
            Console.WriteLine("Start TS" + ts_device.ToString());
            ts_build_queue = true;
        }

        public void worker_thread()
        {
            //BinaryWriter binWriter = null;
            bool bufferingData = false;
            Console.WriteLine(">> Starting TS Worked Thread <<");

            try
            {
                Console.WriteLine("TS Thread: Starting...");

                /*
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 12000);
                UdpClient newsock = new UdpClient(ipep);
                IPEndPoint dest = new IPEndPoint(IPAddress.Parse("192.168.0.105"), 12000);
                */

                //IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12000);
                //UdpClient newsock = new UdpClient();
                //newsock.Connect(ipep);


                NimStatus nim_status = null;
                //RawTSData raw_data = null;
                byte raw_data = 0;

                //byte[] data = new byte[8*512];
                //byte[] data = new byte[1];
                byte[] data = new byte[4096];

                while (true)
                {

                    if (ts_build_queue == false)
                    {
                        //Console.WriteLine("ts build queue, false");

                        hardware.ftdi_ts_flush(ts_device);

                        bufferingData = false;
                        while (registered_consumers[0].Count() > 0)
                        {
                            registered_consumers[0].TryDequeue(out raw_data);
                        }


                        continue;
                    }

                    if (ts_build_queue == true && bufferingData == false)
                    {
                        Console.WriteLine("ts build queue, true");

                        // clear out the queue first
                        Console.WriteLine("Clearing out TS Queue");

                        hardware.ftdi_ts_flush(ts_device);

                        while (registered_consumers[0].Count() > 0)
                        {
                            registered_consumers[0].TryDequeue(out raw_data);
                        }

                        Console.WriteLine("Done... starting buffering");

                        bufferingData = true;
                    }

                    /*
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
                        
                            }

                            Thread.Sleep(50);

                        }
                    }
                    */
                           
                    uint dataRead = 0;
                    //Console.WriteLine("TSThread: Reading TS Data");

                        if (hardware.ftdi_ts_read(ts_device, ref data, ref dataRead) != 0)
                            Console.WriteLine("Read Error");

                    if (dataRead > 0)
                    {
                        //Console.WriteLine(dataRead);

                        for ( int c = 0; c < dataRead; c++)
                        {
                            //RawTSData raw_ts_data = new RawTSData();
                            //raw_ts_data.rawTSData = new byte[1];
                            //raw_ts_data.rawTSData[0] = data[c];
                            //raw_ts_data.datalen = 1;
                            for (int consumers = 0; consumers < registered_consumers.Count; consumers++)
                            {
                                if (registered_consumers[consumers] != null)
                                    registered_consumers[consumers].Enqueue(data[c]);
                                //parser_ts_data_queue.Enqueue(data[c]);
                            }
                        }

                        //newsock.Send(data, Convert.ToInt32(dataRead));
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

                    //Thread.Sleep(20);

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

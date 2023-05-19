using System;
using System.Collections.Generic;
using System.Threading;

namespace opentuner
{

    public delegate void TSDataCallback(TSStatus ts_status);

    public class TSThread
    {
        ftdi hardware;

        NimThread nim_thread = null;

        bool ts_build_queue = false;

        private List<CircularBuffer> registered_consumers = new List<CircularBuffer>();

        public byte ts_device = ftdi.TS2;

        public TSThread(ftdi _hardware, CircularBuffer _raw_ts_data_queue, NimThread _nim_thread, byte _ts_device)
        {
            Console.WriteLine(" >> Starting TS Thread <<");

            hardware = _hardware;
            nim_thread = _nim_thread;

            //NimStatusCallback status_callback = new NimStatusCallback(Nim_thread_onNewStatus);

            Console.WriteLine(" >> Registering Raw TS Queue << ");
            registered_consumers.Add(_raw_ts_data_queue);

            ts_device = _ts_device;
        }

        public void RegisterTSConsumer(CircularBuffer raw_ts_data_queue)
        {
            Console.WriteLine(" >> Registering New Queue << ");
            registered_consumers.Add(raw_ts_data_queue);
        }

        /*
        private void Nim_thread_onNewStatus(NimStatus nim_status)
        {
            if (ts_device == ftdi.TS2)
            {
                Console.WriteLine("Stream Format (1): " + nim_status.T1P2_stream_format.ToString());
            }
            else
            {
                Console.WriteLine("Stream Format (2): " + nim_status.T2P1_stream_format.ToString());
            }
        }
        */

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

                //NimStatus nim_status = null;
                //byte raw_data = 0;

                byte[] data = new byte[4096];

                while (true)
                {

                    if (ts_build_queue == false)
                    {
                        //Console.WriteLine("ts build queue, false");

                        hardware.ftdi_ts_flush(ts_device);

                        bufferingData = false;

                        registered_consumers[0].Clear();

                        continue;
                    }

                    if (ts_build_queue == true && bufferingData == false)
                    {
                        //Console.WriteLine("ts build queue, true");
                        //Console.WriteLine("Clearing out TS Queue");

                        hardware.ftdi_ts_flush(ts_device);

                        registered_consumers[0].Clear();

                        //Console.WriteLine("Done... starting buffering");

                        bufferingData = true;
                    }


                           
                    uint dataRead = 0;
                    //Console.WriteLine("TSThread: Reading TS Data");

                    if (hardware.ftdi_ts_read(ts_device, ref data, ref dataRead) != 0)
                        Console.WriteLine("Read Error");

                    if (dataRead > 0)
                    {
                        for ( int c = 0; c < dataRead; c++)
                        {
                            for (int consumers = 0; consumers < registered_consumers.Count; consumers++)
                            {
                                if (registered_consumers[consumers] != null)
                                    registered_consumers[consumers].Enqueue(data[c]);
                            }
                        }
                    }
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

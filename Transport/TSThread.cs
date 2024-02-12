using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace opentuner
{
    // this thread will read TS data using the provided callback functions and pass it along to the TS consumers (eg. TSParserThread)
    // this should be device agnostic
    // callback functions required for reading TS data from source
    public delegate void FlushTS();
    public delegate byte ReadTS(ref byte[] data, ref uint BytesRead);

    public class TSThread
    {
        private bool ts_build_queue = false;
        private List<CircularBuffer> registered_consumers = new List<CircularBuffer>();
        private FlushTS flush_ts_callback = null;
        private ReadTS read_ts_callback = null;
        private string identifier = "";

        public TSThread(CircularBuffer _raw_ts_data_queue, NimThread _nim_thread, FlushTS _flush_ts_callback, ReadTS _read_ts_callback, string _identifier)
        {
            Console.WriteLine(" >> Starting TS Thread <<");
            Console.WriteLine(" >> Registering Raw TS Queue << ");

            registered_consumers.Add(_raw_ts_data_queue);

            flush_ts_callback = _flush_ts_callback;
            read_ts_callback = _read_ts_callback;
            identifier = _identifier;
        }

        public void RegisterTSConsumer(CircularBuffer raw_ts_data_queue)
        {
            Console.WriteLine(" >> Registering New Queue << ");
            registered_consumers.Add(raw_ts_data_queue);
        }

        public void stop_ts()
        {
            Console.WriteLine("Stopping TS: " + identifier);
            ts_build_queue = false;
        }

        public void start_ts()
        {
            Console.WriteLine("Starting TS:" + identifier);
            ts_build_queue = true;
        }

        public void worker_thread()
        {
            bool bufferingData = false;
            Console.WriteLine(">> Starting TS Worked Thread <<");

            try
            {
                Console.WriteLine("TS Thread: Starting...");

                byte[] data = new byte[4096];

                while (true)
                {

                    if (ts_build_queue == false)
                    {
                        if (flush_ts_callback != null)
                            flush_ts_callback();

                        bufferingData = false;
                        registered_consumers[0].Clear();
                        continue;
                    }

                    if (ts_build_queue == true && bufferingData == false)
                    {
                        if (flush_ts_callback != null)
                            flush_ts_callback();

                        registered_consumers[0].Clear();
                        bufferingData = true;
                    }
                         
                    uint dataRead = 0;

                    if (read_ts_callback(ref data, ref dataRead) != 0)
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

            Console.WriteLine("TS Thread Closed");
        }
    }
}

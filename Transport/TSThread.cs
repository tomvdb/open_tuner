using Serilog;
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

        private EventWaitHandle thread_wait_event_handle;

        public TSThread(CircularBuffer _raw_ts_data_queue, FlushTS _flush_ts_callback, ReadTS _read_ts_callback, string _identifier)
        {
            Log.Information(" >> Starting TS Thread <<");
            Log.Information(" >> Registering Raw TS Queue << ");

            thread_wait_event_handle = new EventWaitHandle(false, EventResetMode.AutoReset);

            registered_consumers.Add(_raw_ts_data_queue);

            flush_ts_callback = _flush_ts_callback;
            read_ts_callback = _read_ts_callback;
            identifier = _identifier;
        }

        public void NewDataPresent()
        {
            thread_wait_event_handle.Set();
        }

        public void RegisterTSConsumer(CircularBuffer raw_ts_data_queue)
        {
            Log.Information(" >> Registering New Queue << ");
            registered_consumers.Add(raw_ts_data_queue);
        }

        public void stop_ts()
        {
            Log.Information("Stopping TS: " + identifier);
            ts_build_queue = false;
            thread_wait_event_handle.Set(); // fire worker thread to handle stop event
        }

        public void start_ts()
        {
            Log.Information("Starting TS:" + identifier);
            ts_build_queue = true;
            thread_wait_event_handle.Set(); // fire worker thread to handle start event
        }

        public void worker_thread()
        {
            bool bufferingData = false;
            Log.Information(">> Starting TS Worked Thread <<");

            try
            {
                Log.Information("TS Thread: Starting...");

                byte[] data = new byte[4096];
                uint dataRead = 0;

                while (true)
                {
                    // Wait on an Event.
                    thread_wait_event_handle.WaitOne();

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

                    if (bufferingData == true)
                    {
                        dataRead = 0;

                        if (read_ts_callback(ref data, ref dataRead) != 0)
                            Log.Information("Read Error");

                        if (dataRead > 0)
                        {
                            for (int c = 0; c < dataRead; c++)
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
            }
            catch (ThreadAbortException)
            {
                Log.Information("TS Thread: Closing ");
            }
            finally
            {
            }

            Log.Information("TS Thread Closed");
        }
    }
}

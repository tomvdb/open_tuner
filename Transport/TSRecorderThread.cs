using System;
using System.IO;
using System.Threading;
using opentuner.Classes;

namespace opentuner.Transport
{
    public class TSRecorderThread
    {

        CircularBuffer _ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);

        object locker = new object();

        public event EventHandler<bool> onRecordStatusChange;

        protected bool _record;
        public bool record
        {
            get
            {
                lock (locker)
                    return _record;
            }
            set
            {
                lock (locker)
                    _record =value;
            }
        }

        bool recording = false;
        string media_path = "";
        public string id = "";

        public TSRecorderThread(TSThread _ts_thread, string _media_path, string _id)
        {
            _ts_thread.RegisterTSConsumer(_ts_data_queue);
            media_path = _media_path;
            id = _id;
        }

        public void worker_thread()
        {
            BinaryWriter binWriter = null;
            byte data;
            bool ts_sync = true;
            try
            {
                while (true)
                {
                    if (recording == false && record == true)
                    {
                        // open a new file
                        Console.WriteLine("recording");
                        string filename = media_path + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + id + ".ts";
                        binWriter = new BinaryWriter(File.Open(filename, FileMode.Create));
                        recording = true;
                        ts_sync = true;

                        onRecordStatusChange?.Invoke(this, true);
                    }
                    else
                    {
                        if (recording == true && record == false)
                        {
                            Console.WriteLine("stop recording");
                            recording = false;

                            // stop record
                            binWriter.Close();
                            binWriter.Dispose();
                            binWriter = null;

                            onRecordStatusChange?.Invoke(this, false);
                        }
                    }

                    int ts_data_count = _ts_data_queue.Count;

                    if (ts_data_count > 0)
                    {
                        //if (_ts_data_queue.TryDequeue(out data))
                        //{                        
                        data = _ts_data_queue.Dequeue();

                            if (record == true)
                            {

                                if (binWriter != null)
                                {
                                    if (ts_sync == true && data == 0x47)
                                    {
                                        Console.WriteLine("TS Header Sync");
                                        ts_sync = false;
                                    }

                                    if (ts_sync == false)
                                    { 
                                        binWriter.Write(data);
                                    }
                                }
                            }
                        //}
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("TS Recorded Thread: Closing ");
            }
            finally
            {
                Console.WriteLine("Closing TS Recorder");
            }
        }
    }
}

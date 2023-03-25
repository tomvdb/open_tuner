using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace opentuner
{
    public class TSRecorderThread
    {

        ConcurrentQueue<byte> _ts_data_queue = new ConcurrentQueue<byte>();

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

        public TSRecorderThread(TSThread _ts_thread, string _media_path)
        {
            _ts_thread.RegisterTSConsumer(_ts_data_queue);
            media_path = _media_path;
        }

        public void worker_thread()
        {
            BinaryWriter binWriter = null;
            byte data;

            try
            {
                while (true)
                {
                    if (recording == false && record == true)
                    {
                        // open a new file
                        Console.WriteLine("recording");
                        string filename = this.media_path + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".ts";
                        binWriter = new BinaryWriter(File.Open(filename, FileMode.Create));
                        recording = true;

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

                    int ts_data_count = _ts_data_queue.Count();

                    if (ts_data_count > 0)
                    {
                        if (_ts_data_queue.TryDequeue(out data))
                        {
                            if (record == true)
                            {
                                if (binWriter != null)
                                {
                                    binWriter.Write(data);
                                }
                            }
                        }
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

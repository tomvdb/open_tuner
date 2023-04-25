using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace opentuner
{
    public class TSStreamMediaInput : LibVLCSharp.Shared.MediaInput
    {

        ConcurrentQueue<byte> ts_data_queue;
        public bool ts_sync = false;

        public TSStreamMediaInput(ConcurrentQueue<byte> _ts_data_queue )
        {
            // we can't seek live data
            CanSeek = false;
            ts_data_queue = _ts_data_queue;
        }

        public override bool Open(out ulong size)
        {
            Console.WriteLine("Open Media Input");

            bool success = true;
            size = ulong.MaxValue;

            return success;
        }

        public override void Close()
        {
            Console.WriteLine("Close Media Input");
        }


        public override int Read(IntPtr buf, uint len)
        {
            //Console.WriteLine("Read from VLC: " + len.ToString()) ;

            int timeout = 0;

            // wait for next data
            while (ts_data_queue.Count() < 188)
            {
                //Console.WriteLine("Waiting: " + timeout.ToString());
                // if we haven't received anything within a few seconds then most likely won't get anything
                if (timeout > 5000)
                {
                    Console.WriteLine("TSStreamMediaInput : Read Timeout");
                    return 0;
                }

                Thread.Sleep(50);
                timeout += 50;
            }

            int queue_count = ts_data_queue.Count();    // this is slow, so we do it once here and use an internal variable

            if (queue_count > 0)
            {
                //RawTSData raw_ts_data = null;
                byte raw_ts_data = 0;

                uint buildLen = len;
                if (queue_count < buildLen)
                {
                    buildLen = Convert.ToUInt32(queue_count);
                }

                byte[] vlc_data = new byte[buildLen];

                int counter = 0;

                while (counter < buildLen)
                {
                    if (ts_data_queue.TryDequeue(out raw_ts_data))
                    {
                        //vlc_data[counter++] = raw_ts_data.rawTSData[0];

                        if (ts_sync == false && raw_ts_data != 0x47)
                        {
                            continue;
                        }
                        else
                        {
                            ts_sync = true;
                            vlc_data[counter++] = raw_ts_data;
                        }
                    }
                }

                Marshal.Copy(vlc_data.ToArray(), 0, buf, vlc_data.Length);
                return vlc_data.Length;

            }

            Console.WriteLine("TS StreamInput: Shouldn't be here");
            return 0;
        }

        public override bool Seek(ulong offset)
        {
            // seeking is not allowed/possible
            return false;
        }

    }
}

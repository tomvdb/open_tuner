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
            //Console.WriteLine("Read from VLC");

            int timeout = 0;

            // wait for next data
            while (ts_data_queue.Count() < 188)
            {
                // if we haven't received anything within a second then most likely won't get anything
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
                        vlc_data[counter++] = raw_ts_data;
                    }
                }

                Marshal.Copy(vlc_data.ToArray(), 0, buf, vlc_data.Length);
                return vlc_data.Length;

                /*

                if (ts_data_queue.TryDequeue(out raw_ts_data))
                {
                    //Console.WriteLine("Sending Data to player");


                    if (len < raw_ts_data.datalen )
                    {
                        Console.WriteLine("oooh, this is about to crash: " + len.ToString());
                    }

                    //Console.WriteLine("Data: " + raw_ts_data.datalen.ToString() + "," + ts_data_queue.Count());

                    Marshal.Copy(raw_ts_data.rawTSData.ToArray(), 0, buf, raw_ts_data.datalen);
                    return raw_ts_data.datalen;
                }
                else
                {
                    Console.WriteLine("DeQueue Failed");
                }
                */
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

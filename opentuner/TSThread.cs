using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace opentuner
{
    public class TSThread
    {
        ftdi hardware;

        public TSThread(ftdi _hardware)
        {
            hardware = _hardware;
        }

        public void worker_thread()
        {

            try
            {
                Console.WriteLine("TS Thread: Starting...");
                uint ts_available = 0;

                while (true)
                {

                    byte err = hardware.ftdi_ts_available(ref ts_available);

                    //if (ts_available > 0)
                    Console.WriteLine("TS Bytes: " + ts_available);

                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine("TS Thread: Closing");
            }

        }

    }
}

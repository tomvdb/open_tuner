using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NAudio.Dmo;
using Serilog;
using static opentuner.MediaSources.OTSource;

namespace opentuner.MediaSources.Winterhill
{
    public class PicoWHBroadcastListener
    {
        private Thread listener_thread;
        private bool CloseThread = false;
        private UdpClient listener = new UdpClient(9997);

        public delegate void OnBroadcastDelegate(string data);

        public event OnBroadcastDelegate OnBroadcast;

        public PicoWHBroadcastListener() 
        {
            listener_thread = new Thread(ListenerThread);
            listener_thread.Start();
        }

        public void Close()
        {
            CloseThread = true;
            listener_thread?.Abort();
            listener.Close();
        }

        public void ListenerThread()
        {
            int port = 9997;
        
            while (!CloseThread)
            {

                //Log.Information("Listening for PicoTuner (WH) Broadcasts");
                
                try
                { 
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                    byte[] data = listener.Receive(ref remoteEndPoint);

                    string receivedMessage = Encoding.ASCII.GetString(data);

                    OnBroadcast?.Invoke(receivedMessage);

                    //Log.Information(remoteEndPoint.ToString());
                    //Log.Information(receivedMessage);
                } 
                catch (Exception ex)
                {    
                }
                

             }

            listener.Close();
            Log.Information("Broadcast Listener Thread Closed");
        }
    }
}

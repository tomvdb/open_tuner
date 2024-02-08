using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace opentuner.Utilities
{
    public class UdpListener
    {
        private UdpClient udpClient;
        private int port;
        private Thread listenerThread;

        // Event to be raised when data is received
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public UdpListener(int port)
        {
            this.port = port;
        }

        public void StartListening()
        {
            udpClient = new UdpClient(port);
            listenerThread = new Thread(new ThreadStart(ListenForData));
            listenerThread.Start();
        }

        private void ListenForData()
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string receivedMessage = Encoding.UTF8.GetString(data);

                    // Raise the DataReceived event
                    OnDataReceived(new DataReceivedEventArgs(receivedMessage, remoteEndPoint));
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            // Raise the DataReceived event
            DataReceived?.Invoke(this, e);
        }

        public void StopListening()
        {
            udpClient.Close();
            listenerThread.Join();
        }
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public IPEndPoint RemoteEndPoint { get; }

        public DataReceivedEventArgs(string message, IPEndPoint remoteEndPoint)
        {
            Message = message;
            RemoteEndPoint = remoteEndPoint;
        }
    }
}

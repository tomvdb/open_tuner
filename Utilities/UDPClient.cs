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
    public class UDPClient
    {
        private UdpClient udpClient;
        private int port;
        private bool isListening;
        private Thread listenThread;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<string> ConnectionStatusChanged;

        public UDPClient(int port)
        {
            this.port = port;
            udpClient = new UdpClient(port);
            isListening = false;
            listenThread = new Thread(ListenForData);
        }

        public void Connect()
        {
            if (!isListening)
            {
                isListening = true;
                listenThread.Start();

                OnConnectionStatusChanged("Connected");
            }
        }

        public void Disconnect()
        {
            if (isListening)
            {
                isListening = false;
                listenThread.Join(); // Wait for the thread to finish

                udpClient.Close();

                OnConnectionStatusChanged("Disconnected");
            }
        }

        private void ListenForData()
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (isListening)
                {
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                    OnDataReceived(receivedBytes);
                }
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged($"Error: {ex.Message}");
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        protected virtual void OnConnectionStatusChanged(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }
    }
}

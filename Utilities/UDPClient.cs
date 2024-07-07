using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Serilog;

namespace opentuner.Utilities
{
    public class UDPClient
    {
        private UdpClient udpClient;
        private int port;
        private bool isListening;
        private Thread listenThread;
        
        private int _id;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<bool> ConnectionStatusChanged;

        public int getID() { return _id; }  

        public void Close()
        {
            udpClient?.Close();
        }



        public UDPClient(int port)
        {
            this.port = port;
            udpClient = new UdpClient(port);
            isListening = false;
            listenThread = new Thread(ListenForData);
        }

        public UDPClient(int port, int ID)
        {
            _id = ID;
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

                OnConnectionStatusChanged(true);
            }
        }

        public void Disconnect()
        {
            if (isListening)
            {
                isListening = false;
                listenThread.Join(); // Wait for the thread to finish

                udpClient.Close();

                OnConnectionStatusChanged(false);
            }
        }

        private void ListenForData()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {

                while (isListening)
                {
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                    try
                    {
                        OnDataReceived(receivedBytes);
                    }
                    catch (Exception ex)
                    {
                        Log.Information("OnDataReceived event failed: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Information("Listen for UDP Data Exception: "  + this.port.ToString() + " : "+  ex.Message);
                OnConnectionStatusChanged(false);
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        protected virtual void OnConnectionStatusChanged(bool status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Serilog;
using System.Collections;
using opentuner.Utilities;
using System.Drawing;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSource
    {

        UdpClient WH_Client = new UdpClient();
        IPEndPoint winterhill_end_point;

        UDPClient longmynd_status = new UDPClient(9901);

        public void ConnectWinterhillUDP()
        {
            longmynd_status.Connect();
            longmynd_status.DataReceived += Longmynd_status_DataReceived;
            longmynd_status.ConnectionStatusChanged += Longmynd_status_ConnectionStatusChanged;
        }

        private void Longmynd_status_ConnectionStatusChanged(object sender, string e)
        {
            Log.Information("WH UDP Connection Status Changed: " + e);
        }

        private void Longmynd_status_DataReceived(object sender, byte[] e)
        {
            var data = Encoding.ASCII.GetString(e);

            string[] status_strings = data.Split('\n');

            monitorMessage mm = new monitorMessage();
            mm.type = "RX";
            mm.timestamp = 0;
            mm.rx = new ReceiverMessage[3];
            mm.rx[0] = new ReceiverMessage();
            mm.rx[1] = new ReceiverMessage();
            mm.rx[2] = new ReceiverMessage();

            mm.rx[0].rx = 99;
            mm.rx[1].rx = 99;
            mm.rx[2].rx = 99;

            int receiver = 1;

            for (int c = 0; c <  status_strings.Length; c++)
            {
                string[] dt = status_strings[c].Split(',');

                switch(dt[0])
                {
                    case "$0":
                        receiver = Convert.ToInt32(dt[1]); break;
                    case "$1":
                        switch(dt[1].Trim())
                        {
                            case "DVB-S2":
                                mm.rx[receiver].scanstate = 2;
                                break;
                            case "DVB-S1":
                                mm.rx[receiver].scanstate = 3;
                                break;
                            case "header":
                                mm.rx[receiver].scanstate = 1;
                                break;
                            case "search":
                                mm.rx[receiver].scanstate = 0;
                                break;
                            case "lost":
                                mm.rx[receiver].scanstate = 0;
                                break;
                            default:
                                Log.Warning("WH: Don't know how to decode: " + dt[1]);
                                mm.rx[receiver].scanstate = 1;
                                break;
                        }
                        break;
                    case "$6":
                        mm.rx[receiver].frequency = dt[1];
                        break;
                    case "$9":
                        mm.rx[receiver].symbol_rate = dt[1];
                        break;
                    case "$12":
                        mm.rx[receiver].mer = dt[1];
                        break;
                    case "$13":
                        mm.rx[receiver].service_name = dt[1]; 
                        break;
                    case "$14":
                        mm.rx[receiver].service_provider_name = dt[1]; 
                        break;
                    case "$15":
                        mm.rx[receiver].null_percentage = dt[1]; 
                        break;
                    case "$18":
                        mm.rx[receiver].modcod = dt[1];
                        break;
                    case "$30":
                        mm.rx[receiver].dbmargin = dt[1];
                        break;
                }
            }

            if (mm.rx[receiver].service_name == null)
                mm.rx[receiver].service_name = "";

            if (mm.rx[receiver].service_provider_name == null)
                mm.rx[receiver].service_provider_name = "";

            if (mm.rx[receiver].mer == null) 
                mm.rx[receiver].mer = "";

            if (mm.rx[receiver].modcod == null)
                mm.rx[receiver].modcod = "";

            if (mm.rx[receiver].dbmargin == null)
                mm.rx[receiver].dbmargin = "";


            mm.rx[receiver].rx = receiver;
            mm.rx[receiver].ts_addr = "";
            mm.rx[receiver].ts_port = (_settings.WinterhillUdpBasePort + (receiver == 1 ? 21 : 22)).ToString();

            UpdateInfo(mm);
        }

        public void UDPSetFrequency(int device, int freq, int sr)
        {

            winterhill_end_point = new IPEndPoint(IPAddress.Parse(_settings.WinterhillUdpHost), device == 0 ? 9921 : 9922);

            Log.Information("UDP Set Frequency Device: " + device);
            
            
            try
            {
                VideoChangeCB?.Invoke(device + 1, false);
                playing[device] = false;
                demodstate[device] = -1;
            }
            catch (Exception ex) { }
            

            byte[] outStream = Encoding.ASCII.GetBytes("[GlobalMsg],Freq=" + freq.ToString() + ",Offset=" + _settings.Offset[device].ToString() + ",Doppler=0,Srate=" + sr.ToString() + ",FPlug=A" + "\n");
            try
            {
                Log.Information("Setting WH UDP Frequency: " + freq.ToString());
                WH_Client.Client.SendTo(outStream, winterhill_end_point);
            }
            catch
            {
                Console.WriteLine("Error sending UDP Command");
            }
        }

    }
}

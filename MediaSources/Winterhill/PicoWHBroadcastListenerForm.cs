using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Serilog;

namespace opentuner.MediaSources.Winterhill
{
    public partial class PicoWHBroadcastListenerForm : Form
    {
        UdpClient WH_Client = new UdpClient();

        private PicoWHBroadcastListener broadCastListener;
        public PicoWHBroadcastListenerForm()
        {
            InitializeComponent();
        }


        private void UpdateLabel(Label Lbl, Object obj)
        {

            if (Lbl == null)
                return;

            if (obj == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateLabelDelegate ulb = new UpdateLabelDelegate(UpdateLabel);
                if (Lbl != null)
                {
                    Lbl?.Invoke(ulb, new object[] { Lbl, obj });
                }
            }
            else
            {
                Lbl.Text = obj.ToString();
            }

        }

        public static void UpdateLB(ListBox LB, Object obj)
        {

            if (LB == null)
                return;

            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(UpdateLB);
                if (LB != null)
                {
                    LB?.Invoke(ulb, new object[] { LB, obj });
                }
            }
            else
            {
                if (LB.Items.Count > 1000)
                {
                    LB.Items.Remove(0);
                }

                int i = LB.Items.Add(DateTime.Now.ToShortTimeString() + " : " + obj);
                LB.TopIndex = i;
            }

        }

        private void PicoWHBroadcastListenerForm_Load(object sender, EventArgs e)
        {
            try
            {
                broadCastListener = new PicoWHBroadcastListener();
                broadCastListener.OnBroadcast += BroadCastListener_OnBroadcast;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Opening the Broadcast Listener - Make sure you don't have one running already.\n" + ex.Message);
                Close();
            }
        }

        private void BroadCastListener_OnBroadcast(string data)
        {
            string[] message_data = data.Split('\n');

            for (int c = 0; c < message_data.Length; c++)
            {
                UpdateLB(lbBroadcast, message_data[c].Trim());

                if (message_data[c].Contains( "IP address"))
                {
                    UpdateLabel(lblDetectedIP, message_data[c].Substring(17).Trim());
                }

                if (message_data[c].Contains( "Base IP port"))
                {
                    UpdateLabel(lblDetectedBasePort, message_data[c].Substring(17).Trim());
                }
            }
            
        }

        public void CloseForm()
        {
            broadCastListener?.Close();
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void lblDetectedIP_TextChanged(object sender, EventArgs e)
        {
            if (lblDetectedIP.Text != "Unknown")
            {
                btnReset.Enabled = true;
                btnReboot.Enabled = true;
                btnBootsel.Enabled = true;
            }
        }

        private void lblDetectedBasePort_TextChanged(object sender, EventArgs e)
        {
            if (lblDetectedBasePort.Text != "Unknown")
            {
                btnReset.Enabled = true;
                btnReboot.Enabled = true;
                btnBootsel.Enabled = true;
            }
        }

        public void SendRemoteCommand(string command)
        {
            int baseport = 9900;

            if (!int.TryParse(lblDetectedBasePort.Text, out baseport))
                baseport = 9900;

            baseport = (baseport / 100);
            baseport = baseport * 100;
            baseport += 20;

            IPEndPoint remote_end_point = new IPEndPoint(IPAddress.Parse(lblDetectedIP.Text), baseport);

            byte[] outStream = Encoding.ASCII.GetBytes(command);

            try
            {
                Log.Information("Sending command : " + command + " to " + lblDetectedIP.Text + " : " + baseport.ToString());

                WH_Client.Client.SendTo(outStream, remote_end_point);
            }
            catch
            {
                Console.WriteLine("Error sending UDP Command");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SendRemoteCommand("[to@wh] reset=147");
        }

        private void btnReboot_Click(object sender, EventArgs e)
        {
            SendRemoteCommand("[to@wh] reboot=258");
        }

        private void btnBootsel_Click(object sender, EventArgs e)
        {
            SendRemoteCommand("[to@wh] bootsel=369");
        }

        private void btnChangeBasePort_Click(object sender, EventArgs e)
        {
            int new_baseport = 0;

            if (int.TryParse(txtNewBasePort.Text, out new_baseport))
            {
                SendRemoteCommand("[to@wh] bip=" + new_baseport.ToString());    
            }
        }

        private void llblCopyToClickboardIP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(lblDetectedIP.Text);
        }

        private void llblCopyToClickboardPort_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(lblDetectedBasePort.Text);
        }

        private void PicoWHBroadcastListenerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //CloseForm();
            //e.Cancel = false;
        }
    }
}

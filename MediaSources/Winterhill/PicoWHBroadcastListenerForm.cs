using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Winterhill
{
    public partial class PicoWHBroadcastListenerForm : Form
    {

        private PicoWHBroadcastListener broadCastListener;
        public PicoWHBroadcastListenerForm()
        {
            InitializeComponent();
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
            broadCastListener = new PicoWHBroadcastListener();
            broadCastListener.OnBroadcast += BroadCastListener_OnBroadcast;
        }

        private void BroadCastListener_OnBroadcast(string data)
        {
            string[] message_data = data.Split('\n');

            for (int c = 0; c < message_data.Length; c++)
            {
                UpdateLB(lbBroadcast, message_data[c].Trim());
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            broadCastListener.Close();
            Close();
        }
    }
}

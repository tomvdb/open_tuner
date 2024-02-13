using opentuner.ExtraFeatures.MqttClient;
using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.Transmit
{



    public partial class PlutoControl : Form
    {

        public PlutoControl()
        {
            InitializeComponent();


        }

        private void PlutoControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }
    }
}

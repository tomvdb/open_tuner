using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.ExtraFeatures.QuickTuneControl
{
    public partial class QuickTuneControlSettingsForm : Form
    {
        private QuickTuneControlSettings _settings;

        public QuickTuneControlSettingsForm(ref QuickTuneControlSettings Settings)
        {
            _settings = Settings;
            InitializeComponent();

            txtUdp1.Text = _settings.UDPListenPorts[0].ToString();
            txtUdp2.Text = _settings.UDPListenPorts[1].ToString();
            txtUdp3.Text = _settings.UDPListenPorts[2].ToString();
            txtUdp4.Text = _settings.UDPListenPorts[3].ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int udp1 = 0;
            int udp2 = 1;
            int udp3 = 2;
            int udp4 = 3;

            if (!int.TryParse(txtUdp1.Text, out udp1))
            {
                MessageBox.Show("UDP 1 Port is invalid");
                return;
            }

            if (!int.TryParse(txtUdp2.Text, out udp2))
            {
                MessageBox.Show("UDP 2 Port is invalid");
                return;
            }

            if (!int.TryParse(txtUdp3.Text, out udp3))
            {
                MessageBox.Show("UDP 3 Port is invalid");
                return;
            }

            if (!int.TryParse(txtUdp4.Text, out udp4))
            {
                MessageBox.Show("UDP 4 Port is invalid");
                return;
            }

            _settings.UDPListenPorts[0] = udp1;
            _settings.UDPListenPorts[1] = udp2;
            _settings.UDPListenPorts[2] = udp3;
            _settings.UDPListenPorts[3] = udp4;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

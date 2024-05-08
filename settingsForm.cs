using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public partial class settingsForm : Form
    {

        MainSettings _settings;
        public settingsForm(ref MainSettings settings)
        {
            InitializeComponent();

            _settings = settings;

            // load settings
            comboDefaultSource.SelectedIndex = _settings.default_source;
            comboMediaPlayer1.SelectedIndex = _settings.mediaplayer_preferences[0];
            comboMediaPlayer2.SelectedIndex = _settings.mediaplayer_preferences[1];
            comboMediaPlayer3.SelectedIndex = _settings.mediaplayer_preferences[2];
            comboMediaPlayer4.SelectedIndex = _settings.mediaplayer_preferences[3];

            checkWindowed1.Checked = _settings.mediaplayer_windowed[0];
            checkWindowed2.Checked = _settings.mediaplayer_windowed[1];
            checkWindowed3.Checked = _settings.mediaplayer_windowed[2];
            checkWindowed4.Checked = _settings.mediaplayer_windowed[3];

            txtStreaming1IP.Text = _settings.streamer_udp_hosts[0];
            txtStreaming2IP.Text = _settings.streamer_udp_hosts[1];
            txtStreaming3IP.Text = _settings.streamer_udp_hosts[2];
            txtStreaming4IP.Text = _settings.streamer_udp_hosts[3];

            txtStreaming1Port.Text = _settings.streamer_udp_ports[0].ToString();
            txtStreaming2Port.Text = _settings.streamer_udp_ports[1].ToString();
            txtStreaming3Port.Text = _settings.streamer_udp_ports[2].ToString();
            txtStreaming4Port.Text = _settings.streamer_udp_ports[3].ToString();

            txtSnapshotPath.Text = _settings.media_path;

            checkBoxMuted.Checked = _settings.mute_at_startup;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // save
            int streamingPort1 = 0;
            int streamingPort2 = 0;
            int streamingPort3 = 0;
            int streamingPort4 = 0;

            if (!int.TryParse(txtStreaming1Port.Text, out streamingPort1))
            {
                MessageBox.Show("Streaming Port 1 is invalid");
                return;
            }

            if (!int.TryParse(txtStreaming2Port.Text, out streamingPort2))
            {
                MessageBox.Show("Streaming Port 2 is invalid");
                return;
            }

            if (!int.TryParse(txtStreaming3Port.Text, out streamingPort3))
            {
                MessageBox.Show("Streaming Port 3 is invalid");
                return;
            }

            if (!int.TryParse(txtStreaming4Port.Text, out streamingPort4))
            {
                MessageBox.Show("Streaming Port 4 is invalid");
                return;
            }

            _settings.streamer_udp_ports[0]= streamingPort1;
            _settings.streamer_udp_ports[1] = streamingPort2;
            _settings.streamer_udp_ports[2] = streamingPort3;
            _settings.streamer_udp_ports[3] = streamingPort4;

            _settings.streamer_udp_hosts[0] = txtStreaming1IP.Text;
            _settings.streamer_udp_hosts[1] = txtStreaming2IP.Text;
            _settings.streamer_udp_hosts[2] = txtStreaming3IP.Text;
            _settings.streamer_udp_hosts[3] = txtStreaming4IP.Text;

            _settings.mediaplayer_preferences[0] = comboMediaPlayer1.SelectedIndex;
            _settings.mediaplayer_preferences[1] = comboMediaPlayer2.SelectedIndex;
            _settings.mediaplayer_preferences[2] = comboMediaPlayer3.SelectedIndex;
            _settings.mediaplayer_preferences[3] = comboMediaPlayer4.SelectedIndex;

            _settings.mediaplayer_windowed[0] = checkWindowed1.Checked;
            _settings.mediaplayer_windowed[1] = checkWindowed2.Checked;
            _settings.mediaplayer_windowed[2] = checkWindowed3.Checked;
            _settings.mediaplayer_windowed[3] = checkWindowed4.Checked;

            _settings.media_path = txtSnapshotPath.Text;

            _settings.default_source = comboDefaultSource.SelectedIndex;

            _settings.mute_at_startup = checkBoxMuted.Checked;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = txtSnapshotPath.Text;

            if (fbd.ShowDialog() == DialogResult.OK )
            {
                txtSnapshotPath.Text = fbd.SelectedPath + "\\";
            }
        }
    }
}

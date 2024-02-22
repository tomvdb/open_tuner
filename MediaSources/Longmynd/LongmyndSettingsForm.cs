using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Longmynd
{
    public partial class LongmyndSettingsForm : Form
    {

        private LongmyndSettings _settings;
        public LongmyndSettingsForm(ref LongmyndSettings Settings)
        {
            InitializeComponent();
            _settings = Settings;

            comboHardwareInterface.SelectedIndex = _settings.DefaultInterface;
            txtWSIpAddress.Text = _settings.LongmyndWSHost;
            txtWSPort.Text = _settings.LongmyndWSPort.ToString();
            txtMqttIpAddress.Text = _settings.LongmyndMqttHost;
            txtMqttPort.Text = _settings.LongmyndMqttPort.ToString();
            txtBaseCmdTopic.Text = _settings.CmdTopic;
            txtTuner1FreqOffset.Text = _settings.Offset1.ToString();
            txtTSPort.Text = _settings.TS_Port.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            uint offset = 0;
            int wsport = 0;
            int mqttport = 0;
            int tsport = 0;

            if (!uint.TryParse(txtTuner1FreqOffset.Text, out offset))
            {
                MessageBox.Show("Invalid Offset");
                return;
            }

            if (!int.TryParse(txtWSPort.Text, out wsport))
            {
                MessageBox.Show("Invalid WS Port");
                return;
            }

            if (!int.TryParse(txtMqttPort.Text, out mqttport))
            {
                MessageBox.Show("Invalid Mqtt Port");
                return;
            }

            if (!int.TryParse(txtTSPort.Text, out tsport))
            {
                MessageBox.Show("Invalid TS Port");
                return;
            }

            _settings.DefaultInterface = (byte)comboHardwareInterface.SelectedIndex;
            _settings.TS_Port = tsport;
            _settings.LongmyndWSHost = txtWSIpAddress.Text;
            _settings.LongmyndWSPort = wsport;
            _settings.LongmyndMqttHost = txtMqttIpAddress.Text;
            _settings.LongmyndMqttPort  = mqttport;
            _settings.Offset1 = offset;
            _settings.CmdTopic = txtBaseCmdTopic.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

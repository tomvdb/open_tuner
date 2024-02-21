using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.ExtraFeatures.MqttClient
{
    public partial class MqttSettingsForm : Form
    {
        private MqttManagerSettings _settings;

        public MqttSettingsForm(ref MqttManagerSettings Settings)
        {
            this._settings = Settings;
            InitializeComponent();

            txtBrokerIp.Text = _settings.MqttBroker;
            txtBrokerPort.Text = _settings.MqttPort.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int mqttPort = 0;

            if (!int.TryParse(txtBrokerPort.Text, out mqttPort))
            {
                MessageBox.Show("Mqtt Port value is invalid!");
                return;
            }

            _settings.MqttBroker = txtBrokerIp.Text;
            _settings.MqttPort = mqttPort;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

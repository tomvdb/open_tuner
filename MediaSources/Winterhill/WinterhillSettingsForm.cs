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
    public partial class WinterhillSettingsForm : Form
    {
        private WinterhillSettings _settings;

        public WinterhillSettingsForm(WinterhillSettings Settings)
        {
            InitializeComponent();

            _settings = Settings;

            txtWHWSIp.Text = _settings.WinterhillWSHost;
            txtWHWSPort.Text = _settings.WinterhillWSPort.ToString();
            txtWHWSBaseUdp.Text = _settings.WinterhillUdpBasePort.ToString();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int wsport = 0;
            int baseport = 0;

            if (!int.TryParse(txtWHWSPort.Text, out wsport))
            {
                MessageBox.Show("Winterhill WS Port is not valid.");
                return;
            }

            if (!int.TryParse(txtWHWSBaseUdp.Text, out baseport))
            {
                MessageBox.Show("Winterhill WS Base Port is not valid");
                return;
            }

            _settings.WinterhillWSHost = txtWHWSIp.Text;
            _settings.WinterhillWSPort = wsport;
            _settings.WinterhillUdpBasePort = baseport;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

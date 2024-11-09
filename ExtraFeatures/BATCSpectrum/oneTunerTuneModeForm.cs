using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.ExtraFeatures.BATCSpectrum
{
    public partial class oneTunerTuneModeForm : Form
    {
        private int tuneMode = 1;
        private bool avoidBeacon = true;

        public oneTunerTuneModeForm(int _tuner, int _tuneMode, bool _avoidBeacon)
        {
            tuneMode = _tuneMode;
            avoidBeacon = _avoidBeacon;
            InitializeComponent();

            tuneMode1.SelectedIndex = tuneMode;
            avoidBeacon1.Checked = avoidBeacon;
            label1.Text = "RX " + _tuner.ToString() + ":";
        }

        public int getTuneMode()
        {
            return tuneMode;
        }

        public bool getAvoidBeacon()
        {
            return avoidBeacon;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            tuneMode = tuneMode1.SelectedIndex;
            avoidBeacon = avoidBeacon1.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

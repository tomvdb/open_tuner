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

            switch (tuneMode)
            {
                case 0:
                    radioButton1.Checked = true;
                    break;
                case 1:
                    radioButton2.Checked = true;
                    break;
                case 2:
                    radioButton3.Checked = true;
                    break;
                case 3:
                    radioButton4.Checked = true;
                    break;
            }
            //tuneMode1.SelectedIndex = tuneMode;
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
            if (radioButton1.Checked) { tuneMode = 0; }
            if (radioButton2.Checked) { tuneMode = 1; }
            if (radioButton3.Checked) { tuneMode = 2; }
            if (radioButton4.Checked) { tuneMode = 3; }
            avoidBeacon = avoidBeacon1.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                avoidBeacon1.Visible = false;
                avoidBeacon1.Checked = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                avoidBeacon1.Visible = false;
                avoidBeacon1.Checked = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                avoidBeacon1.Visible = false;
                avoidBeacon1.Checked = true;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                avoidBeacon1.Visible = true;
                avoidBeacon1.Checked = avoidBeacon;
            }
        }
    }
}

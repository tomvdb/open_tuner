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
    public partial class BATCSpectrumSettingsForm : Form
    {
        private BATCSpectrumSettings spectrumSettings;

        public BATCSpectrumSettingsForm(ref BATCSpectrumSettings _spectrumSettings)
        {
            spectrumSettings = _spectrumSettings;
            InitializeComponent();

            tuneMode1.SelectedIndex = spectrumSettings.tuneMode[0];
            tuneMode2.SelectedIndex = spectrumSettings.tuneMode[1];
            tuneMode3.SelectedIndex = spectrumSettings.tuneMode[2];
            tuneMode4.SelectedIndex = spectrumSettings.tuneMode[3];

            avoidBeacon1.Checked = spectrumSettings.avoidBeacon[0];
            avoidBeacon2.Checked = spectrumSettings.avoidBeacon[1];
            avoidBeacon3.Checked = spectrumSettings.avoidBeacon[2];
            avoidBeacon4.Checked = spectrumSettings.avoidBeacon[3];

            overPowerIndicatorLayout.SelectedIndex = spectrumSettings.overPowerIndicatorLayout;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            spectrumSettings.tuneMode[0] = tuneMode1.SelectedIndex;
            spectrumSettings.tuneMode[1] = tuneMode2.SelectedIndex;
            spectrumSettings.tuneMode[2] = tuneMode3.SelectedIndex;
            spectrumSettings.tuneMode[3] = tuneMode4.SelectedIndex;

            spectrumSettings.avoidBeacon[0] = avoidBeacon1.Checked;
            spectrumSettings.avoidBeacon[1] = avoidBeacon2.Checked;
            spectrumSettings.avoidBeacon[2] = avoidBeacon3.Checked;
            spectrumSettings.avoidBeacon[3] = avoidBeacon4.Checked;

            spectrumSettings.overPowerIndicatorLayout = overPowerIndicatorLayout.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

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
    public partial class tuneModeSettingsForm : Form
    {
        private tuneModeSettings tuneModeSettings;

        public tuneModeSettingsForm(ref tuneModeSettings _tuneModeSettings)
        {
            tuneModeSettings = _tuneModeSettings;
            InitializeComponent();

            tuneMode1.SelectedIndex = tuneModeSettings.tuneMode[0];
            tuneMode2.SelectedIndex = tuneModeSettings.tuneMode[1];
            tuneMode3.SelectedIndex = tuneModeSettings.tuneMode[2];
            tuneMode4.SelectedIndex = tuneModeSettings.tuneMode[3];

            avoidBeacon1.Checked = tuneModeSettings.avoidBeacon[0];
            avoidBeacon2.Checked = tuneModeSettings.avoidBeacon[1];
            avoidBeacon3.Checked = tuneModeSettings.avoidBeacon[2];
            avoidBeacon4.Checked = tuneModeSettings.avoidBeacon[3];

            overPowerIndicatorLayout.SelectedIndex = tuneModeSettings.overPowerIndicatorLayout;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            tuneModeSettings.tuneMode[0] = tuneMode1.SelectedIndex;
            tuneModeSettings.tuneMode[1] = tuneMode2.SelectedIndex;
            tuneModeSettings.tuneMode[2] = tuneMode3.SelectedIndex;
            tuneModeSettings.tuneMode[3] = tuneMode4.SelectedIndex;

            tuneModeSettings.avoidBeacon[0] = avoidBeacon1.Checked;
            tuneModeSettings.avoidBeacon[1] = avoidBeacon2.Checked;
            tuneModeSettings.avoidBeacon[2] = avoidBeacon3.Checked;
            tuneModeSettings.avoidBeacon[3] = avoidBeacon4.Checked;

            tuneModeSettings.overPowerIndicatorLayout = overPowerIndicatorLayout.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

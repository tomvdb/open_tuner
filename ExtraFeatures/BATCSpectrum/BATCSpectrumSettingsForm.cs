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

            treshHold.Value = Convert.ToDecimal(spectrumSettings.treshHold);
            autoHoldTimeValue.Value = Convert.ToDecimal(spectrumSettings.autoHoldTimeValue);
            autoTuneTimeValue.Value = Convert.ToDecimal(spectrumSettings.autoTuneTimeValue);

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

            spectrumSettings.treshHold = Convert.ToSingle(treshHold.Value);
            spectrumSettings.autoHoldTimeValue = Convert.ToInt32(autoHoldTimeValue.Value);
            spectrumSettings.autoTuneTimeValue = Convert.ToInt32(autoTuneTimeValue.Value);

            spectrumSettings.avoidBeacon[0] = avoidBeacon1.Checked;
            spectrumSettings.avoidBeacon[1] = avoidBeacon2.Checked;
            spectrumSettings.avoidBeacon[2] = avoidBeacon3.Checked;
            spectrumSettings.avoidBeacon[3] = avoidBeacon4.Checked;

            spectrumSettings.overPowerIndicatorLayout = overPowerIndicatorLayout.SelectedIndex;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void tuneMode1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tuneMode1.SelectedIndex < 3)
            {
                avoidBeacon1.Visible = false;
                if (tuneMode1.SelectedIndex == 0)   // "manual" mode
                {
                    avoidBeacon1.Checked = false;
                }
                else
                {
                    avoidBeacon1.Checked = true;
                }
            }
            else
            {
                avoidBeacon1.Visible = true;
            }
        }

        private void tuneMode2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tuneMode2.SelectedIndex < 3)
            {
                avoidBeacon2.Visible = false;
                if (tuneMode2.SelectedIndex == 0)   // "manual" mode
                {
                    avoidBeacon2.Checked = false;
                }
                else
                {
                    avoidBeacon2.Checked = true;
                }
            }
            else
            {
                avoidBeacon2.Visible = true;
            }
        }

        private void tuneMode3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tuneMode3.SelectedIndex < 3)
            {
                avoidBeacon3.Visible = false;
                if (tuneMode3.SelectedIndex == 0)   // "manual" mode
                {
                    avoidBeacon3.Checked = false;
                }
                else
                {
                    avoidBeacon3.Checked = true;
                }
            }
            else
            {
                avoidBeacon3.Visible = true;
            }
        }

        private void tuneMode4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tuneMode4.SelectedIndex < 3)
            {
                avoidBeacon4.Visible = false;
                if (tuneMode4.SelectedIndex == 0)   // "manual" mode
                {
                    avoidBeacon4.Checked = false;
                }
                else
                {
                    avoidBeacon4.Checked = true;
                }
            }
            else
            {
                avoidBeacon4.Visible = true;
            }
        }
    }
}

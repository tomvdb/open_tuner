using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace opentuner.ExtraFeatures.DATVReporter
{
    public partial class DATVReporterSettingsForm : Form
    {
        public DATVReporterSettingsForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtCallsign.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Callsign can't be empty");
                return;
            }

            if (txtGridLocator.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Grid Locator can't be empty");
                return;
            }

            if (txtServiceUrl.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Service URL can't be empty");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

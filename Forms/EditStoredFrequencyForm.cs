using System;
using System.Windows.Forms;

namespace opentuner.Forms
{
    public partial class EditStoredFrequencyForm : Form
    {
        public EditStoredFrequencyForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // do some validation
            if (comboDefaultTuner.SelectedIndex < 0)
            {
                MessageBox.Show("You need to select a default Tuner");
                return;
            }

            if (comboRFInput.SelectedIndex < 0)
            {
                MessageBox.Show("You need to select a RF Input");
                return;
            }
            

            if (txtName.Text.Length == 0)
            {
                MessageBox.Show("You need to specify a name for this stored frequency");
                return;
            }

            if (txtFreq.Text.Length == 0)
            {
                MessageBox.Show("You need to specify a frequency for this stored frequency");
                return;
            }

            if (txtOffset.Text.Length == 0)
            {
                MessageBox.Show("You need to specify an offset for this stored frequency (0 is fine)");
                return;
            }

            if (txtSR.Text.Length == 0)
            {
                MessageBox.Show("You need to specify a Symbol Rate for this stored frequency");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

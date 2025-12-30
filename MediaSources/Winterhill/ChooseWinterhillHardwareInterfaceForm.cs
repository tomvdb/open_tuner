using System;
using System.Windows.Forms;

namespace opentuner.MediaSources.WinterHill
{
    public partial class ChooseWinterHillHardwareInterfaceForm : Form
    {
        public ChooseWinterHillHardwareInterfaceForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ChooseWinterHillHardwareInterfaceForm_Load(object sender, EventArgs e)
        {
            comboHardwareInterface.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.Cancel;
        }
    }
}

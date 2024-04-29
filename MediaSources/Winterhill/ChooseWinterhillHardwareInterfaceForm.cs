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
    public partial class ChooseWinterhillHardwareInterfaceForm : Form
    {
        public ChooseWinterhillHardwareInterfaceForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ChooseWinterhillHardwareInterfaceForm_Load(object sender, EventArgs e)
        {
            comboHardwareInterface.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.Cancel;
        }
    }
}

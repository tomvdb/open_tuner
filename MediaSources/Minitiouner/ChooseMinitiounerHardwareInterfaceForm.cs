using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Minitiouner
{
    public partial class ChooseMinitiounerHardwareInterfaceForm : Form
    {
        public ChooseMinitiounerHardwareInterfaceForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult= DialogResult.OK;
        }

        private void ChooseHardwareInterfaceForm_Load(object sender, EventArgs e)
        {
            comboHardwareSelect.SelectedIndex = 0;
        }
    }
}

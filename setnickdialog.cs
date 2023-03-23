using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public partial class setnickdialog : Form
    {
        public setnickdialog()
        {
            InitializeComponent();
        }

        private void btnSetNick_Click(object sender, EventArgs e)
        {
            if (txtNick.Text.Length == 0)
            {
                MessageBox.Show("Your nick is too short...");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}

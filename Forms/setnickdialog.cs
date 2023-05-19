using System;
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

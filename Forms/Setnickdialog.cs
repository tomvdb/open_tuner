using System;
using System.Windows.Forms;

namespace opentuner.Forms
{
    public partial class Setnickdialog : Form
    {
        public Setnickdialog()
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

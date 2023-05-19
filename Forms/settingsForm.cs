using System;
using System.Drawing;
using System.Windows.Forms;

namespace opentuner
{
    public partial class settingsForm : Form
    {
        public Font currentChatFont;

        public settingsForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = txtSnapshotPath.Text;

            if (fbd.ShowDialog() == DialogResult.OK )
            {
                txtSnapshotPath.Text = fbd.SelectedPath + "\\";
            }
        }


    }
}

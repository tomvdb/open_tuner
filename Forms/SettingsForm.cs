using opentuner.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace opentuner.Forms
{
    public partial class SettingsForm : Form
    {
        public Font currentChatFont;

        public SettingsForm()
        {
            InitializeComponent();
            OTColorChanger.OTChangeControlColors(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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

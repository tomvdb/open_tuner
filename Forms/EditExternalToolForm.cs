using System;
using System.Windows.Forms;

namespace opentuner.Forms
{
    public partial class EditExternalToolForm : Form
    {
        public EditExternalToolForm()
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
            if (txtToolName.Text.Length == 0)
            {
                MessageBox.Show("You need to specify a tool name");
                return;
            }

            if (txtToolPath.Text.Length == 0)
            {
                MessageBox.Show("You need to specify a path to the tool executeable");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Executable|*.exe";
            ofd.CheckFileExists = true;
            ofd.AddExtension = true;
            ofd.DefaultExt = ".exe";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtToolPath.Text = ofd.FileName;    
            }
        }
    }
}

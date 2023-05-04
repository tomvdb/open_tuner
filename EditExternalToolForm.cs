using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public partial class EditExternalToolForm : Form
    {
        public EditExternalToolForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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

            this.DialogResult = DialogResult.OK;
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

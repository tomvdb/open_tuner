using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace opentuner
{
    public partial class externalToolsManager : Form
    {
        private List<ExternalTool> externalToolList;

        public externalToolsManager(List<ExternalTool> externalTools)
        {
            InitializeComponent();
            externalToolList = externalTools;
        }

        public void load_tools()
        {
            listTools.Items.Clear();

            lblToolName.Text = "";
            lblToolPath.Text = "";
            lblToolParameters.Text = "";
            lblEnableUDP1.Text = "";
            lblUDP2.Text = "";

            for (int c = 0; c < externalToolList.Count; c++)
            {
                listTools.Items.Add(externalToolList[c].ToolName);
            }

            if (listTools.Items.Count > 0)
                listTools.SelectedIndex = 0;
        }

        public void show_tool(int index)
        {
            if (index < externalToolList.Count)
            {
                lblToolName.Text = externalToolList[index].ToolName;
                lblToolPath.Text = externalToolList[index].ToolPath;
                lblToolParameters.Text = externalToolList[index].ToolParameters;
                lblEnableUDP1.Text = externalToolList[index].EnableUDP1.ToString();
                lblUDP2.Text = externalToolList[index].EnableUDP2.ToString();
            }
        }

        private void listTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            show_tool(listTools.SelectedIndex);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listTools.SelectedIndex > -1)
            {
                if (MessageBox.Show("Are you sure you want to delete '" + externalToolList[listTools.SelectedIndex].ToolName + "'?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    externalToolList.RemoveAt(listTools.SelectedIndex);
                    load_tools();
                }
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditExternalToolForm editExternalToolForm = new EditExternalToolForm();

            if (editExternalToolForm.ShowDialog() == DialogResult.OK)
            {
                ExternalTool et = new ExternalTool();
                et.ToolName = editExternalToolForm.txtToolName.Text;
                et.ToolPath = editExternalToolForm.txtToolPath.Text;
                et.ToolParameters = editExternalToolForm.txtToolParameters.Text;
                et.EnableUDP1 = editExternalToolForm.checkUdp1.Checked;
                et.EnableUDP2 = editExternalToolForm.checkUdp2.Checked;

                externalToolList.Add(et);
                load_tools();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void externalToolsManager_Load(object sender, EventArgs e)
        {
            load_tools();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listTools.SelectedIndex > -1)
            {
                int index = listTools.SelectedIndex;

                EditExternalToolForm editExternalToolForm = new EditExternalToolForm();

                editExternalToolForm.txtToolName.Text = externalToolList[index].ToolName;
                editExternalToolForm.txtToolPath.Text = externalToolList[index].ToolPath;
                editExternalToolForm.txtToolParameters.Text = externalToolList[index].ToolParameters;
                editExternalToolForm.checkUdp1.Checked = externalToolList[index].EnableUDP1;
                editExternalToolForm.checkUdp2.Checked = externalToolList[index].EnableUDP2;

                if (editExternalToolForm.ShowDialog() == DialogResult.OK)
                {

                    externalToolList[index].ToolName = editExternalToolForm.txtToolName.Text;
                    externalToolList[index].ToolPath = editExternalToolForm.txtToolPath.Text;
                    externalToolList[index].ToolParameters = editExternalToolForm.txtToolParameters.Text;
                    externalToolList[index].EnableUDP1 = editExternalToolForm.checkUdp1.Checked;
                    externalToolList[index].EnableUDP2 = editExternalToolForm.checkUdp2.Checked;

                    load_tools();
                }

            }
        }
    }
}

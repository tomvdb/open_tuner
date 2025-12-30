using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.ExtraFeatures.BATCWebchat
{
    public partial class WebChatSettngsForm : Form
    {
        WebChatSettings _settings;
        public WebChatSettngsForm(ref WebChatSettings Settings)
        {
            InitializeComponent();

            _settings = Settings;
            numChatFontSize.Value = _settings.chat_font_size;
            txtSigReportTemplate.Text = _settings.sigreport_template;
            txtNick.Text = _settings.nickname;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _settings.chat_font_size = (int)numChatFontSize.Value;
            _settings.sigreport_template = txtSigReportTemplate.Text;
            _settings.nickname = txtNick.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

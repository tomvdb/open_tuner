using System;
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
            checkAutoStart.Checked = _settings.gui_autostart;
            checkAutoLogin.Checked = _settings.gui_autologin;
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
            _settings.gui_autostart = checkAutoStart.Checked;
            _settings.gui_autologin = checkAutoLogin.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

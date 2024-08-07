﻿using SocketIOClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Windows.Interop;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Threading;
using Serilog;
using opentuner.ExtraFeatures.BATCWebchat;
using opentuner.MediaSources;

namespace opentuner
{
    public partial class WebChatForm : Form
    {
        static Font consoleFont; 
        static Font consoleFontBold;

        public string prop_title { set { this.Text = value; } }

        WebChatSettings _settings;
        OTSource _source;

        public WebChatForm(WebChatSettings Settings, OTSource Source)
        {
            InitializeComponent();

            _settings = Settings;
            _source = Source;
            consoleFont = new Font("Consolas", _settings.chat_font_size);
            consoleFontBold = new Font("Consolas", _settings.chat_font_size + 1, FontStyle.Bold);
        }

        private SocketIO client = null;

        private void wbchat_Load(object sender, EventArgs e)
        {

            if (_settings.nickname.Length > 0)
            {
                txtNick.Text = _settings.nickname;
            }
            else
            {
                txtNick.Text = "NONICK";
            }

            client = new SocketIO("https://eshail.batc.org.uk/", new SocketIOOptions
            {
                Path = "/wb/chat/socket.io",
                Query = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("room", "eshail-wb"),
                    }
            });

            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;

            Action<SocketIOResponse> callbackHistory = new Action<SocketIOResponse>(onHistoryCallback);
            Action<SocketIOResponse> callbackMessage = new Action<SocketIOResponse>(onMessageCallback);
            Action<SocketIOResponse> callbackNicks = new Action<SocketIOResponse>(onNicksCallback);
            Action<SocketIOResponse> callbackViewers = new Action<SocketIOResponse>(onViewersCallback);

            client.On("history", callbackHistory);
            client.On("message", callbackMessage);
            client.On("nicks", callbackNicks);
            client.On("viewers", callbackViewers);

            lbUsers.Font = consoleFontBold;
            lbUsers.ForeColor = Color.FromArgb(204, 204, 204);
            txtMessage.BackColor = Color.FromArgb(63, 70, 76);
            txtMessage.ForeColor = Color.FromArgb(204, 204, 204);
            txtMessage.Font = consoleFontBold;

            AddChat(richChat, "", "", "Connecting...");
            client.ConnectAsync();

            if (_source.GetVideoSourceCount() > 0)
                btnSigReportTuner1.Enabled = true;
            if (_source.GetVideoSourceCount() > 1)
                btnSigReportTuner2.Enabled = true;
            if (_source.GetVideoSourceCount() > 2)
            {
                btnSigReportTuner3.Enabled = true;
                btnSigReportTuner4.Enabled = true;
            }
        }

        private void Client_OnDisconnected(object sender, string e)
        {
            lblConnected.Text = "Connected: False";
        }

        private delegate void UpdateLBDelegate(System.Windows.Forms.ListBox LB, Object obj);

        public static void AddItem(System.Windows.Forms.ListBox LB, Object obj)
        {
            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(AddItem);
                
                    LB.Invoke(ulb, new object[] { LB, obj });
            }
            else
            {
                if (LB.Items.Count > 1000)
                {
                    LB.Items.Remove(0);
                }

                int i = LB.Items.Add(obj);
                LB.TopIndex = i;
            }
        }

        private delegate void UpdateFormTitle(WebChatForm frm, string new_title);

        public void updateTitle(WebChatForm frm, string new_title)
        {

            if (frm.InvokeRequired)
            {
                UpdateFormTitle ulb = new UpdateFormTitle(updateTitle);

                    frm.Invoke(ulb, new object[] { frm, new_title });
            }
            else
            {
                frm.prop_title = new_title;
            }
        }

        private delegate void UpdateRTBDelegate(RichTextBox LB, string tstr, string nick, string msg);

        public static void AddChat(RichTextBox rtb, string tstr, string nick, string msg)
        {
            if (rtb.InvokeRequired)
            {
                UpdateRTBDelegate ulb = new UpdateRTBDelegate(AddChat);
                
                rtb.Invoke(ulb, new object[] { rtb, tstr, nick, msg });
            }
            else
            {

                // 204, 204, 204
                rtb.SelectionStart = rtb.TextLength;
                rtb.ScrollToCaret();
                rtb.SelectionFont = consoleFont;
                rtb.SelectionColor = Color.FromArgb(204, 204, 204);
                rtb.SelectionStart = rtb.TextLength;
                rtb.AppendText(tstr);


                rtb.SelectionFont = consoleFontBold;
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor = Color.FromArgb(251, 222, 45);
                rtb.AppendText(" <" + nick + "> ");

                rtb.SelectionFont = consoleFont;
                rtb.SelectionColor = Color.FromArgb(204, 204, 204);
                rtb.SelectionStart = rtb.TextLength;
                rtb.AppendText(msg + "\n");
                rtb.SelectionStart = rtb.TextLength;
                rtb.ScrollToCaret();
            }
        }

        private delegate void ClearRTBDelegate(RichTextBox rtb);
        public static void ClearChat(RichTextBox rtb)
        {
            if (rtb.InvokeRequired)
            {
                ClearRTBDelegate crd = new ClearRTBDelegate(ClearChat);
                    rtb.Invoke(crd, new Object[] { rtb });
            }
            else
            {
                rtb.Clear();
            }
        }

        public static void ClearAll(System.Windows.Forms.ListBox LB, Object obj)
        {
            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(ClearAll);
                LB.Invoke(ulb, new object[] { LB, obj });
            }
            else
            {
                LB.Items.Clear();
            }
        }


        private void initUsers(SocketIOResponse response)
        {
            ClearAll(lbUsers, "");

            var nicks = response.GetValue(0).GetProperty("nicks").EnumerateArray();

            foreach (System.Text.Json.JsonElement nick in nicks)
            {
                AddItem(lbUsers, nick.ToString());
            }
        }

        private void initHistory(SocketIOResponse response)
        {
            //ClearAll(lbChat, "");
            ClearChat(richChat);

            var history = response.GetValue(0).GetProperty("history").EnumerateArray();

            foreach (System.Text.Json.JsonElement hist_item in history)
            {
                string time = hist_item.GetProperty("time").ToString();
                DateTime timeobj = Convert.ToDateTime(time);

                //string historymsg = timeobj.ToString("HH:mm") + " <" + hist_item.GetProperty("name").ToString() + ">" + " " + hist_item.GetProperty("message").ToString();
                //AddItem(lbChat, historymsg);
                AddChat(richChat, timeobj.ToString("HH:mm"), hist_item.GetProperty("name").ToString(), hist_item.GetProperty("message").ToString());
            }
        }

        private void onViewersCallback(SocketIOResponse response)
        {
            updateTitle(this, "QO-100 Wideband Chat - Viewers: " + response.GetValue(0).GetProperty("num").ToString());
        }

        private void onMessageCallback(SocketIOResponse response)
        {
            var newMessage = response.GetValue(0);
            string time = newMessage.GetProperty("time").ToString();
            DateTime timeobj = Convert.ToDateTime(time);
            //string newMsg = timeobj.ToString("HH:mm") + " <" + newMessage.GetProperty("name").ToString() + ">" + " " + newMessage.GetProperty("message").ToString();
            //AddItem(lbChat, newMsg);

            AddChat(richChat, timeobj.ToString("HH:mm"), newMessage.GetProperty("name").ToString(), newMessage.GetProperty("message").ToString());
        }

        private void onNicksCallback(SocketIOResponse response)
        {
            initUsers(response);
        }

        private void onHistoryCallback(SocketIOResponse response)
        {
            initUsers(response);
            initHistory(response);
        }
        class nickInfo
        {
            [JsonPropertyName("nick")]
            public string nick { get; set; }
        }

        class chatMessage
        {
            [JsonPropertyName("message")]
            public string message { get; set; }
        }

        private void Client_OnConnected(object sender, EventArgs e)
        {
            Log.Information("Connected socketio");
            lblConnected.Text = "Connected: True";
        }

        private void setNick()
        {
            if (client.Connected)
            {
                string nick = txtNick.Text.Trim();

                if (nick.Length > 0)
                {
                    client.EmitAsync("setnick", new nickInfo { nick = nick });
                    txtMessage.Enabled = true;
                    AddItem(lbChat, "*** Your nick is set to " + nick + " ***");
                    _settings.nickname = nick;
                }
            }
        }

        private void btnSetNick_Click(object sender, EventArgs e)
        {
            setNick();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void sendMessage()
        {
            if (client.Connected)
            {
                string msg = txtMessage.Text.Trim();

                if (msg.Length > 0)
                {
                    client.EmitAsync("message", new chatMessage { message = msg });
                }

                txtMessage.Text = "";
            }

        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                sendMessage();
            }
        }

        private void wbchat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void lbUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (txtMessage.Enabled && lbUsers.SelectedIndex >= 0)
            {
                string selectedName = lbUsers.SelectedItem.ToString();
                txtMessage.Text = txtMessage.Text + " @" + selectedName + " ";
            }
        }

        private void checkStayOnTop_CheckedChanged(object sender, EventArgs e)
        {
            
            if ( checkStayOnTop.Checked )
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
           
        }

        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richChat.SelectedText, TextDataFormat.UnicodeText);
        }

        private void txtNick_Click(object sender, EventArgs e)
        {
            setnickdialog nickDialog = new setnickdialog();

            nickDialog.txtNick.Text = txtNick.Text;

            _settings.nickname = txtNick.Text;

            if (nickDialog.ShowDialog() == DialogResult.OK)
            {
                txtNick.Text = nickDialog.txtNick.Text;
                setNick();

                DateTime timeobj = DateTime.Now;
                AddChat(richChat, timeobj.ToString("HH:mm"), "Chat","You are now known as '" + txtNick.Text + "'");

            }
        }




        private void lbChat_Resize(object sender, EventArgs e)
        {
        }

        private void richChat_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to follow this link?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
        }

        private void lbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbUsers.SelectedIndex > 0)
            {
                // from Chris - DH3CS
                string user = lbUsers.GetItemText(lbUsers.SelectedItem);

                try
                {
                    richChat.SelectionStart = 0;
                    richChat.SelectionLength = richChat.Text.Length - 1;
                    richChat.SelectionBackColor = Color.FromArgb(63, 70, 76);
                    // set the current caret position to the end, if nothing will be found
                    richChat.SelectionStart = richChat.Text.Length;

                    foreach (Match match in Regex.Matches(richChat.Text, user))
                    {
                        richChat.SelectionStart = match.Index;
                        richChat.SelectionLength = match.Length;
                        richChat.SelectionBackColor = Color.FromArgb(0, 255, 0);
                    }
                    // scroll it automatically
                    richChat.ScrollToCaret();
                }
                catch { }
            }
            else
            {
                richChat.SelectionStart = 0;
                richChat.SelectionLength = richChat.Text.Length - 1;
                richChat.SelectionBackColor = Color.FromArgb(63, 70, 76);
                richChat.SelectionStart = richChat.Text.Length;
                richChat.ScrollToCaret();
            }
        }

        private void lbUsers_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void lbUsers_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { lbUsers.SelectedIndex = -1; }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtMessage.Text = Clipboard.GetText();
        }

        private void getSignalReportData(int tuner)
        {
            var data = _source.GetSignalData(tuner);

            //string signalReport = "SigReport: " + lblServiceName.Text.ToString() + "/" + lblServiceProvider.Text.ToString() + " - " + lbldbMargin.Text.ToString() + " (" + lblMer.Text.ToString() + ") - " + lblSR.Text.ToString() + "" + " - " + (freq).ToString() + " ";
            string signalReport = _settings.sigreport_template.ToString();

            // SigReport: {SN}/{SP} - {DBM} - ({MER}) - {SR} - {FREQ}

            signalReport = signalReport.Replace("{SN}", data["ServiceName"]);
            signalReport = signalReport.Replace("{SP}", data["ServiceProvider"]);
            signalReport = signalReport.Replace("{DBM}", data["dbMargin"]);
            signalReport = signalReport.Replace("{MER}", data["Mer"] + " dB");
            signalReport = signalReport.Replace("{SR}", data["SR"] + "");
            signalReport = signalReport.Replace("{FREQ}", data["Frequency"] + "");

            txtMessage.Text = signalReport;

            Clipboard.SetText(signalReport);
        }

        private void btnSigReportTuner1_Click(object sender, EventArgs e)
        {
            getSignalReportData(0);
        }

        private void btnSigReportTuner2_Click(object sender, EventArgs e)
        {
            getSignalReportData(1);
        }

        private void btnSigReportTuner3_Click(object sender, EventArgs e)
        {
            getSignalReportData(2);
        }

        private void btnSigReportTuner4_Click(object sender, EventArgs e)
        {
            getSignalReportData(3);
        }
    }
}

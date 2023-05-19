using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace opentuner
{
    public partial class wbchat : Form
    {

        static Font consoleFont; 
        static Font consoleFontBold;

        public string prop_title { set { this.Text = value; } }

        public wbchat(int fontSize)
        {
            InitializeComponent();
            consoleFont = new Font("Consolas", fontSize);
            consoleFontBold = new Font("Consolas", fontSize + 1, FontStyle.Bold);
        }

        private SocketIO client = null;

        private void wbchat_Load(object sender, EventArgs e)
        {

            if (Properties.Settings.Default.chat_nick.Length > 0)
            {
                txtNick.Text = Properties.Settings.Default.chat_nick;
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

        private delegate void UpdateFormTitle(wbchat frm, string new_title);

        public void updateTitle(wbchat frm, string new_title)
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
                    Properties.Settings.Default.chat_nick = nick;
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
            /*
            if ( checkStayOnTop.Checked )
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
            */
        }

        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string copy = "";

            for (int c = 0; c < lbChat.SelectedItems.Count; c++)
            {
                copy += lbChat.SelectedItems[c].ToString() + "\n";
            }

            Clipboard.SetText(copy);
        }

        private void txtNick_Click(object sender, EventArgs e)
        {
            setnickdialog nickDialog = new setnickdialog();

            nickDialog.txtNick.Text = txtNick.Text;

            if (nickDialog.ShowDialog() == DialogResult.OK)
            {
                txtNick.Text = nickDialog.txtNick.Text;
                setNick();

                DateTime timeobj = DateTime.Now;
                AddChat(richChat, timeobj.ToString("HH:mm"), "Chat","You are now known as '" + txtNick.Text + "'");

            }
        }

        /*
        private void lst_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(lbChat.Items[e.Index].ToString(), lbChat.Font, lbChat.Width).Height;
        }

        private void lst_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(lbChat.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
        }
        */


        private void lbChat_Resize(object sender, EventArgs e)
        {
            //lbChat.Refresh();
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
                string result = null;
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
    }
}

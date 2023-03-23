using SocketIOClient;
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

namespace opentuner
{
    public partial class wbchat : Form
    {
        public wbchat()
        {
            InitializeComponent();
        }
        private SocketIO client = null;

        private void wbchat_Load(object sender, EventArgs e)
        {
            //lbChat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            //lbChat.MeasureItem += lst_MeasureItem;
            //lbChat.DrawItem += lst_DrawItem;

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
            AddItem(lbChat, "Connecting ...");
            client.ConnectAsync();
        }

        private void Client_OnDisconnected(object sender, string e)
        {
            lblConnected.Text = "Connected: False";
        }

        private delegate void UpdateLBDelegate(ListBox LB, Object obj);

        public static void AddItem(ListBox LB, Object obj)
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

        public static void ClearAll(ListBox LB, Object obj)
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
            ClearAll(lbChat, "");

            var history = response.GetValue(0).GetProperty("history").EnumerateArray();

            foreach (System.Text.Json.JsonElement hist_item in history)
            {
                string time = hist_item.GetProperty("time").ToString();

                DateTime timeobj = Convert.ToDateTime(time);
                string historymsg = timeobj.ToString("HH:mm") + " <" + hist_item.GetProperty("name").ToString() + ">" + " " + hist_item.GetProperty("message").ToString();
                AddItem(lbChat, historymsg);
            }
        }

        private void onViewersCallback(SocketIOResponse response)
        {
            lblViewers.Text = "Viewers: " + response.GetValue(0).GetProperty("num").ToString();
        }

        private void onMessageCallback(SocketIOResponse response)
        {
            var newMessage = response.GetValue(0);
            string time = newMessage.GetProperty("time").ToString();
            DateTime timeobj = Convert.ToDateTime(time);
            string newMsg = timeobj.ToString("HH:mm") + " <" + newMessage.GetProperty("name").ToString() + ">" + " " + newMessage.GetProperty("message").ToString();
            AddItem(lbChat, newMsg);
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
    }
}

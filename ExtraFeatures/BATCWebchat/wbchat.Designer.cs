namespace opentuner
{
    partial class WebChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtNick = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnected = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblViewers = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.richChat = new System.Windows.Forms.RichTextBox();
            this.lbChat = new System.Windows.Forms.ListBox();
            this.chatContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.textInputContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.chatContextMenuStrip.SuspendLayout();
            this.textInputContextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtNick});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtNick
            // 
            this.txtNick.IsLink = true;
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(118, 17);
            this.txtNick.Text = "toolStripStatusLabel2";
            this.txtNick.Click += new System.EventHandler(this.txtNick_Click);
            // 
            // lblConnected
            // 
            this.lblConnected.Name = "lblConnected";
            this.lblConnected.Size = new System.Drawing.Size(97, 17);
            this.lblConnected.Text = "Connected: False";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(18, 17);
            this.toolStripStatusLabel1.Text = " - ";
            // 
            // lblViewers
            // 
            this.lblViewers.Name = "lblViewers";
            this.lblViewers.Size = new System.Drawing.Size(62, 17);
            this.lblViewers.Text = "Viewers : 0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbUsers);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richChat);
            this.splitContainer1.Panel2.Controls.Add(this.lbChat);
            this.splitContainer1.Panel2.Controls.Add(this.txtMessage);
            this.splitContainer1.Size = new System.Drawing.Size(800, 428);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 1;
            // 
            // lbUsers
            // 
            this.lbUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(70)))), ((int)(((byte)(76)))));
            this.lbUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUsers.FormattingEnabled = true;
            this.lbUsers.IntegralHeight = false;
            this.lbUsers.ItemHeight = 15;
            this.lbUsers.Location = new System.Drawing.Point(0, 0);
            this.lbUsers.Name = "lbUsers";
            this.lbUsers.Size = new System.Drawing.Size(180, 428);
            this.lbUsers.Sorted = true;
            this.lbUsers.TabIndex = 0;
            this.lbUsers.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbUsers_MouseClick);
            this.lbUsers.SelectedIndexChanged += new System.EventHandler(this.lbUsers_SelectedIndexChanged);
            this.lbUsers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbUsers_MouseDoubleClick);
            this.lbUsers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbUsers_MouseDown);
            // 
            // richChat
            // 
            this.richChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(70)))), ((int)(((byte)(76)))));
            this.richChat.ContextMenuStrip = this.chatContextMenuStrip;
            this.richChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richChat.Location = new System.Drawing.Point(0, 0);
            this.richChat.Name = "richChat";
            this.richChat.ReadOnly = true;
            this.richChat.Size = new System.Drawing.Size(616, 408);
            this.richChat.TabIndex = 3;
            this.richChat.Text = "";
            this.richChat.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richChat_LinkClicked);
            // 
            // lbChat
            // 
            this.lbChat.ContextMenuStrip = this.chatContextMenuStrip;
            this.lbChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbChat.FormattingEnabled = true;
            this.lbChat.HorizontalScrollbar = true;
            this.lbChat.IntegralHeight = false;
            this.lbChat.Location = new System.Drawing.Point(0, 0);
            this.lbChat.Name = "lbChat";
            this.lbChat.ScrollAlwaysVisible = true;
            this.lbChat.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbChat.Size = new System.Drawing.Size(616, 408);
            this.lbChat.TabIndex = 1;
            this.lbChat.Resize += new System.EventHandler(this.lbChat_Resize);
            // 
            // chatContextMenuStrip
            // 
            this.chatContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedTextToolStripMenuItem});
            this.chatContextMenuStrip.Name = "contextMenuStrip1";
            this.chatContextMenuStrip.Size = new System.Drawing.Size(216, 26);
            // 
            // copySelectedTextToolStripMenuItem
            // 
            this.copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
            this.copySelectedTextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copySelectedTextToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.copySelectedTextToolStripMenuItem.Text = "Copy Selected Text";
            this.copySelectedTextToolStripMenuItem.Click += new System.EventHandler(this.copySelectedTextToolStripMenuItem_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.AcceptsReturn = true;
            this.txtMessage.ContextMenuStrip = this.textInputContextStrip;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Enabled = false;
            this.txtMessage.Location = new System.Drawing.Point(0, 408);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(616, 20);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // textInputContextStrip
            // 
            this.textInputContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem});
            this.textInputContextStrip.Name = "textInputContextStrip";
            this.textInputContextStrip.Size = new System.Drawing.Size(181, 48);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectAllToolStripMenuItem.Text = "&Paste";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // wbchat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "wbchat";
            this.ShowIcon = false;
            this.Text = "QO-100 Wideband Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.wbchat_FormClosing);
            this.Load += new System.EventHandler(this.wbchat_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.chatContextMenuStrip.ResumeLayout(false);
            this.textInputContextStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblConnected;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblViewers;
        public System.Windows.Forms.ListBox lbUsers;
        public System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip chatContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copySelectedTextToolStripMenuItem;
        public System.Windows.Forms.ToolStripStatusLabel txtNick;
        public System.Windows.Forms.ListBox lbChat;
        public System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.RichTextBox richChat;
        private System.Windows.Forms.ContextMenuStrip textInputContextStrip;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    }
}
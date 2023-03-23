namespace opentuner
{
    partial class wbchat
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
            this.lbChat = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.lbChat);
            this.splitContainer1.Panel2.Controls.Add(this.txtMessage);
            this.splitContainer1.Size = new System.Drawing.Size(800, 428);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 1;
            // 
            // lbUsers
            // 
            this.lbUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbUsers.FormattingEnabled = true;
            this.lbUsers.IntegralHeight = false;
            this.lbUsers.Location = new System.Drawing.Point(0, 0);
            this.lbUsers.Name = "lbUsers";
            this.lbUsers.Size = new System.Drawing.Size(180, 428);
            this.lbUsers.TabIndex = 0;
            this.lbUsers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbUsers_MouseDoubleClick);
            // 
            // lbChat
            // 
            this.lbChat.ContextMenuStrip = this.contextMenuStrip1;
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
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedTextToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(216, 26);
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
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Enabled = false;
            this.txtMessage.Location = new System.Drawing.Point(0, 408);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(616, 20);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
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
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copySelectedTextToolStripMenuItem;
        public System.Windows.Forms.ToolStripStatusLabel txtNick;
        public System.Windows.Forms.ListBox lbChat;
        public System.Windows.Forms.TextBox txtMessage;
    }
}
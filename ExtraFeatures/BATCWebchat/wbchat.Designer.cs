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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSigReportTuner4 = new System.Windows.Forms.Button();
            this.btnSigReportTuner3 = new System.Windows.Forms.Button();
            this.btnSigReportTuner2 = new System.Windows.Forms.Button();
            this.btnSigReportTuner1 = new System.Windows.Forms.Button();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.richChat = new System.Windows.Forms.RichTextBox();
            this.chatContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbChat = new System.Windows.Forms.ListBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.textInputContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.chatContextMenuStrip.SuspendLayout();
            this.textInputContextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtNick});
            this.statusStrip1.Location = new System.Drawing.Point(0, 480);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1073, 22);
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
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richChat);
            this.splitContainer1.Panel2.Controls.Add(this.lbChat);
            this.splitContainer1.Panel2.Controls.Add(this.txtMessage);
            this.splitContainer1.Size = new System.Drawing.Size(1073, 480);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSigReportTuner4);
            this.groupBox1.Controls.Add(this.btnSigReportTuner3);
            this.groupBox1.Controls.Add(this.btnSigReportTuner2);
            this.groupBox1.Controls.Add(this.btnSigReportTuner1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 340);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 140);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnSigReportTuner4
            // 
            this.btnSigReportTuner4.Enabled = false;
            this.btnSigReportTuner4.Location = new System.Drawing.Point(12, 106);
            this.btnSigReportTuner4.Name = "btnSigReportTuner4";
            this.btnSigReportTuner4.Size = new System.Drawing.Size(151, 23);
            this.btnSigReportTuner4.TabIndex = 3;
            this.btnSigReportTuner4.Text = "Signal Report Tuner 4";
            this.btnSigReportTuner4.UseVisualStyleBackColor = true;
            this.btnSigReportTuner4.Click += new System.EventHandler(this.btnSigReportTuner4_Click);
            // 
            // btnSigReportTuner3
            // 
            this.btnSigReportTuner3.Enabled = false;
            this.btnSigReportTuner3.Location = new System.Drawing.Point(12, 77);
            this.btnSigReportTuner3.Name = "btnSigReportTuner3";
            this.btnSigReportTuner3.Size = new System.Drawing.Size(151, 23);
            this.btnSigReportTuner3.TabIndex = 2;
            this.btnSigReportTuner3.Text = "Signal Report Tuner 3";
            this.btnSigReportTuner3.UseVisualStyleBackColor = true;
            this.btnSigReportTuner3.Click += new System.EventHandler(this.btnSigReportTuner3_Click);
            // 
            // btnSigReportTuner2
            // 
            this.btnSigReportTuner2.Enabled = false;
            this.btnSigReportTuner2.Location = new System.Drawing.Point(12, 48);
            this.btnSigReportTuner2.Name = "btnSigReportTuner2";
            this.btnSigReportTuner2.Size = new System.Drawing.Size(151, 23);
            this.btnSigReportTuner2.TabIndex = 1;
            this.btnSigReportTuner2.Text = "Signal Report Tuner 2";
            this.btnSigReportTuner2.UseVisualStyleBackColor = true;
            this.btnSigReportTuner2.Click += new System.EventHandler(this.btnSigReportTuner2_Click);
            // 
            // btnSigReportTuner1
            // 
            this.btnSigReportTuner1.Enabled = false;
            this.btnSigReportTuner1.Location = new System.Drawing.Point(12, 19);
            this.btnSigReportTuner1.Name = "btnSigReportTuner1";
            this.btnSigReportTuner1.Size = new System.Drawing.Size(151, 23);
            this.btnSigReportTuner1.TabIndex = 0;
            this.btnSigReportTuner1.Text = "Signal Report Tuner 1";
            this.btnSigReportTuner1.UseVisualStyleBackColor = true;
            this.btnSigReportTuner1.Click += new System.EventHandler(this.btnSigReportTuner1_Click);
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
            this.lbUsers.Size = new System.Drawing.Size(180, 340);
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
            this.richChat.Size = new System.Drawing.Size(889, 460);
            this.richChat.TabIndex = 3;
            this.richChat.Text = "";
            this.richChat.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richChat_LinkClicked);
            // 
            // chatContextMenuStrip
            // 
            this.chatContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedTextToolStripMenuItem});
            this.chatContextMenuStrip.Name = "contextMenuStrip1";
            this.chatContextMenuStrip.Size = new System.Drawing.Size(218, 26);
            // 
            // copySelectedTextToolStripMenuItem
            // 
            this.copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
            this.copySelectedTextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copySelectedTextToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.copySelectedTextToolStripMenuItem.Text = "Copy Selected Text";
            this.copySelectedTextToolStripMenuItem.Click += new System.EventHandler(this.copySelectedTextToolStripMenuItem_Click);
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
            this.lbChat.Size = new System.Drawing.Size(889, 460);
            this.lbChat.TabIndex = 1;
            this.lbChat.Resize += new System.EventHandler(this.lbChat_Resize);
            // 
            // txtMessage
            // 
            this.txtMessage.AcceptsReturn = true;
            this.txtMessage.ContextMenuStrip = this.textInputContextStrip;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Enabled = false;
            this.txtMessage.Location = new System.Drawing.Point(0, 460);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(889, 20);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // textInputContextStrip
            // 
            this.textInputContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem});
            this.textInputContextStrip.Name = "textInputContextStrip";
            this.textInputContextStrip.Size = new System.Drawing.Size(146, 26);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.selectAllToolStripMenuItem.Text = "&Paste";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // WebChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 502);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "WebChatForm";
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
            this.groupBox1.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSigReportTuner4;
        private System.Windows.Forms.Button btnSigReportTuner3;
        private System.Windows.Forms.Button btnSigReportTuner2;
        private System.Windows.Forms.Button btnSigReportTuner1;
    }
}
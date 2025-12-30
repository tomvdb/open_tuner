namespace opentuner.MediaSources.WinterHill
{
    partial class PicoWHBroadcastListenerForm
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
            this.lbBroadcast = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDetectedIP = new System.Windows.Forms.Label();
            this.lblDetectedBasePort = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnChangeBasePort = new System.Windows.Forms.Button();
            this.txtNewBasePort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBootsel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReboot = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.llblCopyToClickboardIP = new System.Windows.Forms.LinkLabel();
            this.llblCopyToClickboardPort = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbBroadcast
            // 
            this.lbBroadcast.FormattingEnabled = true;
            this.lbBroadcast.Location = new System.Drawing.Point(12, 12);
            this.lbBroadcast.Name = "lbBroadcast";
            this.lbBroadcast.Size = new System.Drawing.Size(483, 329);
            this.lbBroadcast.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(820, 346);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(511, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Detected IP Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(511, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Detected Base Port:";
            // 
            // lblDetectedIP
            // 
            this.lblDetectedIP.AutoSize = true;
            this.lblDetectedIP.Location = new System.Drawing.Point(625, 21);
            this.lblDetectedIP.Name = "lblDetectedIP";
            this.lblDetectedIP.Size = new System.Drawing.Size(53, 13);
            this.lblDetectedIP.TabIndex = 4;
            this.lblDetectedIP.Text = "Unknown";
            this.lblDetectedIP.TextChanged += new System.EventHandler(this.lblDetectedIP_TextChanged);
            // 
            // lblDetectedBasePort
            // 
            this.lblDetectedBasePort.AutoSize = true;
            this.lblDetectedBasePort.Location = new System.Drawing.Point(625, 45);
            this.lblDetectedBasePort.Name = "lblDetectedBasePort";
            this.lblDetectedBasePort.Size = new System.Drawing.Size(53, 13);
            this.lblDetectedBasePort.TabIndex = 5;
            this.lblDetectedBasePort.Text = "Unknown";
            this.lblDetectedBasePort.TextChanged += new System.EventHandler(this.lblDetectedBasePort_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnChangeBasePort);
            this.groupBox1.Controls.Add(this.txtNewBasePort);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnBootsel);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnReboot);
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Location = new System.Drawing.Point(514, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 260);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Remote Commands ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(308, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "The Base IP Port settings are forgotten when power is removed.";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(170, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 31);
            this.label6.TabIndex = 8;
            this.label6.Text = "Valid Base IP Port settings are any even number from xx00 to xx14.";
            // 
            // btnChangeBasePort
            // 
            this.btnChangeBasePort.Enabled = false;
            this.btnChangeBasePort.Location = new System.Drawing.Point(16, 119);
            this.btnChangeBasePort.Name = "btnChangeBasePort";
            this.btnChangeBasePort.Size = new System.Drawing.Size(75, 44);
            this.btnChangeBasePort.TabIndex = 7;
            this.btnChangeBasePort.Text = "Change Base Port";
            this.btnChangeBasePort.UseVisualStyleBackColor = true;
            this.btnChangeBasePort.Click += new System.EventHandler(this.btnChangeBasePort_Click);
            // 
            // txtNewBasePort
            // 
            this.txtNewBasePort.Location = new System.Drawing.Point(114, 132);
            this.txtNewBasePort.Name = "txtNewBasePort";
            this.txtNewBasePort.Size = new System.Drawing.Size(50, 20);
            this.txtNewBasePort.TabIndex = 6;
            this.txtNewBasePort.Text = "9900";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Software Upgrade Mode (Via Serial)";
            // 
            // btnBootsel
            // 
            this.btnBootsel.Enabled = false;
            this.btnBootsel.Location = new System.Drawing.Point(16, 88);
            this.btnBootsel.Name = "btnBootsel";
            this.btnBootsel.Size = new System.Drawing.Size(75, 23);
            this.btnBootsel.TabIndex = 4;
            this.btnBootsel.Text = "Bootsel";
            this.btnBootsel.UseVisualStyleBackColor = true;
            this.btnBootsel.Click += new System.EventHandler(this.btnBootsel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(194, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Reboot (Does not clear remote settings)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(111, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Clear Remote Settings and Reboot";
            // 
            // btnReboot
            // 
            this.btnReboot.Enabled = false;
            this.btnReboot.Location = new System.Drawing.Point(16, 59);
            this.btnReboot.Name = "btnReboot";
            this.btnReboot.Size = new System.Drawing.Size(75, 23);
            this.btnReboot.TabIndex = 1;
            this.btnReboot.Text = "Reboot";
            this.btnReboot.UseVisualStyleBackColor = true;
            this.btnReboot.Click += new System.EventHandler(this.btnReboot_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(16, 30);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // llblCopyToClickboardIP
            // 
            this.llblCopyToClickboardIP.AutoSize = true;
            this.llblCopyToClickboardIP.Location = new System.Drawing.Point(805, 21);
            this.llblCopyToClickboardIP.Name = "llblCopyToClickboardIP";
            this.llblCopyToClickboardIP.Size = new System.Drawing.Size(90, 13);
            this.llblCopyToClickboardIP.TabIndex = 7;
            this.llblCopyToClickboardIP.TabStop = true;
            this.llblCopyToClickboardIP.Text = "Copy to Clipboard";
            this.llblCopyToClickboardIP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblCopyToClickboardIP_LinkClicked);
            // 
            // llblCopyToClickboardPort
            // 
            this.llblCopyToClickboardPort.AutoSize = true;
            this.llblCopyToClickboardPort.Location = new System.Drawing.Point(805, 45);
            this.llblCopyToClickboardPort.Name = "llblCopyToClickboardPort";
            this.llblCopyToClickboardPort.Size = new System.Drawing.Size(94, 13);
            this.llblCopyToClickboardPort.TabIndex = 8;
            this.llblCopyToClickboardPort.TabStop = true;
            this.llblCopyToClickboardPort.Text = "Copy To Clipboard";
            this.llblCopyToClickboardPort.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblCopyToClickboardPort_LinkClicked);
            // 
            // PicoWHBroadcastListenerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 384);
            this.ControlBox = false;
            this.Controls.Add(this.llblCopyToClickboardPort);
            this.Controls.Add(this.llblCopyToClickboardIP);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblDetectedBasePort);
            this.Controls.Add(this.lblDetectedIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbBroadcast);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PicoWHBroadcastListenerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PicoTuner (WH) Broadcast Listener";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PicoWHBroadcastListenerForm_FormClosing);
            this.Load += new System.EventHandler(this.PicoWHBroadcastListenerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbBroadcast;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDetectedIP;
        private System.Windows.Forms.Label lblDetectedBasePort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBootsel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReboot;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnChangeBasePort;
        private System.Windows.Forms.TextBox txtNewBasePort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel llblCopyToClickboardIP;
        private System.Windows.Forms.LinkLabel llblCopyToClickboardPort;
    }
}
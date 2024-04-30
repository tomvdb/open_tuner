namespace opentuner.MediaSources.Winterhill
{
    partial class WinterhillSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWHWSPort = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBroadcastListener = new System.Windows.Forms.Button();
            this.comboDefaultInterface = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtWHWSIp = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUDPIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWHWSBaseUdp = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtUDPBasePort = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtWHWSBaseUdp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtWHWSIp);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtWHWSPort);
            this.groupBox1.Location = new System.Drawing.Point(12, 315);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(471, 144);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Winterhill WS Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Winterhill WS Port:";
            // 
            // txtWHWSPort
            // 
            this.txtWHWSPort.Location = new System.Drawing.Point(191, 71);
            this.txtWHWSPort.Margin = new System.Windows.Forms.Padding(4);
            this.txtWHWSPort.Name = "txtWHWSPort";
            this.txtWHWSPort.Size = new System.Drawing.Size(113, 22);
            this.txtWHWSPort.TabIndex = 8;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(387, 590);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(279, 590);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboDefaultInterface);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(472, 153);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // btnBroadcastListener
            // 
            this.btnBroadcastListener.Location = new System.Drawing.Point(13, 590);
            this.btnBroadcastListener.Name = "btnBroadcastListener";
            this.btnBroadcastListener.Size = new System.Drawing.Size(231, 28);
            this.btnBroadcastListener.TabIndex = 2;
            this.btnBroadcastListener.Text = "PicoTuner (WH) Broadcast Listener";
            this.btnBroadcastListener.UseVisualStyleBackColor = true;
            this.btnBroadcastListener.Click += new System.EventHandler(this.btnBroadcastListener_Click);
            // 
            // comboDefaultInterface
            // 
            this.comboDefaultInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDefaultInterface.FormattingEnabled = true;
            this.comboDefaultInterface.Items.AddRange(new object[] {
            "Always Ask",
            "Websocket (ZR6TG)",
            "PicoTuner Ethernet (G4EWJ)"});
            this.comboDefaultInterface.Location = new System.Drawing.Point(192, 29);
            this.comboDefaultInterface.Name = "comboDefaultInterface";
            this.comboDefaultInterface.Size = new System.Drawing.Size(206, 24);
            this.comboDefaultInterface.TabIndex = 1;
            this.comboDefaultInterface.SelectedIndexChanged += new System.EventHandler(this.comboDefaultInterface_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Default Interface: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Winterhill WS IP:";
            // 
            // txtWHWSIp
            // 
            this.txtWHWSIp.Location = new System.Drawing.Point(191, 38);
            this.txtWHWSIp.Margin = new System.Windows.Forms.Padding(4);
            this.txtWHWSIp.Name = "txtWHWSIp";
            this.txtWHWSIp.Size = new System.Drawing.Size(206, 22);
            this.txtWHWSIp.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtUDPIP);
            this.groupBox3.Controls.Add(this.txtUDPBasePort);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 171);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(471, 137);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Winterhill (PicoTuner Ethernet)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "IP Address:";
            // 
            // txtUDPIP
            // 
            this.txtUDPIP.Location = new System.Drawing.Point(191, 36);
            this.txtUDPIP.Name = "txtUDPIP";
            this.txtUDPIP.Size = new System.Drawing.Size(206, 22);
            this.txtUDPIP.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 106);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "Winterhill Udp Port:";
            // 
            // txtWHWSBaseUdp
            // 
            this.txtWHWSBaseUdp.Location = new System.Drawing.Point(191, 103);
            this.txtWHWSBaseUdp.Margin = new System.Windows.Forms.Padding(4);
            this.txtWHWSBaseUdp.Name = "txtWHWSBaseUdp";
            this.txtWHWSBaseUdp.Size = new System.Drawing.Size(113, 22);
            this.txtWHWSBaseUdp.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 68);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Udp Base Port:";
            // 
            // txtUDPBasePort
            // 
            this.txtUDPBasePort.Location = new System.Drawing.Point(191, 65);
            this.txtUDPBasePort.Margin = new System.Windows.Forms.Padding(4);
            this.txtUDPBasePort.Name = "txtUDPBasePort";
            this.txtUDPBasePort.Size = new System.Drawing.Size(113, 22);
            this.txtUDPBasePort.TabIndex = 14;
            // 
            // WinterhillSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(501, 631);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnBroadcastListener);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WinterhillSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Winterhill Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWHWSPort;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBroadcastListener;
        private System.Windows.Forms.ComboBox comboDefaultInterface;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWHWSIp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtUDPIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWHWSBaseUdp;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtUDPBasePort;
    }
}
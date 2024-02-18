namespace opentuner.MediaSources.Longmynd
{
    partial class LongmyndSettingsForm
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
            this.txtTuner1FreqOffset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupHardwareInterface = new System.Windows.Forms.GroupBox();
            this.txtWSIpAddress = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboHardwareInterface = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMqttIpAddress = new System.Windows.Forms.MaskedTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtWSPort = new System.Windows.Forms.TextBox();
            this.txtMqttPort = new System.Windows.Forms.TextBox();
            this.txtBaseCmdTopic = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupHardwareInterface.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTuner1FreqOffset);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 252);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 77);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tuner Properties";
            // 
            // txtTuner1FreqOffset
            // 
            this.txtTuner1FreqOffset.Location = new System.Drawing.Point(149, 30);
            this.txtTuner1FreqOffset.Name = "txtTuner1FreqOffset";
            this.txtTuner1FreqOffset.Size = new System.Drawing.Size(160, 20);
            this.txtTuner1FreqOffset.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Tuner 1 Freq Offset:";
            // 
            // groupHardwareInterface
            // 
            this.groupHardwareInterface.Controls.Add(this.txtBaseCmdTopic);
            this.groupHardwareInterface.Controls.Add(this.txtMqttPort);
            this.groupHardwareInterface.Controls.Add(this.txtWSPort);
            this.groupHardwareInterface.Controls.Add(this.label8);
            this.groupHardwareInterface.Controls.Add(this.label5);
            this.groupHardwareInterface.Controls.Add(this.txtMqttIpAddress);
            this.groupHardwareInterface.Controls.Add(this.label6);
            this.groupHardwareInterface.Controls.Add(this.label4);
            this.groupHardwareInterface.Controls.Add(this.txtWSIpAddress);
            this.groupHardwareInterface.Controls.Add(this.label2);
            this.groupHardwareInterface.Controls.Add(this.comboHardwareInterface);
            this.groupHardwareInterface.Controls.Add(this.label1);
            this.groupHardwareInterface.Location = new System.Drawing.Point(12, 12);
            this.groupHardwareInterface.Name = "groupHardwareInterface";
            this.groupHardwareInterface.Size = new System.Drawing.Size(328, 234);
            this.groupHardwareInterface.TabIndex = 2;
            this.groupHardwareInterface.TabStop = false;
            this.groupHardwareInterface.Text = "Hardware Interface";
            // 
            // txtWSIpAddress
            // 
            this.txtWSIpAddress.Location = new System.Drawing.Point(149, 62);
            this.txtWSIpAddress.Name = "txtWSIpAddress";
            this.txtWSIpAddress.Size = new System.Drawing.Size(160, 20);
            this.txtWSIpAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP Address (WS): ";
            // 
            // comboHardwareInterface
            // 
            this.comboHardwareInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHardwareInterface.FormattingEnabled = true;
            this.comboHardwareInterface.Items.AddRange(new object[] {
            "Always Ask",
            "Mqtt (F5OEO)",
            "Websocket (M0DNY)"});
            this.comboHardwareInterface.Location = new System.Drawing.Point(149, 24);
            this.comboHardwareInterface.Name = "comboHardwareInterface";
            this.comboHardwareInterface.Size = new System.Drawing.Size(160, 21);
            this.comboHardwareInterface.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Interface: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Port (WS):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Port (MQTT):";
            // 
            // txtMqttIpAddress
            // 
            this.txtMqttIpAddress.Location = new System.Drawing.Point(149, 126);
            this.txtMqttIpAddress.Name = "txtMqttIpAddress";
            this.txtMqttIpAddress.Size = new System.Drawing.Size(160, 20);
            this.txtMqttIpAddress.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "IP Address (MQTT): ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 200);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "MQTT Base CMD Topic:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(184, 335);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(265, 335);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtWSPort
            // 
            this.txtWSPort.Location = new System.Drawing.Point(149, 88);
            this.txtWSPort.Name = "txtWSPort";
            this.txtWSPort.Size = new System.Drawing.Size(160, 20);
            this.txtWSPort.TabIndex = 11;
            // 
            // txtMqttPort
            // 
            this.txtMqttPort.Location = new System.Drawing.Point(149, 152);
            this.txtMqttPort.Name = "txtMqttPort";
            this.txtMqttPort.Size = new System.Drawing.Size(160, 20);
            this.txtMqttPort.TabIndex = 12;
            // 
            // txtBaseCmdTopic
            // 
            this.txtBaseCmdTopic.Location = new System.Drawing.Point(149, 197);
            this.txtBaseCmdTopic.Name = "txtBaseCmdTopic";
            this.txtBaseCmdTopic.Size = new System.Drawing.Size(160, 20);
            this.txtBaseCmdTopic.TabIndex = 13;
            // 
            // LongmyndSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 370);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupHardwareInterface);
            this.Name = "LongmyndSettingsForm";
            this.Text = "Longmynd Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupHardwareInterface.ResumeLayout(false);
            this.groupHardwareInterface.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtTuner1FreqOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupHardwareInterface;
        private System.Windows.Forms.MaskedTextBox txtWSIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboHardwareInterface;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox txtMqttIpAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtBaseCmdTopic;
        private System.Windows.Forms.TextBox txtMqttPort;
        private System.Windows.Forms.TextBox txtWSPort;
    }
}
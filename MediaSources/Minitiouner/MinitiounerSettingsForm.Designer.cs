namespace opentuner.MediaSources.Minitiouner
{
    partial class MinitiounerSettingsForm
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
            this.groupHardwareInterface = new System.Windows.Forms.GroupBox();
            this.txtIpAddress = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboHardwareInterface = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ComboDefaultRFInput = new System.Windows.Forms.ComboBox();
            this.comboSupplyBDefault = new System.Windows.Forms.ComboBox();
            this.comboSupplyADefault = new System.Windows.Forms.ComboBox();
            this.txtTuner2FreqOffset = new System.Windows.Forms.TextBox();
            this.txtTuner1FreqOffset = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupHardwareInterface.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupHardwareInterface
            // 
            this.groupHardwareInterface.Controls.Add(this.txtIpAddress);
            this.groupHardwareInterface.Controls.Add(this.label2);
            this.groupHardwareInterface.Controls.Add(this.comboHardwareInterface);
            this.groupHardwareInterface.Controls.Add(this.label1);
            this.groupHardwareInterface.Location = new System.Drawing.Point(12, 12);
            this.groupHardwareInterface.Name = "groupHardwareInterface";
            this.groupHardwareInterface.Size = new System.Drawing.Size(328, 105);
            this.groupHardwareInterface.TabIndex = 0;
            this.groupHardwareInterface.TabStop = false;
            this.groupHardwareInterface.Text = "Hardware Interface";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Enabled = false;
            this.txtIpAddress.Location = new System.Drawing.Point(134, 62);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(160, 20);
            this.txtIpAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(16, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP Address: ";
            // 
            // comboHardwareInterface
            // 
            this.comboHardwareInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHardwareInterface.FormattingEnabled = true;
            this.comboHardwareInterface.Items.AddRange(new object[] {
            "Always Ask",
            "FTDI Module",
            "PicoTuner"});
            this.comboHardwareInterface.Location = new System.Drawing.Point(134, 24);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ComboDefaultRFInput);
            this.groupBox1.Controls.Add(this.comboSupplyBDefault);
            this.groupBox1.Controls.Add(this.comboSupplyADefault);
            this.groupBox1.Controls.Add(this.txtTuner2FreqOffset);
            this.groupBox1.Controls.Add(this.txtTuner1FreqOffset);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 173);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tuner Properties";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Default RF Input:";
            // 
            // ComboDefaultRFInput
            // 
            this.ComboDefaultRFInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboDefaultRFInput.FormattingEnabled = true;
            this.ComboDefaultRFInput.Items.AddRange(new object[] {
            "Tuner 1 = A, Tuner 2 = A",
            "Tuner 1 = A, Tuner 2 = B",
            "Tuner 1 = B, Tuner 2 = A",
            "Tuner 1 = B, Tuner 2 = B"});
            this.ComboDefaultRFInput.Location = new System.Drawing.Point(134, 136);
            this.ComboDefaultRFInput.Name = "ComboDefaultRFInput";
            this.ComboDefaultRFInput.Size = new System.Drawing.Size(160, 21);
            this.ComboDefaultRFInput.TabIndex = 8;
            // 
            // comboSupplyBDefault
            // 
            this.comboSupplyBDefault.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSupplyBDefault.FormattingEnabled = true;
            this.comboSupplyBDefault.Items.AddRange(new object[] {
            "Off",
            "13V Vertical",
            "18V Horizontal"});
            this.comboSupplyBDefault.Location = new System.Drawing.Point(134, 109);
            this.comboSupplyBDefault.Name = "comboSupplyBDefault";
            this.comboSupplyBDefault.Size = new System.Drawing.Size(160, 21);
            this.comboSupplyBDefault.TabIndex = 7;
            // 
            // comboSupplyADefault
            // 
            this.comboSupplyADefault.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSupplyADefault.FormattingEnabled = true;
            this.comboSupplyADefault.Items.AddRange(new object[] {
            "Off",
            "13V Vertical",
            "18V Horizontal"});
            this.comboSupplyADefault.Location = new System.Drawing.Point(134, 82);
            this.comboSupplyADefault.Name = "comboSupplyADefault";
            this.comboSupplyADefault.Size = new System.Drawing.Size(160, 21);
            this.comboSupplyADefault.TabIndex = 6;
            // 
            // txtTuner2FreqOffset
            // 
            this.txtTuner2FreqOffset.Location = new System.Drawing.Point(134, 56);
            this.txtTuner2FreqOffset.Name = "txtTuner2FreqOffset";
            this.txtTuner2FreqOffset.Size = new System.Drawing.Size(160, 20);
            this.txtTuner2FreqOffset.TabIndex = 5;
            // 
            // txtTuner1FreqOffset
            // 
            this.txtTuner1FreqOffset.Location = new System.Drawing.Point(134, 30);
            this.txtTuner1FreqOffset.Name = "txtTuner1FreqOffset";
            this.txtTuner1FreqOffset.Size = new System.Drawing.Size(160, 20);
            this.txtTuner1FreqOffset.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "LNB B Supply Default:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "LNB A Supply Default:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Tuner 2 Freq Offset:";
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
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(265, 302);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(184, 302);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MinitiounerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 335);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupHardwareInterface);
            this.Name = "MinitiounerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Minitiouner Settings";
            this.groupHardwareInterface.ResumeLayout(false);
            this.groupHardwareInterface.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupHardwareInterface;
        private System.Windows.Forms.MaskedTextBox txtIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboHardwareInterface;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox ComboDefaultRFInput;
        private System.Windows.Forms.ComboBox comboSupplyBDefault;
        private System.Windows.Forms.ComboBox comboSupplyADefault;
        private System.Windows.Forms.TextBox txtTuner2FreqOffset;
        private System.Windows.Forms.TextBox txtTuner1FreqOffset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
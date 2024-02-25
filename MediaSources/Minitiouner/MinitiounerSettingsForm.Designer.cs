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
            this.groupHardwareInterface.Location = new System.Drawing.Point(16, 15);
            this.groupHardwareInterface.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupHardwareInterface.Name = "groupHardwareInterface";
            this.groupHardwareInterface.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupHardwareInterface.Size = new System.Drawing.Size(437, 129);
            this.groupHardwareInterface.TabIndex = 0;
            this.groupHardwareInterface.TabStop = false;
            this.groupHardwareInterface.Text = "Hardware Interface";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Enabled = false;
            this.txtIpAddress.Location = new System.Drawing.Point(179, 76);
            this.txtIpAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(212, 22);
            this.txtIpAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(21, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
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
            this.comboHardwareInterface.Location = new System.Drawing.Point(179, 30);
            this.comboHardwareInterface.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboHardwareInterface.Name = "comboHardwareInterface";
            this.comboHardwareInterface.Size = new System.Drawing.Size(212, 24);
            this.comboHardwareInterface.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
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
            this.groupBox1.Location = new System.Drawing.Point(16, 151);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(437, 213);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tuner Properties";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 171);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 16);
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
            this.ComboDefaultRFInput.Location = new System.Drawing.Point(179, 167);
            this.ComboDefaultRFInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboDefaultRFInput.Name = "ComboDefaultRFInput";
            this.ComboDefaultRFInput.Size = new System.Drawing.Size(212, 24);
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
            this.comboSupplyBDefault.Location = new System.Drawing.Point(179, 134);
            this.comboSupplyBDefault.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboSupplyBDefault.Name = "comboSupplyBDefault";
            this.comboSupplyBDefault.Size = new System.Drawing.Size(212, 24);
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
            this.comboSupplyADefault.Location = new System.Drawing.Point(179, 101);
            this.comboSupplyADefault.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboSupplyADefault.Name = "comboSupplyADefault";
            this.comboSupplyADefault.Size = new System.Drawing.Size(212, 24);
            this.comboSupplyADefault.TabIndex = 6;
            // 
            // txtTuner2FreqOffset
            // 
            this.txtTuner2FreqOffset.Location = new System.Drawing.Point(179, 69);
            this.txtTuner2FreqOffset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTuner2FreqOffset.Name = "txtTuner2FreqOffset";
            this.txtTuner2FreqOffset.Size = new System.Drawing.Size(212, 22);
            this.txtTuner2FreqOffset.TabIndex = 5;
            // 
            // txtTuner1FreqOffset
            // 
            this.txtTuner1FreqOffset.Location = new System.Drawing.Point(179, 37);
            this.txtTuner1FreqOffset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTuner1FreqOffset.Name = "txtTuner1FreqOffset";
            this.txtTuner1FreqOffset.Size = new System.Drawing.Size(212, 22);
            this.txtTuner1FreqOffset.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 138);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "LNB B Supply Default:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 105);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "LNB A Supply Default:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 73);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Tuner 2 Freq Offset:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Tuner 1 Freq Offset:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(353, 372);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(245, 372);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MinitiounerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(473, 412);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupHardwareInterface);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
namespace opentuner.ExtraFeatures.QuickTuneControl
{
    partial class QuickTuneControlSettingsForm
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
            this.txtUdp4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUdp3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUdp2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUdp1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Tuner4TuningMode = new System.Windows.Forms.ComboBox();
            this.Tuner3TuningMode = new System.Windows.Forms.ComboBox();
            this.Tuner2TuningMode = new System.Windows.Forms.ComboBox();
            this.Tuner1TuningMode = new System.Windows.Forms.ComboBox();
            this.AvoidBeacon1 = new System.Windows.Forms.CheckBox();
            this.AvoidBeacon2 = new System.Windows.Forms.CheckBox();
            this.AvoidBeacon3 = new System.Windows.Forms.CheckBox();
            this.AvoidBeacon4 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtUdp4);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtUdp3);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUdp2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUdp1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(198, 181);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick Tune UDP Settings";
            // 
            // txtUdp4
            // 
            this.txtUdp4.Location = new System.Drawing.Point(130, 145);
            this.txtUdp4.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdp4.Name = "txtUdp4";
            this.txtUdp4.Size = new System.Drawing.Size(53, 22);
            this.txtUdp4.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 148);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Tuner 4 UDP Port:";
            // 
            // txtUdp3
            // 
            this.txtUdp3.Location = new System.Drawing.Point(130, 113);
            this.txtUdp3.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdp3.Name = "txtUdp3";
            this.txtUdp3.Size = new System.Drawing.Size(53, 22);
            this.txtUdp3.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 116);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Tuner 3 UDP Port:";
            // 
            // txtUdp2
            // 
            this.txtUdp2.Location = new System.Drawing.Point(130, 81);
            this.txtUdp2.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdp2.Name = "txtUdp2";
            this.txtUdp2.Size = new System.Drawing.Size(53, 22);
            this.txtUdp2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tuner 2 UDP Port:";
            // 
            // txtUdp1
            // 
            this.txtUdp1.Location = new System.Drawing.Point(130, 49);
            this.txtUdp1.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdp1.Name = "txtUdp1";
            this.txtUdp1.Size = new System.Drawing.Size(53, 22);
            this.txtUdp1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tuner 1 UDP Port:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(377, 203);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(269, 203);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AvoidBeacon4);
            this.groupBox2.Controls.Add(this.AvoidBeacon3);
            this.groupBox2.Controls.Add(this.AvoidBeacon2);
            this.groupBox2.Controls.Add(this.AvoidBeacon1);
            this.groupBox2.Controls.Add(this.Tuner4TuningMode);
            this.groupBox2.Controls.Add(this.Tuner3TuningMode);
            this.groupBox2.Controls.Add(this.Tuner2TuningMode);
            this.groupBox2.Controls.Add(this.Tuner1TuningMode);
            this.groupBox2.Location = new System.Drawing.Point(221, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 181);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tuning Mode";
            // 
            // Tuner4TuningMode
            // 
            this.Tuner4TuningMode.FormattingEnabled = true;
            this.Tuner4TuningMode.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Timed)"});
            this.Tuner4TuningMode.Location = new System.Drawing.Point(6, 145);
            this.Tuner4TuningMode.Name = "Tuner4TuningMode";
            this.Tuner4TuningMode.Size = new System.Drawing.Size(121, 24);
            this.Tuner4TuningMode.TabIndex = 3;
            // 
            // Tuner3TuningMode
            // 
            this.Tuner3TuningMode.FormattingEnabled = true;
            this.Tuner3TuningMode.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Timed)"});
            this.Tuner3TuningMode.Location = new System.Drawing.Point(6, 113);
            this.Tuner3TuningMode.Name = "Tuner3TuningMode";
            this.Tuner3TuningMode.Size = new System.Drawing.Size(121, 24);
            this.Tuner3TuningMode.TabIndex = 2;
            // 
            // Tuner2TuningMode
            // 
            this.Tuner2TuningMode.FormattingEnabled = true;
            this.Tuner2TuningMode.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Timed)"});
            this.Tuner2TuningMode.Location = new System.Drawing.Point(6, 81);
            this.Tuner2TuningMode.Name = "Tuner2TuningMode";
            this.Tuner2TuningMode.Size = new System.Drawing.Size(121, 24);
            this.Tuner2TuningMode.TabIndex = 1;
            // 
            // Tuner1TuningMode
            // 
            this.Tuner1TuningMode.FormattingEnabled = true;
            this.Tuner1TuningMode.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Timed)"});
            this.Tuner1TuningMode.Location = new System.Drawing.Point(6, 49);
            this.Tuner1TuningMode.Name = "Tuner1TuningMode";
            this.Tuner1TuningMode.Size = new System.Drawing.Size(121, 24);
            this.Tuner1TuningMode.TabIndex = 0;
            // 
            // AvoidBeacon1
            // 
            this.AvoidBeacon1.AutoSize = true;
            this.AvoidBeacon1.Checked = true;
            this.AvoidBeacon1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidBeacon1.Location = new System.Drawing.Point(133, 51);
            this.AvoidBeacon1.Name = "AvoidBeacon1";
            this.AvoidBeacon1.Size = new System.Drawing.Size(111, 20);
            this.AvoidBeacon1.TabIndex = 4;
            this.AvoidBeacon1.Text = "Avoid Beacon";
            this.AvoidBeacon1.UseVisualStyleBackColor = true;
            // 
            // AvoidBeacon2
            // 
            this.AvoidBeacon2.AutoSize = true;
            this.AvoidBeacon2.Checked = true;
            this.AvoidBeacon2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidBeacon2.Location = new System.Drawing.Point(133, 83);
            this.AvoidBeacon2.Name = "AvoidBeacon2";
            this.AvoidBeacon2.Size = new System.Drawing.Size(111, 20);
            this.AvoidBeacon2.TabIndex = 5;
            this.AvoidBeacon2.Text = "Avoid Beacon";
            this.AvoidBeacon2.UseVisualStyleBackColor = true;
            // 
            // AvoidBeacon3
            // 
            this.AvoidBeacon3.AutoSize = true;
            this.AvoidBeacon3.Checked = true;
            this.AvoidBeacon3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidBeacon3.Location = new System.Drawing.Point(133, 115);
            this.AvoidBeacon3.Name = "AvoidBeacon3";
            this.AvoidBeacon3.Size = new System.Drawing.Size(111, 20);
            this.AvoidBeacon3.TabIndex = 6;
            this.AvoidBeacon3.Text = "Avoid Beacon";
            this.AvoidBeacon3.UseVisualStyleBackColor = true;
            // 
            // AvoidBeacon4
            // 
            this.AvoidBeacon4.AutoSize = true;
            this.AvoidBeacon4.Checked = true;
            this.AvoidBeacon4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidBeacon4.Location = new System.Drawing.Point(133, 147);
            this.AvoidBeacon4.Name = "AvoidBeacon4";
            this.AvoidBeacon4.Size = new System.Drawing.Size(111, 20);
            this.AvoidBeacon4.TabIndex = 7;
            this.AvoidBeacon4.Text = "Avoid Beacon";
            this.AvoidBeacon4.UseVisualStyleBackColor = true;
            // 
            // QuickTuneControlSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(489, 243);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "QuickTuneControlSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quick Tune Listener Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtUdp4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUdp3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUdp2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUdp1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox Tuner1TuningMode;
        private System.Windows.Forms.ComboBox Tuner2TuningMode;
        private System.Windows.Forms.ComboBox Tuner3TuningMode;
        private System.Windows.Forms.ComboBox Tuner4TuningMode;
        private System.Windows.Forms.CheckBox AvoidBeacon1;
        private System.Windows.Forms.CheckBox AvoidBeacon2;
        private System.Windows.Forms.CheckBox AvoidBeacon3;
        private System.Windows.Forms.CheckBox AvoidBeacon4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
namespace opentuner.ExtraFeatures.BATCSpectrum
{
    partial class oneTunerTuneModeForm
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
            this.avoidBeacon1 = new System.Windows.Forms.CheckBox();
            this.tuneMode1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.avoidBeacon1);
            this.groupBox1.Controls.Add(this.tuneMode1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(15, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 54);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tuning Mode";
            // 
            // avoidBeacon1
            // 
            this.avoidBeacon1.AutoSize = true;
            this.avoidBeacon1.Checked = true;
            this.avoidBeacon1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avoidBeacon1.Enabled = false;
            this.avoidBeacon1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.avoidBeacon1.Location = new System.Drawing.Point(177, 21);
            this.avoidBeacon1.Name = "avoidBeacon1";
            this.avoidBeacon1.Size = new System.Drawing.Size(111, 20);
            this.avoidBeacon1.TabIndex = 8;
            this.avoidBeacon1.Text = "Avoid Beacon";
            this.avoidBeacon1.UseVisualStyleBackColor = true;
            // 
            // tuneMode1
            // 
            this.tuneMode1.Enabled = false;
            this.tuneMode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tuneMode1.FormattingEnabled = true;
            this.tuneMode1.Items.AddRange(new object[] {
            "Manual",
            "Auto"});
            this.tuneMode1.Location = new System.Drawing.Point(50, 20);
            this.tuneMode1.Name = "tuneMode1";
            this.tuneMode1.Size = new System.Drawing.Size(121, 21);
            this.tuneMode1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "RX 1:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(219, 61);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // oneTunerTuneModeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 99);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "oneTunerTuneModeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox tuneMode1;
        private System.Windows.Forms.CheckBox avoidBeacon1;
        private System.Windows.Forms.Button btnSave;
    }
}
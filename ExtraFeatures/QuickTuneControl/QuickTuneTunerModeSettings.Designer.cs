namespace opentuner.ExtraFeatures.QuickTuneControl
{
    partial class QuickTuneTunerModeSettings
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Tuner1TuningMode = new System.Windows.Forms.ComboBox();
            this.AvoidBeacon1 = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AvoidBeacon1);
            this.groupBox2.Controls.Add(this.Tuner1TuningMode);
            this.groupBox2.Location = new System.Drawing.Point(8, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(229, 55);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tuning Mode";
            // 
            // Tuner1TuningMode
            // 
            this.Tuner1TuningMode.FormattingEnabled = true;
            this.Tuner1TuningMode.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Timed)"});
            this.Tuner1TuningMode.Location = new System.Drawing.Point(6, 19);
            this.Tuner1TuningMode.Name = "Tuner1TuningMode";
            this.Tuner1TuningMode.Size = new System.Drawing.Size(121, 21);
            this.Tuner1TuningMode.TabIndex = 0;
            // 
            // AvoidBeacon1
            // 
            this.AvoidBeacon1.AutoSize = true;
            this.AvoidBeacon1.Checked = true;
            this.AvoidBeacon1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidBeacon1.Location = new System.Drawing.Point(133, 23);
            this.AvoidBeacon1.Name = "AvoidBeacon1";
            this.AvoidBeacon1.Size = new System.Drawing.Size(93, 17);
            this.AvoidBeacon1.TabIndex = 4;
            this.AvoidBeacon1.Text = "Avoid Beacon";
            this.AvoidBeacon1.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(30, 74);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(138, 74);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // QuickTuneTunerModeSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 110);
            this.ControlBox = false;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickTuneTunerModeSettings";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox AvoidBeacon1;
        private System.Windows.Forms.ComboBox Tuner1TuningMode;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
    }
}
namespace opentuner.ExtraFeatures.BATCSpectrum
{
    partial class BATCSpectrumSettingsForm
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
            this.label10 = new System.Windows.Forms.Label();
            this.treshHold = new System.Windows.Forms.NumericUpDown();
            this.avoidBeacon4 = new System.Windows.Forms.CheckBox();
            this.avoidBeacon3 = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.avoidBeacon2 = new System.Windows.Forms.CheckBox();
            this.avoidBeacon1 = new System.Windows.Forms.CheckBox();
            this.tuneMode4 = new System.Windows.Forms.ComboBox();
            this.tuneMode3 = new System.Windows.Forms.ComboBox();
            this.tuneMode2 = new System.Windows.Forms.ComboBox();
            this.tuneMode1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.overPowerIndicatorLayout = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.autoTuneTimeValue = new System.Windows.Forms.NumericUpDown();
            this.autoHoldTimeValue = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treshHold)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoTuneTimeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.autoHoldTimeValue)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.treshHold);
            this.groupBox1.Controls.Add(this.avoidBeacon4);
            this.groupBox1.Controls.Add(this.avoidBeacon3);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.avoidBeacon2);
            this.groupBox1.Controls.Add(this.avoidBeacon1);
            this.groupBox1.Controls.Add(this.tuneMode4);
            this.groupBox1.Controls.Add(this.tuneMode3);
            this.groupBox1.Controls.Add(this.tuneMode2);
            this.groupBox1.Controls.Add(this.tuneMode1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(15, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 181);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tuning Mode";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label10.Location = new System.Drawing.Point(175, 145);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 16);
            this.label10.TabIndex = 12;
            this.label10.Text = "dB below Beacon";
            // 
            // treshHold
            // 
            this.treshHold.DecimalPlaces = 1;
            this.treshHold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.treshHold.Location = new System.Drawing.Point(80, 145);
            this.treshHold.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            -2147418112});
            this.treshHold.Minimum = new decimal(new int[] {
            69,
            0,
            0,
            -2147418112});
            this.treshHold.Name = "treshHold";
            this.treshHold.Size = new System.Drawing.Size(89, 20);
            this.treshHold.TabIndex = 11;
            this.treshHold.Value = new decimal(new int[] {
            60,
            0,
            0,
            -2147418112});
            // 
            // avoidBeacon4
            // 
            this.avoidBeacon4.AutoSize = true;
            this.avoidBeacon4.Checked = true;
            this.avoidBeacon4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avoidBeacon4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.avoidBeacon4.Location = new System.Drawing.Point(177, 116);
            this.avoidBeacon4.Name = "avoidBeacon4";
            this.avoidBeacon4.Size = new System.Drawing.Size(111, 20);
            this.avoidBeacon4.TabIndex = 11;
            this.avoidBeacon4.Text = "Avoid Beacon";
            this.avoidBeacon4.UseVisualStyleBackColor = true;
            // 
            // avoidBeacon3
            // 
            this.avoidBeacon3.AutoSize = true;
            this.avoidBeacon3.Checked = true;
            this.avoidBeacon3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avoidBeacon3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.avoidBeacon3.Location = new System.Drawing.Point(177, 86);
            this.avoidBeacon3.Name = "avoidBeacon3";
            this.avoidBeacon3.Size = new System.Drawing.Size(111, 20);
            this.avoidBeacon3.TabIndex = 10;
            this.avoidBeacon3.Text = "Avoid Beacon";
            this.avoidBeacon3.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label9.Location = new System.Drawing.Point(6, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 16);
            this.label9.TabIndex = 10;
            this.label9.Text = "Treshhold";
            // 
            // avoidBeacon2
            // 
            this.avoidBeacon2.AutoSize = true;
            this.avoidBeacon2.Checked = true;
            this.avoidBeacon2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avoidBeacon2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.avoidBeacon2.Location = new System.Drawing.Point(177, 56);
            this.avoidBeacon2.Name = "avoidBeacon2";
            this.avoidBeacon2.Size = new System.Drawing.Size(111, 20);
            this.avoidBeacon2.TabIndex = 9;
            this.avoidBeacon2.Text = "Avoid Beacon";
            this.avoidBeacon2.UseVisualStyleBackColor = true;
            // 
            // avoidBeacon1
            // 
            this.avoidBeacon1.AutoSize = true;
            this.avoidBeacon1.Checked = true;
            this.avoidBeacon1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avoidBeacon1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.avoidBeacon1.Location = new System.Drawing.Point(177, 26);
            this.avoidBeacon1.Name = "avoidBeacon1";
            this.avoidBeacon1.Size = new System.Drawing.Size(111, 20);
            this.avoidBeacon1.TabIndex = 8;
            this.avoidBeacon1.Text = "Avoid Beacon";
            this.avoidBeacon1.UseVisualStyleBackColor = true;
            // 
            // tuneMode4
            // 
            this.tuneMode4.FormattingEnabled = true;
            this.tuneMode4.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Next new)",
            "Auto (Timed)"});
            this.tuneMode4.Location = new System.Drawing.Point(50, 115);
            this.tuneMode4.Name = "tuneMode4";
            this.tuneMode4.Size = new System.Drawing.Size(121, 21);
            this.tuneMode4.TabIndex = 7;
            this.tuneMode4.SelectedIndexChanged += new System.EventHandler(this.tuneMode4_SelectedIndexChanged);
            // 
            // tuneMode3
            // 
            this.tuneMode3.FormattingEnabled = true;
            this.tuneMode3.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Next new)",
            "Auto (Timed)"});
            this.tuneMode3.Location = new System.Drawing.Point(50, 85);
            this.tuneMode3.Name = "tuneMode3";
            this.tuneMode3.Size = new System.Drawing.Size(121, 21);
            this.tuneMode3.TabIndex = 6;
            this.tuneMode3.SelectedIndexChanged += new System.EventHandler(this.tuneMode3_SelectedIndexChanged);
            // 
            // tuneMode2
            // 
            this.tuneMode2.FormattingEnabled = true;
            this.tuneMode2.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Next new)",
            "Auto (Timed)"});
            this.tuneMode2.Location = new System.Drawing.Point(50, 55);
            this.tuneMode2.Name = "tuneMode2";
            this.tuneMode2.Size = new System.Drawing.Size(121, 21);
            this.tuneMode2.TabIndex = 5;
            this.tuneMode2.SelectedIndexChanged += new System.EventHandler(this.tuneMode2_SelectedIndexChanged);
            // 
            // tuneMode1
            // 
            this.tuneMode1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tuneMode1.FormattingEnabled = true;
            this.tuneMode1.Items.AddRange(new object[] {
            "Manual",
            "Auto (Hold)",
            "Auto (Next new)",
            "Auto (Timed)"});
            this.tuneMode1.Location = new System.Drawing.Point(50, 25);
            this.tuneMode1.Name = "tuneMode1";
            this.tuneMode1.Size = new System.Drawing.Size(121, 21);
            this.tuneMode1.TabIndex = 4;
            this.tuneMode1.SelectedIndexChanged += new System.EventHandler(this.tuneMode1_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label4.Location = new System.Drawing.Point(6, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "RX 4:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "RX 3:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label2.Location = new System.Drawing.Point(6, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "RX 2:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "RX 1:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(111, 365);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(219, 365);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.overPowerIndicatorLayout);
            this.groupBox2.Location = new System.Drawing.Point(15, 298);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 60);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Overpower Indicator Layout";
            // 
            // overPowerIndicatorLayout
            // 
            this.overPowerIndicatorLayout.FormattingEnabled = true;
            this.overPowerIndicatorLayout.Items.AddRange(new object[] {
            "classic",
            "classic + line",
            "box from top to line",
            "box from line to bottom",
            "line"});
            this.overPowerIndicatorLayout.Location = new System.Drawing.Point(9, 25);
            this.overPowerIndicatorLayout.Name = "overPowerIndicatorLayout";
            this.overPowerIndicatorLayout.Size = new System.Drawing.Size(162, 21);
            this.overPowerIndicatorLayout.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.autoTuneTimeValue);
            this.groupBox3.Controls.Add(this.autoHoldTimeValue);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(15, 202);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(304, 90);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Timer";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label8.Location = new System.Drawing.Point(6, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "Timed";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label7.Location = new System.Drawing.Point(8, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Hold Time";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label6.Location = new System.Drawing.Point(177, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Seconds";
            // 
            // autoTuneTimeValue
            // 
            this.autoTuneTimeValue.Location = new System.Drawing.Point(82, 55);
            this.autoTuneTimeValue.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.autoTuneTimeValue.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.autoTuneTimeValue.Name = "autoTuneTimeValue";
            this.autoTuneTimeValue.Size = new System.Drawing.Size(89, 20);
            this.autoTuneTimeValue.TabIndex = 6;
            this.autoTuneTimeValue.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // autoHoldTimeValue
            // 
            this.autoHoldTimeValue.Location = new System.Drawing.Point(82, 25);
            this.autoHoldTimeValue.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.autoHoldTimeValue.Name = "autoHoldTimeValue";
            this.autoHoldTimeValue.Size = new System.Drawing.Size(89, 20);
            this.autoHoldTimeValue.TabIndex = 5;
            this.autoHoldTimeValue.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label5.Location = new System.Drawing.Point(177, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Seconds";
            // 
            // BATCSpectrumSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 403);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BATCSpectrumSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BATCSpectrumSettings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treshHold)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoTuneTimeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.autoHoldTimeValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox tuneMode1;
        private System.Windows.Forms.ComboBox tuneMode2;
        private System.Windows.Forms.ComboBox tuneMode3;
        private System.Windows.Forms.ComboBox tuneMode4;
        private System.Windows.Forms.CheckBox avoidBeacon1;
        private System.Windows.Forms.CheckBox avoidBeacon2;
        private System.Windows.Forms.CheckBox avoidBeacon3;
        private System.Windows.Forms.CheckBox avoidBeacon4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox overPowerIndicatorLayout;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown autoHoldTimeValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown autoTuneTimeValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown treshHold;
    }
}
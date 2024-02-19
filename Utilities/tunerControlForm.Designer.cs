namespace opentuner
{
    partial class TunerControlForm
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
            this.lblgHz = new System.Windows.Forms.Label();
            this.lblmHz = new System.Windows.Forms.Label();
            this.lblkHz = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUpdateFreq = new System.Windows.Forms.Button();
            this.lblOffset = new System.Windows.Forms.Label();
            this.checkTunerOnTop = new System.Windows.Forms.CheckBox();
            this.lblNimFreq = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboSR = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.khzIndicator = new System.Windows.Forms.Label();
            this.mhzIndicator = new System.Windows.Forms.Label();
            this.ghzIndicator = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblgHz
            // 
            this.lblgHz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblgHz.Font = new System.Drawing.Font("Lucida Console", 69.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblgHz.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblgHz.Location = new System.Drawing.Point(15, 73);
            this.lblgHz.Margin = new System.Windows.Forms.Padding(0);
            this.lblgHz.Name = "lblgHz";
            this.lblgHz.Size = new System.Drawing.Size(233, 113);
            this.lblgHz.TabIndex = 0;
            this.lblgHz.Text = "000";
            this.lblgHz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblgHz.MouseEnter += new System.EventHandler(this.lblgHz_MouseEnter);
            this.lblgHz.MouseLeave += new System.EventHandler(this.lblgHz_MouseLeave);
            // 
            // lblmHz
            // 
            this.lblmHz.BackColor = System.Drawing.Color.Transparent;
            this.lblmHz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblmHz.Font = new System.Drawing.Font("Lucida Console", 69.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmHz.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblmHz.Location = new System.Drawing.Point(200, 73);
            this.lblmHz.Margin = new System.Windows.Forms.Padding(0);
            this.lblmHz.Name = "lblmHz";
            this.lblmHz.Size = new System.Drawing.Size(220, 113);
            this.lblmHz.TabIndex = 1;
            this.lblmHz.Text = "000";
            this.lblmHz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblmHz.MouseEnter += new System.EventHandler(this.lblmHz_MouseEnter);
            this.lblmHz.MouseLeave += new System.EventHandler(this.lblmHz_MouseLeave);
            // 
            // lblkHz
            // 
            this.lblkHz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblkHz.Font = new System.Drawing.Font("Lucida Console", 69.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblkHz.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblkHz.Location = new System.Drawing.Point(384, 73);
            this.lblkHz.Margin = new System.Windows.Forms.Padding(0);
            this.lblkHz.Name = "lblkHz";
            this.lblkHz.Size = new System.Drawing.Size(232, 113);
            this.lblkHz.TabIndex = 2;
            this.lblkHz.Text = "000";
            this.lblkHz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblkHz.MouseEnter += new System.EventHandler(this.lblkHz_MouseEnter);
            this.lblkHz.MouseLeave += new System.EventHandler(this.lblkHz_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Offset:";
            // 
            // btnUpdateFreq
            // 
            this.btnUpdateFreq.Enabled = false;
            this.btnUpdateFreq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateFreq.Location = new System.Drawing.Point(519, 223);
            this.btnUpdateFreq.Name = "btnUpdateFreq";
            this.btnUpdateFreq.Size = new System.Drawing.Size(88, 35);
            this.btnUpdateFreq.TabIndex = 4;
            this.btnUpdateFreq.Text = "Update";
            this.btnUpdateFreq.UseVisualStyleBackColor = true;
            this.btnUpdateFreq.Click += new System.EventHandler(this.btnUpdateFreq_Click);
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffset.Location = new System.Drawing.Point(68, 9);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(16, 17);
            this.lblOffset.TabIndex = 5;
            this.lblOffset.Text = "0";
            // 
            // checkTunerOnTop
            // 
            this.checkTunerOnTop.AutoSize = true;
            this.checkTunerOnTop.Location = new System.Drawing.Point(12, 241);
            this.checkTunerOnTop.Name = "checkTunerOnTop";
            this.checkTunerOnTop.Size = new System.Drawing.Size(84, 17);
            this.checkTunerOnTop.TabIndex = 6;
            this.checkTunerOnTop.Text = "Stay on Top";
            this.checkTunerOnTop.UseVisualStyleBackColor = true;
            this.checkTunerOnTop.CheckedChanged += new System.EventHandler(this.checkTunerOnTop_CheckedChanged);
            // 
            // lblNimFreq
            // 
            this.lblNimFreq.AutoSize = true;
            this.lblNimFreq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNimFreq.Location = new System.Drawing.Point(408, 232);
            this.lblNimFreq.Name = "lblNimFreq";
            this.lblNimFreq.Size = new System.Drawing.Size(0, 17);
            this.lblNimFreq.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(295, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Nim Freq (kHz):";
            // 
            // comboSR
            // 
            this.comboSR.FormattingEnabled = true;
            this.comboSR.Items.AddRange(new object[] {
            "66",
            "125",
            "250",
            "333",
            "500",
            "1000",
            "1500",
            "2000"});
            this.comboSR.Location = new System.Drawing.Point(510, 8);
            this.comboSR.Name = "comboSR";
            this.comboSR.Size = new System.Drawing.Size(83, 21);
            this.comboSR.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(408, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Symbol Rate :";
            // 
            // khzIndicator
            // 
            this.khzIndicator.AutoSize = true;
            this.khzIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.khzIndicator.ForeColor = System.Drawing.Color.DarkGreen;
            this.khzIndicator.Location = new System.Drawing.Point(408, 60);
            this.khzIndicator.Name = "khzIndicator";
            this.khzIndicator.Size = new System.Drawing.Size(0, 16);
            this.khzIndicator.TabIndex = 13;
            // 
            // mhzIndicator
            // 
            this.mhzIndicator.AutoSize = true;
            this.mhzIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mhzIndicator.ForeColor = System.Drawing.Color.DarkGreen;
            this.mhzIndicator.Location = new System.Drawing.Point(226, 60);
            this.mhzIndicator.Name = "mhzIndicator";
            this.mhzIndicator.Size = new System.Drawing.Size(0, 16);
            this.mhzIndicator.TabIndex = 14;
            // 
            // ghzIndicator
            // 
            this.ghzIndicator.AutoSize = true;
            this.ghzIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ghzIndicator.ForeColor = System.Drawing.Color.DarkGreen;
            this.ghzIndicator.Location = new System.Drawing.Point(40, 60);
            this.ghzIndicator.Name = "ghzIndicator";
            this.ghzIndicator.Size = new System.Drawing.Size(0, 16);
            this.ghzIndicator.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "SHIFT x10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "CTRL x100";
            // 
            // TunerControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(618, 270);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ghzIndicator);
            this.Controls.Add(this.mhzIndicator);
            this.Controls.Add(this.khzIndicator);
            this.Controls.Add(this.comboSR);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblNimFreq);
            this.Controls.Add(this.checkTunerOnTop);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.btnUpdateFreq);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblkHz);
            this.Controls.Add(this.lblmHz);
            this.Controls.Add(this.lblgHz);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TunerControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tuner Control";
            this.Load += new System.EventHandler(this.tunerControlForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblgHz;
        private System.Windows.Forms.Label lblmHz;
        private System.Windows.Forms.Label lblkHz;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpdateFreq;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.CheckBox checkTunerOnTop;
        private System.Windows.Forms.Label lblNimFreq;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox comboSR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label khzIndicator;
        private System.Windows.Forms.Label mhzIndicator;
        private System.Windows.Forms.Label ghzIndicator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
    }
}
namespace opentuner
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFreq = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLO = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSR = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDemoState = new System.Windows.Forms.Label();
            this.lblMer = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLnaGain = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblpower_i = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPower_q = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSR = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblModcod = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblLPDCError = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblBer = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblFreqCar = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Frequency: ";
            // 
            // txtFreq
            // 
            this.txtFreq.Location = new System.Drawing.Point(96, 52);
            this.txtFreq.Name = "txtFreq";
            this.txtFreq.Size = new System.Drawing.Size(147, 20);
            this.txtFreq.TabIndex = 4;
            this.txtFreq.Text = "10491520";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Offset :";
            // 
            // txtLO
            // 
            this.txtLO.Location = new System.Drawing.Point(96, 77);
            this.txtLO.Name = "txtLO";
            this.txtLO.Size = new System.Drawing.Size(147, 20);
            this.txtLO.TabIndex = 6;
            this.txtLO.Text = "9750000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Symbol Rate :";
            // 
            // txtSR
            // 
            this.txtSR.Location = new System.Drawing.Point(96, 102);
            this.txtSR.Name = "txtSR";
            this.txtSR.Size = new System.Drawing.Size(100, 20);
            this.txtSR.TabIndex = 8;
            this.txtSR.Text = "1500";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(96, 128);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Change";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblFreqCar);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.lblBer);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.lblLPDCError);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.lblModcod);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.lblSR);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.lblPower_q);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.lblpower_i);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblLnaGain);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblMer);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblDemoState);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(356, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 307);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " NIM Status ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Demod State: ";
            // 
            // lblDemoState
            // 
            this.lblDemoState.AutoSize = true;
            this.lblDemoState.Location = new System.Drawing.Point(168, 29);
            this.lblDemoState.Name = "lblDemoState";
            this.lblDemoState.Size = new System.Drawing.Size(13, 13);
            this.lblDemoState.TabIndex = 1;
            this.lblDemoState.Text = "0";
            // 
            // lblMer
            // 
            this.lblMer.AutoSize = true;
            this.lblMer.Location = new System.Drawing.Point(168, 54);
            this.lblMer.Name = "lblMer";
            this.lblMer.Size = new System.Drawing.Size(13, 13);
            this.lblMer.TabIndex = 3;
            this.lblMer.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Mer: ";
            // 
            // lblLnaGain
            // 
            this.lblLnaGain.AutoSize = true;
            this.lblLnaGain.Location = new System.Drawing.Point(168, 78);
            this.lblLnaGain.Name = "lblLnaGain";
            this.lblLnaGain.Size = new System.Drawing.Size(13, 13);
            this.lblLnaGain.TabIndex = 5;
            this.lblLnaGain.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "LNA Gain:";
            // 
            // lblpower_i
            // 
            this.lblpower_i.AutoSize = true;
            this.lblpower_i.Location = new System.Drawing.Point(168, 103);
            this.lblpower_i.Name = "lblpower_i";
            this.lblpower_i.Size = new System.Drawing.Size(13, 13);
            this.lblpower_i.TabIndex = 7;
            this.lblpower_i.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Power I:";
            // 
            // lblPower_q
            // 
            this.lblPower_q.AutoSize = true;
            this.lblPower_q.Location = new System.Drawing.Point(168, 129);
            this.lblPower_q.Name = "lblPower_q";
            this.lblPower_q.Size = new System.Drawing.Size(13, 13);
            this.lblPower_q.TabIndex = 9;
            this.lblPower_q.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 129);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Power Q";
            // 
            // lblSR
            // 
            this.lblSR.AutoSize = true;
            this.lblSR.Location = new System.Drawing.Point(168, 157);
            this.lblSR.Name = "lblSR";
            this.lblSR.Size = new System.Drawing.Size(13, 13);
            this.lblSR.TabIndex = 11;
            this.lblSR.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 157);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 10;
            this.label13.Text = "Symbol Rate:";
            // 
            // lblModcod
            // 
            this.lblModcod.AutoSize = true;
            this.lblModcod.Location = new System.Drawing.Point(168, 184);
            this.lblModcod.Name = "lblModcod";
            this.lblModcod.Size = new System.Drawing.Size(13, 13);
            this.lblModcod.TabIndex = 13;
            this.lblModcod.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 184);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Modcod:";
            // 
            // lblLPDCError
            // 
            this.lblLPDCError.AutoSize = true;
            this.lblLPDCError.Location = new System.Drawing.Point(168, 210);
            this.lblLPDCError.Name = "lblLPDCError";
            this.lblLPDCError.Size = new System.Drawing.Size(13, 13);
            this.lblLPDCError.TabIndex = 15;
            this.lblLPDCError.Text = "0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(18, 210);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(102, 13);
            this.label17.TabIndex = 14;
            this.label17.Text = "Errors LDPC Count :";
            // 
            // lblBer
            // 
            this.lblBer.AutoSize = true;
            this.lblBer.Location = new System.Drawing.Point(168, 236);
            this.lblBer.Name = "lblBer";
            this.lblBer.Size = new System.Drawing.Size(13, 13);
            this.lblBer.TabIndex = 17;
            this.lblBer.Text = "0";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(18, 236);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 13);
            this.label19.TabIndex = 16;
            this.label19.Text = "Ber: ";
            // 
            // lblFreqCar
            // 
            this.lblFreqCar.AutoSize = true;
            this.lblFreqCar.Location = new System.Drawing.Point(168, 264);
            this.lblFreqCar.Name = "lblFreqCar";
            this.lblFreqCar.Size = new System.Drawing.Size(13, 13);
            this.lblFreqCar.TabIndex = 19;
            this.lblFreqCar.Text = "0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(18, 264);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(124, 13);
            this.label21.TabIndex = 18;
            this.label21.Text = "Frequency Carrier Offset:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.txtSR);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLO);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFreq);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Tuner Tests";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFreq;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLO;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSR;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDemoState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblFreqCar;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblBer;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblLPDCError;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblModcod;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblSR;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblPower_q;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblpower_i;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblLnaGain;
        private System.Windows.Forms.Label label7;
    }
}


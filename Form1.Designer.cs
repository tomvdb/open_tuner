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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.statusPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFrequencyChange = new System.Windows.Forms.Button();
            this.txtSR = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLO = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFreq = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnectTuner = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFreqCar = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblBer = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblLPDCError = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblModcod = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblSR = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblPower_q = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblpower_i = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblLnaGain = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblMer = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblDemoState = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.debugPage = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.videoView1 = new LibVLCSharp.WinForms.VideoView();
            this.spectrum = new System.Windows.Forms.PictureBox();
            this.dbgListBox = new System.Windows.Forms.ListBox();
            this.websocketTimer = new System.Windows.Forms.Timer(this.components);
            this.lbldbMargin = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.statusPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.debugPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectrum)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1723, 972);
            this.splitContainer1.SplitterDistance = 436;
            this.splitContainer1.TabIndex = 12;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.statusPage);
            this.tabControl1.Controls.Add(this.debugPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(436, 972);
            this.tabControl1.TabIndex = 26;
            // 
            // statusPage
            // 
            this.statusPage.AutoScroll = true;
            this.statusPage.Controls.Add(this.groupBox2);
            this.statusPage.Controls.Add(this.btnConnectTuner);
            this.statusPage.Controls.Add(this.groupBox1);
            this.statusPage.Location = new System.Drawing.Point(4, 22);
            this.statusPage.Name = "statusPage";
            this.statusPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusPage.Size = new System.Drawing.Size(428, 946);
            this.statusPage.TabIndex = 0;
            this.statusPage.Text = "Status";
            this.statusPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnFrequencyChange);
            this.groupBox2.Controls.Add(this.txtSR);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtLO);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtFreq);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 359);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(397, 203);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tuner Control";
            // 
            // btnFrequencyChange
            // 
            this.btnFrequencyChange.Location = new System.Drawing.Point(102, 111);
            this.btnFrequencyChange.Name = "btnFrequencyChange";
            this.btnFrequencyChange.Size = new System.Drawing.Size(75, 23);
            this.btnFrequencyChange.TabIndex = 40;
            this.btnFrequencyChange.Text = "Change";
            this.btnFrequencyChange.UseVisualStyleBackColor = true;
            this.btnFrequencyChange.Click += new System.EventHandler(this.btnFrequencyChange_Click);
            // 
            // txtSR
            // 
            this.txtSR.Location = new System.Drawing.Point(102, 85);
            this.txtSR.Name = "txtSR";
            this.txtSR.Size = new System.Drawing.Size(100, 20);
            this.txtSR.TabIndex = 39;
            this.txtSR.Text = "1500";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Symbol Rate :";
            // 
            // txtLO
            // 
            this.txtLO.Location = new System.Drawing.Point(102, 60);
            this.txtLO.Name = "txtLO";
            this.txtLO.Size = new System.Drawing.Size(147, 20);
            this.txtLO.TabIndex = 37;
            this.txtLO.Text = "9750000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Offset :";
            // 
            // txtFreq
            // 
            this.txtFreq.Location = new System.Drawing.Point(102, 35);
            this.txtFreq.Name = "txtFreq";
            this.txtFreq.Size = new System.Drawing.Size(147, 20);
            this.txtFreq.TabIndex = 35;
            this.txtFreq.Text = "10491520";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Frequency: ";
            // 
            // btnConnectTuner
            // 
            this.btnConnectTuner.Location = new System.Drawing.Point(12, 17);
            this.btnConnectTuner.Name = "btnConnectTuner";
            this.btnConnectTuner.Size = new System.Drawing.Size(75, 23);
            this.btnConnectTuner.TabIndex = 26;
            this.btnConnectTuner.Text = "Connect";
            this.btnConnectTuner.UseVisualStyleBackColor = true;
            this.btnConnectTuner.Click += new System.EventHandler(this.btnConnectTuner_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbldbMargin);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 307);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " NIM Status ";
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
            // lblDemoState
            // 
            this.lblDemoState.AutoSize = true;
            this.lblDemoState.Location = new System.Drawing.Point(168, 29);
            this.lblDemoState.Name = "lblDemoState";
            this.lblDemoState.Size = new System.Drawing.Size(13, 13);
            this.lblDemoState.TabIndex = 1;
            this.lblDemoState.Text = "0";
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
            // debugPage
            // 
            this.debugPage.Controls.Add(this.dbgListBox);
            this.debugPage.Location = new System.Drawing.Point(4, 22);
            this.debugPage.Name = "debugPage";
            this.debugPage.Padding = new System.Windows.Forms.Padding(3);
            this.debugPage.Size = new System.Drawing.Size(428, 946);
            this.debugPage.TabIndex = 1;
            this.debugPage.Text = "Debug";
            this.debugPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.videoView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.spectrum);
            this.splitContainer2.Size = new System.Drawing.Size(1283, 972);
            this.splitContainer2.SplitterDistance = 710;
            this.splitContainer2.TabIndex = 1;
            // 
            // videoView1
            // 
            this.videoView1.BackColor = System.Drawing.Color.Black;
            this.videoView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoView1.Location = new System.Drawing.Point(0, 0);
            this.videoView1.MediaPlayer = null;
            this.videoView1.Name = "videoView1";
            this.videoView1.Size = new System.Drawing.Size(1283, 710);
            this.videoView1.TabIndex = 0;
            this.videoView1.Text = "videoView1";
            // 
            // spectrum
            // 
            this.spectrum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrum.Location = new System.Drawing.Point(0, 0);
            this.spectrum.Name = "spectrum";
            this.spectrum.Size = new System.Drawing.Size(1283, 258);
            this.spectrum.TabIndex = 0;
            this.spectrum.TabStop = false;
            this.spectrum.Click += new System.EventHandler(this.spectrum_Click);
            this.spectrum.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spectrum_MouseMove);
            // 
            // dbgListBox
            // 
            this.dbgListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbgListBox.FormattingEnabled = true;
            this.dbgListBox.Location = new System.Drawing.Point(3, 3);
            this.dbgListBox.Name = "dbgListBox";
            this.dbgListBox.Size = new System.Drawing.Size(422, 940);
            this.dbgListBox.TabIndex = 0;
            // 
            // websocketTimer
            // 
            this.websocketTimer.Enabled = true;
            this.websocketTimer.Interval = 2000;
            this.websocketTimer.Tick += new System.EventHandler(this.websocketTimer_Tick);
            // 
            // lbldbMargin
            // 
            this.lbldbMargin.AutoSize = true;
            this.lbldbMargin.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldbMargin.Location = new System.Drawing.Point(316, 17);
            this.lbldbMargin.Name = "lbldbMargin";
            this.lbldbMargin.Size = new System.Drawing.Size(75, 25);
            this.lbldbMargin.TabIndex = 20;
            this.lbldbMargin.Text = "D99.99";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1723, 972);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Open Tuner 0.1 (ZR6TG)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.statusPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.debugPage.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.videoView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectrum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private LibVLCSharp.WinForms.VideoView videoView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage statusPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFrequencyChange;
        private System.Windows.Forms.TextBox txtSR;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFreq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnectTuner;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.Label lblMer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblDemoState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage debugPage;
        private System.Windows.Forms.PictureBox spectrum;
        private System.Windows.Forms.ListBox dbgListBox;
        private System.Windows.Forms.Timer websocketTimer;
        private System.Windows.Forms.Label lbldbMargin;
    }
}


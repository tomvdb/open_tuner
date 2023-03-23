namespace opentuner
{
    partial class settingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(settingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDefaultLO2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboDefaultLNB = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDefaultLO = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtSnapshotPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkEnableSpectrum = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.checkEnableChat = new System.Windows.Forms.CheckBox();
            this.btnChatFontSetting = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDefaultLO2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboDefaultLNB);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtDefaultLO);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtDefaultLO2
            // 
            resources.ApplyResources(this.txtDefaultLO2, "txtDefaultLO2");
            this.txtDefaultLO2.Name = "txtDefaultLO2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboDefaultLNB
            // 
            this.comboDefaultLNB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDefaultLNB.FormattingEnabled = true;
            this.comboDefaultLNB.Items.AddRange(new object[] {
            resources.GetString("comboDefaultLNB.Items"),
            resources.GetString("comboDefaultLNB.Items1"),
            resources.GetString("comboDefaultLNB.Items2")});
            resources.ApplyResources(this.comboDefaultLNB, "comboDefaultLNB");
            this.comboDefaultLNB.Name = "comboDefaultLNB";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtDefaultLO
            // 
            resources.ApplyResources(this.txtDefaultLO, "txtDefaultLO");
            this.txtDefaultLO.Name = "txtDefaultLO";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboLanguage);
            this.groupBox2.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FormattingEnabled = true;
            this.comboLanguage.Items.AddRange(new object[] {
            resources.GetString("comboLanguage.Items"),
            resources.GetString("comboLanguage.Items1"),
            resources.GetString("comboLanguage.Items2"),
            resources.GetString("comboLanguage.Items3"),
            resources.GetString("comboLanguage.Items4"),
            resources.GetString("comboLanguage.Items5"),
            resources.GetString("comboLanguage.Items6"),
            resources.GetString("comboLanguage.Items7")});
            resources.ApplyResources(this.comboLanguage, "comboLanguage");
            this.comboLanguage.Name = "comboLanguage";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.txtSnapshotPath);
            this.groupBox3.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSnapshotPath
            // 
            resources.ApplyResources(this.txtSnapshotPath, "txtSnapshotPath");
            this.txtSnapshotPath.Name = "txtSnapshotPath";
            this.txtSnapshotPath.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnChatFontSetting);
            this.groupBox4.Controls.Add(this.checkEnableChat);
            this.groupBox4.Controls.Add(this.checkEnableSpectrum);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkEnableSpectrum
            // 
            resources.ApplyResources(this.checkEnableSpectrum, "checkEnableSpectrum");
            this.checkEnableSpectrum.Name = "checkEnableSpectrum";
            this.checkEnableSpectrum.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // checkEnableChat
            // 
            resources.ApplyResources(this.checkEnableChat, "checkEnableChat");
            this.checkEnableChat.Name = "checkEnableChat";
            this.checkEnableChat.UseVisualStyleBackColor = true;
            // 
            // btnChatFontSetting
            // 
            resources.ApplyResources(this.btnChatFontSetting, "btnChatFontSetting");
            this.btnChatFontSetting.Name = "btnChatFontSetting";
            this.btnChatFontSetting.UseVisualStyleBackColor = true;
            this.btnChatFontSetting.Click += new System.EventHandler(this.btnChatFontSetting_Click);
            // 
            // settingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "settingsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        public System.Windows.Forms.CheckBox checkEnableSpectrum;
        public System.Windows.Forms.ComboBox comboDefaultLNB;
        public System.Windows.Forms.TextBox txtDefaultLO;
        public System.Windows.Forms.TextBox txtSnapshotPath;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox comboLanguage;
        public System.Windows.Forms.TextBox txtDefaultLO2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnChatFontSetting;
        public System.Windows.Forms.CheckBox checkEnableChat;
    }
}
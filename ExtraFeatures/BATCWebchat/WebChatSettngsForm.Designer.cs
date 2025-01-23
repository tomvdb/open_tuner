namespace opentuner.ExtraFeatures.BATCWebchat
{
    partial class WebChatSettngsForm
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.numChatFontSize = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSigReportTemplate = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChatFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtNick);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.numChatFontSize);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.txtSigReportTemplate);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Location = new System.Drawing.Point(16, 15);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(924, 279);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "QO-100 Specific";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(19, 46);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(95, 16);
            this.label17.TabIndex = 7;
            this.label17.Text = "Chat Font Size:";
            // 
            // numChatFontSize
            // 
            this.numChatFontSize.Location = new System.Drawing.Point(132, 43);
            this.numChatFontSize.Margin = new System.Windows.Forms.Padding(4);
            this.numChatFontSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numChatFontSize.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numChatFontSize.Name = "numChatFontSize";
            this.numChatFontSize.ReadOnly = true;
            this.numChatFontSize.Size = new System.Drawing.Size(79, 22);
            this.numChatFontSize.TabIndex = 6;
            this.numChatFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(19, 162);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(142, 96);
            this.label14.TabIndex = 5;
            this.label14.Text = "{SN} - ServiceName\r\n{SP} - ServiceProvider\r\n{DBM} - db Margin (D)\r\n{MER} - Mer\r\n{" +
    "SR} - Symbol Rate\r\n{FREQ} - Freq\r\n";
            // 
            // txtSigReportTemplate
            // 
            this.txtSigReportTemplate.Location = new System.Drawing.Point(23, 128);
            this.txtSigReportTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.txtSigReportTemplate.Name = "txtSigReportTemplate";
            this.txtSigReportTemplate.Size = new System.Drawing.Size(892, 22);
            this.txtSigReportTemplate.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(19, 101);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(153, 16);
            this.label13.TabIndex = 3;
            this.label13.Text = "Signal Report Template:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(840, 302);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(732, 302);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(266, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nickname:";
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(343, 42);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(572, 22);
            this.txtNick.TabIndex = 9;
            // 
            // WebChatSettngsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(960, 352);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox4);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WebChatSettngsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "QO-100 Web Chat Settings";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChatFontSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.NumericUpDown numChatFontSize;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.TextBox txtSigReportTemplate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.Label label1;
    }
}
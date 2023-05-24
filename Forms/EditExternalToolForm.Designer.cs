namespace opentuner.Forms
{
    partial class EditExternalToolForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtToolName = new System.Windows.Forms.TextBox();
            this.txtToolPath = new System.Windows.Forms.TextBox();
            this.txtToolParameters = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkUdp1 = new System.Windows.Forms.CheckBox();
            this.checkUdp2 = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tool Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tool Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tool Parameters:";
            // 
            // txtToolName
            // 
            this.txtToolName.Location = new System.Drawing.Point(137, 17);
            this.txtToolName.Name = "txtToolName";
            this.txtToolName.Size = new System.Drawing.Size(221, 20);
            this.txtToolName.TabIndex = 3;
            // 
            // txtToolPath
            // 
            this.txtToolPath.Location = new System.Drawing.Point(137, 45);
            this.txtToolPath.Name = "txtToolPath";
            this.txtToolPath.ReadOnly = true;
            this.txtToolPath.Size = new System.Drawing.Size(221, 20);
            this.txtToolPath.TabIndex = 4;
            // 
            // txtToolParameters
            // 
            this.txtToolParameters.Location = new System.Drawing.Point(137, 72);
            this.txtToolParameters.Name = "txtToolParameters";
            this.txtToolParameters.Size = new System.Drawing.Size(221, 20);
            this.txtToolParameters.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(364, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 21);
            this.button1.TabIndex = 6;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkUdp1
            // 
            this.checkUdp1.AutoSize = true;
            this.checkUdp1.Location = new System.Drawing.Point(137, 110);
            this.checkUdp1.Name = "checkUdp1";
            this.checkUdp1.Size = new System.Drawing.Size(131, 17);
            this.checkUdp1.TabIndex = 7;
            this.checkUdp1.Text = "Enable UDP (Tuner 1)";
            this.checkUdp1.UseVisualStyleBackColor = true;
            // 
            // checkUdp2
            // 
            this.checkUdp2.AutoSize = true;
            this.checkUdp2.Location = new System.Drawing.Point(137, 133);
            this.checkUdp2.Name = "checkUdp2";
            this.checkUdp2.Size = new System.Drawing.Size(131, 17);
            this.checkUdp2.TabIndex = 8;
            this.checkUdp2.Text = "Enable UDP (Tuner 2)";
            this.checkUdp2.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSave.Location = new System.Drawing.Point(283, 181);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Location = new System.Drawing.Point(202, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EditExternalToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(409, 216);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.checkUdp2);
            this.Controls.Add(this.checkUdp1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtToolParameters);
            this.Controls.Add(this.txtToolPath);
            this.Controls.Add(this.txtToolName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditExternalToolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit External Tool Properties";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtToolName;
        public System.Windows.Forms.TextBox txtToolPath;
        public System.Windows.Forms.TextBox txtToolParameters;
        public System.Windows.Forms.CheckBox checkUdp1;
        public System.Windows.Forms.CheckBox checkUdp2;
    }
}